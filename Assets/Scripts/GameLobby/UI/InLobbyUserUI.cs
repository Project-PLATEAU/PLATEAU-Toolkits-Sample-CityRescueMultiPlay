using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

namespace LobbyRelaySample.UI
{
    /// <summary>
    /// When inside a lobby, this will show information about a player, whether local or remote.
    /// </summary>
    public class InLobbyUserUI : UIPanelBase
    {
        [SerializeField]
        TMP_Text m_DisplayNameText;

        [SerializeField]
        TMP_Text m_StatusText;

        [SerializeField]
        Image m_HostIcon;

        [SerializeField]
        vivox.VivoxUserHandler m_VivoxUserHandler;

        [SerializeField]
        TMPro.TMP_InputField m_ChatInputField;

        [SerializeField]
        ChatUi m_ChatUi;

        private ChannelId m_LobbyChannelId;

        public bool IsAssigned => UserId != null;
        public string UserId { get; set; }
        public LocalPlayer LocalPlayer { get; private set; }

        public async void SetUser(LocalPlayer localPlayer)
        {
            Show();
            LocalPlayer = localPlayer;
            UserId = localPlayer.ID.Value;
            SetIsHost(localPlayer.IsHost.Value);
            SetUserStatus(localPlayer.UserStatus.Value);
            SetDisplayName(LocalPlayer.DisplayName.Value);
            SubscribeToPlayerUpdates();

            m_VivoxUserHandler.SetId(UserId);
            m_ChatInputField.onEndEdit.AddListener((string text) => { EnterKeyOnTextField(); });
            m_VivoxUserHandler.OnChannelIdReady += SetLobbyId;

            LocalPlayer user = await GameManager.Instance.AwaitLocalUserInitialization();

            if (user.ID.Value == UserId)
            {
                m_ChatUi.Initialize(LocalPlayer, m_VivoxUserHandler);
            }
        }
        void SetLobbyId(ChannelId channelId)
        {
            m_LobbyChannelId = channelId;
        }

        void SubscribeToPlayerUpdates()
        {
            LocalPlayer.DisplayName.onChanged += SetDisplayName;
            LocalPlayer.UserStatus.onChanged += SetUserStatus;
            LocalPlayer.IsHost.onChanged += SetIsHost;
        }

        void UnsubscribeToPlayerUpdates()
        {
            if (LocalPlayer == null)
                return;
            if (LocalPlayer.DisplayName?.onChanged != null)
                LocalPlayer.DisplayName.onChanged -= SetDisplayName;
            if (LocalPlayer.UserStatus?.onChanged != null)
                LocalPlayer.UserStatus.onChanged -= SetUserStatus;
            if (LocalPlayer.IsHost?.onChanged != null)
                LocalPlayer.IsHost.onChanged -= SetIsHost;
        }

        public void ResetUI()
        {
            if (LocalPlayer == null)
                return;
            UserId = null;
            SetUserStatus(PlayerStatus.Lobby);
            Hide();
            UnsubscribeToPlayerUpdates();
            LocalPlayer = null;
        }

        void SetDisplayName(string displayName)
        {
            m_DisplayNameText.SetText(displayName);
        }

        void SetUserStatus(PlayerStatus statusText)
        {
            m_StatusText.SetText(SetStatusFancy(statusText));
        }

        void SetIsHost(bool isHost)
        {
            m_HostIcon.enabled = isHost;
        }

        string SetStatusFancy(PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Lobby:
                    return "<color=#56B4E9>待機中</color>"; // Light Blue
                case PlayerStatus.Ready:
                    return "<color=#009E73>準備完了</color>"; // Light Mint
                case PlayerStatus.Connecting:
                    return "<color=#F0E442>接続中…</color>"; // Bright Yellow
                case PlayerStatus.InGame:
                    return "<color=#005500>参加中</color>"; // Green
                default:
                    return "";
            }
        }

        private void EnterKeyOnTextField()
        {
            if (!Input.GetKeyDown(KeyCode.Return))
            {
                return;
            }
            SubmitTextToVivox();
        }

        private void SubmitTextToVivox()
        {
            if (string.IsNullOrEmpty(m_ChatInputField.text))
            {
                return;
            }

            m_VivoxUserHandler.SendTextMessage(m_ChatInputField.text, m_LobbyChannelId);
            ClearOutTextField();
        }

        private void ClearOutTextField()
        {
            m_ChatInputField.text = string.Empty;
            m_ChatInputField.Select();
            m_ChatInputField.ActivateInputField();
        }
    }
}