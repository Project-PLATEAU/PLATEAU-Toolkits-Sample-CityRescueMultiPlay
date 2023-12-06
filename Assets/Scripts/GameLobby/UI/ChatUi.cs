using LobbyRelaySample.vivox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

namespace LobbyRelaySample.UI
{
    public class ChatUi : UIPanelBase
    {
        [SerializeField] LobbyUserListUI m_UserListHolder;

        VivoxUserHandler m_VivoxUserHandler;

        [SerializeField]
        GameObject m_ChatContentPanel;

        [SerializeField] GameObject m_ChatPanelHolder;

        [SerializeField]
        GameObject m_MessageObject;
        List<GameObject> m_MessageObjPool = new List<GameObject>();

        IChannelTextMessage m_LatestChannelTextMessage;

        [SerializeField] List<GameObject> m_ChatTextPanel = new List<GameObject>();

        bool m_IsTextChatActive = true;
        [SerializeField] Button m_ChatTextBtnActive;
        [SerializeField] Button m_ChatTextBtnInactive;

        LocalPlayer m_LocalPlayer;

        public void Initialize(LocalPlayer localPlayer, VivoxUserHandler userHandler)
        {
            m_LocalPlayer = localPlayer;
            m_VivoxUserHandler = userHandler;
            Manager.onGameStateChanged += ToggleChatPanelHolder;
            if (m_VivoxUserHandler != null)
            {
                m_VivoxUserHandler.OnTextMessageLogReceivedEvent += OnTextMessageLogReceivedEvent;
                m_VivoxUserHandler.OnLeftChannel += ClearChatUi;

            }

            foreach (GameObject go in m_ChatTextPanel)
            {
                go.SetActive(m_IsTextChatActive);
            }
        }

        void ToggleChatPanelHolder(GameState state)
        {
            if (state == GameState.Lobby)
            {
                m_ChatPanelHolder.SetActive(true);
            }else
            {
                m_ChatPanelHolder.SetActive(false);
            }
        }

        public void ToggleTextChat()
        {
            m_IsTextChatActive = !m_IsTextChatActive;
            foreach (GameObject go in m_ChatTextPanel)
            {
                go.SetActive(m_IsTextChatActive);
            }
            m_ChatTextBtnActive.gameObject.SetActive(m_IsTextChatActive);
            m_ChatTextBtnInactive.gameObject.SetActive(!m_IsTextChatActive);
        }

        public void ClearChatUi()
        {

            if (m_MessageObjPool.Count > 0)
            {
                ClearMessageObjectPool();
            }
        }

        void ClearMessageObjectPool()
        {
            for (int i = 0; i < m_MessageObjPool.Count; i++)
            {
                Destroy(m_MessageObjPool[i].gameObject);
            }
            m_MessageObjPool.Clear();
        }

        void OnTextMessageLogReceivedEvent(string sender, IChannelTextMessage channelTextMessage)
        {
            if (!String.IsNullOrEmpty(channelTextMessage.ApplicationStanzaNamespace))
            {
                // If we find a message with an ApplicationStanzaNamespace we don't push that to the chat box.
                // Such messages denote opening/closing or requesting the open status of multiplayer matches.
                return;
            }

            if (!m_IsTextChatActive)
            {
                return;
            }

            if(m_LatestChannelTextMessage == null || !m_LatestChannelTextMessage.Equals(channelTextMessage))
            {
                GameObject newMessageSenderObj = Instantiate(m_MessageObject, m_ChatContentPanel.transform);
                TMPro.TMP_Text senderDisplayName = newMessageSenderObj.GetComponentInChildren<TMPro.TMP_Text>();
                senderDisplayName.color = Color.white;

                GameObject newMessageObj = Instantiate(m_MessageObject, m_ChatContentPanel.transform);
                TMPro.TMP_Text message = newMessageObj.GetComponentInChildren<TMPro.TMP_Text>();
                m_MessageObjPool.Add(newMessageSenderObj);
                m_MessageObjPool.Add(newMessageObj);
                message.text = channelTextMessage.Message;
                m_LatestChannelTextMessage = channelTextMessage;
                newMessageSenderObj.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

                string senderName = m_LocalPlayer.DisplayName.Value;

                if (channelTextMessage.FromSelf)
                {
                    Debug.Log("from self chat");
                    senderDisplayName.alignment = TextAlignmentOptions.Right;

                    senderDisplayName.text = ":" + senderName;

                    message.alignment = TextAlignmentOptions.Right;
                    message.gameObject.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
                    message.gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
                    message.gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
                }
                else
                {
                    foreach (InLobbyUserUI user in m_UserListHolder.GetUserLists())
                    {
                        if (user.UserId == sender)
                        {
                            senderName = user.LocalPlayer.DisplayName.Value;
                        }
                    }

                    senderDisplayName.text = senderName + ":";
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(newMessageObj.GetComponent<RectTransform>());

                LayoutRebuilder.ForceRebuildLayoutImmediate(newMessageSenderObj.GetComponent<RectTransform>());

                LayoutRebuilder.ForceRebuildLayoutImmediate(m_ChatContentPanel.GetComponent<RectTransform>());
            }
            else
            {
                return;
            }
        }
    }
}