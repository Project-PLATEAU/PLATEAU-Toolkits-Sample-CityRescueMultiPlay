using PLATEAU.CityInfo;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using TMPro;
using System.Collections;

namespace LobbyRelaySample.ngo
{
    public class PlayerCam : NetworkBehaviour
    {
        Camera m_mainCamera;
        NetworkVariable<Vector3> m_position = new NetworkVariable<Vector3>(new Vector3(20.8f, 76.8f, -70.3f));
        NetworkVariable<FixedString128Bytes> m_SelectedBuilding = new NetworkVariable<FixedString128Bytes>();

        ulong m_localId;

        [SerializeField]
        TMPro.TMP_Text m_nameOutput = default;

        [SerializeField]
        GameObject m_PlayerAvatar;

        // If the local player cursor spawns before this cursor's owner, the owner's data won't be available yet. This is used to retrieve the data later.
        Action<ulong, Action<PlayerData>> m_retrieveName;

        public float moveSpeed = 5.0f;
        public float sensitivity = 2.0f;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        public Vector3 m_TargetPos;

        public float minVerticalAngle = -90f;
        public float maxVerticalAngle = 90f;

        float minHeight = 60f;

        AttributeExtractor m_AttributeExtractor;

        SubSceneLoader m_SubSceneLoader;

        /// <summary>
        /// This is necessary because wew use WASD to move the avatars and when typing in the chat box, the avatars will also move due to the keypresses.
        /// We will search for this gameobject by hardcoded name.
        /// </summary>
        TMP_InputField m_ChatBoxInputField;
        bool m_CanMove;

        /// <summary>
        /// This cursor is spawned in dynamically but needs references to some scene objects. Pushing full object references over RPC calls
        /// is an option if we create custom data for each and ensure they're all spawned on the host correctly, but it's simpler to do
        /// some one-time retrieval here instead.
        /// This also sets up the visuals to make remote player cursors appear distinct from the local player's cursor.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            m_retrieveName = NetworkedDataStore.Instance.GetPlayerData;
            Camera localCamera = GetComponent<Camera>();
            if (IsOwner)
            {
                localCamera.depth = 99;

                if (localCamera != null)
                {
                    localCamera.enabled = true;
                }

                m_AttributeExtractor = FindObjectOfType<AttributeExtractor>();
                m_mainCamera = localCamera;
            }
            else
            {
                if (localCamera != null)
                {
                    localCamera.enabled = false;
                }
            }

            SampleSceneManager.Instance.onGameBeginning += OnGameBegan;

            m_SelectedBuilding.OnValueChanged += ExtractAtributes;

            m_localId = NetworkManager.Singleton.LocalClientId;

            m_CanMove = true;
        }

        public void OnGameBegan()
        {
            m_retrieveName.Invoke(OwnerClientId, SetName_ClientRpc);
            SampleSceneManager.Instance.onGameBeginning -= OnGameBegan;
        }

        private void Start()
        {
            if (m_mainCamera == null)
            {
                Camera[] cameras = Camera.allCameras;
                foreach (Camera camera in cameras)
                {
                    if (camera.enabled && camera.gameObject.GetComponent<PlayerCam>() != null)
                    {
                        m_mainCamera = camera;
                    }
                }
            }

            if (IsOwner)
            {
                StartCoroutine(LoadSubScenes());
            }

            // The scene is setup such that the chat input field is available in the scene, but it may be disabled
            TMP_InputField[] inputFieldCandidates = FindObjectsOfType<TMP_InputField>(true);
            foreach (TMP_InputField inputField in inputFieldCandidates)
            {
                if (inputField.gameObject.name == "ChatInputField")
                {
                    m_ChatBoxInputField = inputField;
                }
            }

            if (IsOwner && m_ChatBoxInputField != null)
            {
                m_ChatBoxInputField.onSelect.AddListener(StopMove);
                m_ChatBoxInputField.onDeselect.AddListener(CanMove);
            }
        }

        private void CanMove(string arg0)
        {
            m_CanMove = true;
        }

        void StopMove(string arg0)
        {
            m_CanMove = false;
        }

        IEnumerator LoadSubScenes()
        {
            yield return new WaitForSeconds(5f);
            if (IsOwner)
            {
                if (m_SubSceneLoader == null)
                {
                    m_SubSceneLoader = FindObjectOfType<SubSceneLoader>();
                    StartCoroutine(LoadSubScenes());
                }
                else
                {
                    m_SubSceneLoader.LoadSubScenes();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsOwner)
            {
                transform.position = m_position.Value;

                // there is a possibility that in the client, the Non owner player cam is spawned first. This will prevent it from finding an active camera during Start()
                if (m_mainCamera != null)
                {
                    m_PlayerAvatar.transform.LookAt(m_PlayerAvatar.transform.position + m_mainCamera.transform.rotation * Vector3.forward, m_mainCamera.transform.rotation * Vector3.up);
                }
                else
                {
                    Camera[] cameras = Camera.allCameras;
                    foreach (Camera camera in cameras)
                    {
                        if (camera.enabled && camera.gameObject.GetComponent<PlayerCam>() != null)
                        {
                            m_mainCamera = camera;
                        }
                    }
                }
                if (m_nameOutput != null && m_mainCamera != null)
                {
                    m_nameOutput.transform.LookAt(m_mainCamera.transform.position);
                }
            }
            else
            {
                if (m_CanMove)
                {
                    HandleVertical();
                    HandleMovement();
                    HandleRotation();
                    SetPosition_ServerRpc(transform.position);
                }
            }
            HandleObjectSelection();
        }

        [ServerRpc] // Leave (RequireOwnership = true) for these so that only the player whose cursor this is can make updates.
        private void SetPosition_ServerRpc(Vector3 position)
        {
            m_position.Value = position;
        }

        [ServerRpc(RequireOwnership = false)] // any client can make changes to the selected building
        private void SetSelectedBuildingName_ServerRpc(string selectedObjectName)
        {
            m_SelectedBuilding.Value = selectedObjectName;
        }

        void HandleObjectSelection()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (m_AttributeExtractor == null)
                {
                    m_AttributeExtractor = FindObjectOfType<AttributeExtractor>();
                }

                // Create a ray from the mouse cursor on screen in the direction of the camera
                Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit[] hitsFromOwnClick = Physics.RaycastAll(ray);
                CityObjectList.CityObject firstPrimaryObj = new CityObjectList.CityObject();
                GameObject selected = null;

                foreach (RaycastHit hit in hitsFromOwnClick)
                {
                    PLATEAUCityObjectGroup cityObjectGroup = hit.collider.GetComponent<PLATEAUCityObjectGroup>();
                    if (cityObjectGroup != null)
                    {
                        firstPrimaryObj = cityObjectGroup.PrimaryCityObjects.FirstOrDefault();
                        if (firstPrimaryObj != null && firstPrimaryObj.CityObjectType == PLATEAU.CityGML.CityObjectType.COT_Building)
                        {
                            selected = hit.collider.gameObject;
                        }
                    }
                }

                SetSelectedBuildingName_ServerRpc(selected != null ? selected.name : "");
            }
        }

        [ClientRpc]
        private void SetName_ClientRpc(PlayerData data)
        {
            if (!IsOwner)
                m_nameOutput.text = data.name;
        }

        void ExtractAtributes(FixedString128Bytes prev, FixedString128Bytes cur)
        {
            if (m_AttributeExtractor != null)
            {
                m_AttributeExtractor.ShowAttributes(m_SelectedBuilding.Value.ToString());
            }
            else
            {
                m_AttributeExtractor = FindObjectOfType<AttributeExtractor>();
            }
        }

        // Camera controls
        void HandleMovement()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Calculate the movement direction in the camera's local space.
            Vector3 localMovement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized * moveSpeed * Time.deltaTime;

            // Convert local movement direction to world space.
            Vector3 worldMovement = transform.TransformDirection(localMovement);

            // Remove any Y movement to keep the height constant.
            worldMovement.y = 0;

            // Apply the movement.
            transform.Translate(worldMovement, Space.World);
        }

        void HandleVertical()
        {
            Vector3 position = transform.position;

            if (Input.GetKey(KeyCode.Q) && position.y > minHeight)
            {
                // Move down
                position.y -= moveSpeed * Time.deltaTime;
                position.y = Mathf.Max(position.y, minHeight); // Ensure not going below minHeight
            }
            else if (Input.GetKey(KeyCode.E))
            {
                // Move up
                position.y += moveSpeed * Time.deltaTime;
            }

            transform.position = position;
        }

        void HandleRotation()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // マウスボタンが押された瞬間に現在のカメラの回転を取得
                Vector3 eulerAngles = transform.eulerAngles;
                rotationX = eulerAngles.y;
                rotationY = eulerAngles.x > 180 ? eulerAngles.x - 360 : eulerAngles.x;
            }

            if (Input.GetMouseButton(0))
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivity;
                rotationY -= Input.GetAxis("Mouse Y") * sensitivity;

                // rotationYをminVerticalAngleとmaxVerticalAngleの間に制限する
                rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);

                // カメラの位置と回転を更新
                Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
                transform.rotation = rotation;
            }
        }
    }
}