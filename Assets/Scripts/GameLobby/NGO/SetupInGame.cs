using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LobbyRelaySample.ngo
{
    /// <summary>
    /// Once the local localPlayer is in a localLobby and that localLobby has entered the In-Game state, this will load in whatever is necessary to actually run the game part.
    /// This will exist in the game scene so that it can hold references to scene objects that spawned prefab instances will need.
    /// </summary>
    public class SetupInGame : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_disableWhileInGame = default;

        private SampleSceneManager m_SampleSceneManager;

        private bool m_doesNeedCleanup = false;
        private bool m_hasConnectedViaNGO = false;

        private LocalLobby m_lobby;
        private LocalPlayer m_player;

        [SerializeField]
        SampleSceneManager m_SceneManagerPrefab;

        [SerializeField]
        private Camera m_LobbyCamera;

        [SerializeField]
        private GameObject m_LoadingScreen;

        Scene m_LoadedScene;

        private void SetMenuVisibility(bool areVisible)
        {
            foreach (GameObject go in m_disableWhileInGame)
                go.SetActive(areVisible);
        }

        /// <summary>
        /// The prefab with the NetworkManager contains all of the assets and logic needed to set up the NGO minigame.
        /// The UnityTransport needs to also be set up with a new Allocation from Relay.
        /// </summary>
        async Task CreateNetworkManager(LocalLobby localLobby, LocalPlayer localPlayer)
        {
            m_lobby = localLobby;
            m_player = localPlayer;
            m_SampleSceneManager = Instantiate(m_SceneManagerPrefab);
            m_SampleSceneManager.Initialize(OnConnectionVerified, m_lobby.PlayerCount, OnGameBegin, OnGameEnd, localPlayer);

            // Now that the scene is loaded, continue with the network setup
            if (localPlayer.IsHost.Value)
            {
                await SetRelayHostData();
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                await AwaitRelayCode(localLobby);
                await SetRelayClientData();
                NetworkManager.Singleton.StartClient();
            }

            NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;

            if (localPlayer.IsHost.Value)
            {
                var status = NetworkManager.Singleton.SceneManager.LoadScene("FloodSimulation", LoadSceneMode.Additive);
                CheckStatus(status);
            }
        }


        async Task AwaitRelayCode(LocalLobby lobby)
        {
            string relayCode = lobby.RelayCode.Value;
            lobby.RelayCode.onChanged += (code) => relayCode = code;
            while (string.IsNullOrEmpty(relayCode))
            {
                await Task.Delay(100);
            }
        }

        async Task SetRelayHostData()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();

            var allocation = await Relay.Instance.CreateAllocationAsync(m_lobby.MaxPlayerCount.Value);
            var joincode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            GameManager.Instance.HostSetRelayCode(joincode);

            bool isSecure = false;
            var endpoint = GetEndpointForAllocation(allocation.ServerEndpoints,
                allocation.RelayServer.IpV4, allocation.RelayServer.Port, out isSecure);

            transport.SetHostRelayData(AddressFromEndpoint(endpoint), endpoint.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, isSecure);
        }

        async Task SetRelayClientData()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponentInChildren<UnityTransport>();

            var joinAllocation = await Relay.Instance.JoinAllocationAsync(m_lobby.RelayCode.Value);
            bool isSecure = false;
            var endpoint = GetEndpointForAllocation(joinAllocation.ServerEndpoints,
                joinAllocation.RelayServer.IpV4, joinAllocation.RelayServer.Port, out isSecure);

            transport.SetClientRelayData(AddressFromEndpoint(endpoint), endpoint.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key,
                joinAllocation.ConnectionData, joinAllocation.HostConnectionData, isSecure);
        }

        /// <summary>
        /// Determine the server endpoint for connecting to the Relay server, for either an Allocation or a JoinAllocation.
        /// If DTLS encryption is available, and there's a secure server endpoint available, use that as a secure connection. Otherwise, just connect to the Relay IP unsecured.
        /// </summary>
        NetworkEndPoint GetEndpointForAllocation(
            List<RelayServerEndpoint> endpoints,
            string ip,
            int port,
            out bool isSecure)
        {
#if ENABLE_MANAGED_UNITYTLS
            foreach (RelayServerEndpoint endpoint in endpoints)
            {
                if (endpoint.Secure && endpoint.Network == RelayServerEndpoint.NetworkOptions.Udp)
                {
                    isSecure = true;
                    return NetworkEndPoint.Parse(endpoint.Host, (ushort)endpoint.Port);
                }
            }
#endif
            isSecure = false;
            return NetworkEndPoint.Parse(ip, (ushort)port);
        }

        string AddressFromEndpoint(NetworkEndPoint endpoint)
        {
            return endpoint.Address.Split(':')[0];
        }

        void OnConnectionVerified()
        {
            m_hasConnectedViaNGO = true;
        }

        public void StartNetworkedGame(LocalLobby localLobby, LocalPlayer localPlayer)
        {
            m_doesNeedCleanup = true;
            SetMenuVisibility(false);
#pragma warning disable 4014
            CreateNetworkManager(localLobby, localPlayer);
#pragma warning restore 4014
            m_LoadingScreen.SetActive(true);
        }

        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
            {
                StartCoroutine(WaitAndDisableLobbyCamera());
            }
        }

        private IEnumerator WaitAndDisableLobbyCamera()
        {
            // If not host, wait 5 s.
            if (!NetworkManager.Singleton.IsHost)
            {
                yield return new WaitForSeconds(5f);
            }

            // Hide the loading screen.
            if (m_LoadingScreen != null)
            {
                m_LoadingScreen.SetActive(false);
            }

            // Disable the lobby camera
            if (m_LobbyCamera != null)
            {
                m_LobbyCamera.enabled = false;
            }
        }
        private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
        {
            var sceneEventAction = isLoading ? "load" : "unload";
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to {sceneEventAction} Simulation with" +
                    $" a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }

        public void OnGameBegin()
        {
            if (!m_hasConnectedViaNGO)
            {
                // If this localPlayer hasn't successfully connected via NGO, forcibly exit the minigame.
                LogHandlerSettings.Instance.SpawnErrorPopup("Failed to join the game.");
                OnGameEnd();
            }
        }

        /// <summary>
        /// Return to the localLobby after the game, whether due to the game ending or due to a failed connection.
        /// </summary>
        public void OnGameEnd()
        {
            Debug.Log("GmameEnd");

            if (m_LobbyCamera != null)
            {
                m_LobbyCamera.enabled = true;
            }

            if (m_LoadingScreen != null)
            {
                m_LoadingScreen.SetActive(false);
            }

            if (m_doesNeedCleanup)
            {
                NetworkManager.Singleton.Shutdown(true);
                Destroy(m_SampleSceneManager.gameObject);
                if (m_player.IsHost.Value)
                {
                    NetworkManager.Singleton.SceneManager.UnloadScene(m_LoadedScene);

                    bool isSceneLoaded = IsSceneLoaded("Simulation");
                    if (isSceneLoaded)
                    {
                        SceneManager.UnloadSceneAsync("Simulation");
                    }
                    else
                    {
                        Debug.Log("The scene is not loaded.");
                    }
                }
                else
                {
                    bool isSceneLoaded = IsSceneLoaded("Simulation");
                    if (isSceneLoaded)
                    {
                        SceneManager.UnloadSceneAsync("Simulation");
                    }
                    else
                    {
                        Debug.Log("The scene is not loaded.");
                    }

                }
                SetMenuVisibility(true);
                m_lobby.RelayCode.Value = "";
                GameManager.Instance.EndGame();
                m_doesNeedCleanup = false;

                // when game ends, throw the player outside the lobby
                GameManager.Instance.UIChangeMenuState(GameState.Menu);
            }
        }

        public bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true; // The scene is loaded.
                }
            }
            return false; // The scene is not loaded.
        }
    }
}