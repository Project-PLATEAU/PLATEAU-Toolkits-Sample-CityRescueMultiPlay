using LobbyRelaySample;
using LobbyRelaySample.ngo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

namespace LobbyRelaySample.ngo
{
    internal class SampleSceneManager : NetworkBehaviour
    {
        Action m_onConnectionVerified, m_onGameEnd;

        [SerializeField]
        private PlayerCam m_playerCursorPrefab = default;
        public Action onGameBeginning;

        private int
                m_expectedPlayerCount; // Used by the host, but we can't call the RPC until the network connection completes.
        private bool? m_canSpawnInGameObjects;
        private PlayerData
                m_localUserData; // This has an ID that's not necessarily the OwnerClientId, since all clients will see all spawned objects regardless of ownership.

        private float m_timeout = 30f;
        private bool m_hasConnected = false;

        [SerializeField]
        private NetworkedDataStore m_dataStore = default;

        public static SampleSceneManager Instance
        {
            get
            {
                if (s_Instance!) return s_Instance;
                return s_Instance = FindObjectOfType<SampleSceneManager>();
            }
        }

        static SampleSceneManager s_Instance;

        void Update()
        {
            if (m_timeout >= 0)
            {
                m_timeout -= Time.deltaTime;
                if (m_timeout < 0)
                    BeginGame();
            }
        }

        public void Initialize(Action onConnectionVerified, int expectedPlayerCount, Action onGameBegin,
                Action onGameEnd,
                LocalPlayer localUser)
        {
            m_onConnectionVerified = onConnectionVerified;
            m_expectedPlayerCount = expectedPlayerCount;
            onGameBeginning = onGameBegin;
            m_onGameEnd = onGameEnd;
            m_canSpawnInGameObjects = null;
            m_localUserData = new PlayerData(localUser.DisplayName.Value, 0);
        }

        public override void OnNetworkSpawn()
        {
            m_localUserData = new PlayerData(m_localUserData.name, NetworkManager.Singleton.LocalClientId);
            VerifyConnection_ServerRpc(m_localUserData.id);
        }

        /// <summary>
        /// To verify the connection, invoke a server RPC call that then invokes a client RPC call. After this, the actual setup occurs.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void VerifyConnection_ServerRpc(ulong clientId)
        {
            VerifyConnection_ClientRpc(clientId);

            // While we could start pooling symbol objects now, incoming clients would be flooded with the Spawn calls.
            // This could lead to dropped packets such that the InGameRunner's Spawn call fails to occur, so we'll wait until all players join.
        }

        [ClientRpc]
        private void VerifyConnection_ClientRpc(ulong clientId)
        {
            if (clientId == m_localUserData.id)
                VerifyConnectionConfirm_ServerRpc(m_localUserData);
        }

        /// <summary>
        /// Once the connection is confirmed, spawn a player cursor and check if all players have connected.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void VerifyConnectionConfirm_ServerRpc(PlayerData clientData)
        {
            // Note that the client will not receive the cursor object reference, so the cursor must handle initializing itself.
            PlayerCam playerCursor = Instantiate(m_playerCursorPrefab);
            playerCursor.NetworkObject.SpawnWithOwnership(clientData.id);
            playerCursor.name += clientData.name;
            m_dataStore.AddPlayer(clientData.id, clientData.name);
            // The game will begin at this point, or else there's a timeout for booting any unconnected players.
            bool areAllPlayersConnected = NetworkManager.Singleton.ConnectedClients.Count >= m_expectedPlayerCount;
            VerifyConnectionConfirm_ClientRpc(clientData.id, areAllPlayersConnected);
        }

        [ClientRpc]
        private void VerifyConnectionConfirm_ClientRpc(ulong clientId, bool canBeginGame)
        {
            if (clientId == m_localUserData.id)
            {
                m_onConnectionVerified?.Invoke();
                m_hasConnected = true;
            }

            if (canBeginGame && m_hasConnected)
            {
                m_timeout = -1;
                BeginGame();
            }
        }

        void BeginGame()
        {
            m_canSpawnInGameObjects = true;
            GameManager.Instance.BeginGame();
            onGameBeginning?.Invoke();
        }

        public void EndGame()
        {
            if (IsHost)
            {
                StartCoroutine(EndGame_ClientsFirst());
            }
            else
            {
                SendLocalEndGameSignal();
            }
        }

        private IEnumerator EndGame_ClientsFirst()
        {
            EndGame_ClientRpc();
            yield return null;
            SendLocalEndGameSignal();
        }

        [ClientRpc]
        private void EndGame_ClientRpc()
        {
            if (IsHost)
                return;
            SendLocalEndGameSignal();
        }

        private void SendLocalEndGameSignal()
        {
            m_onGameEnd();
        }
    }
}