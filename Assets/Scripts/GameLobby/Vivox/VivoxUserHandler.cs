using UnityEngine;
using Unity.Services.Vivox;
using VivoxUnity;
using System;

namespace LobbyRelaySample.vivox
{
    /// <summary>
    /// Listens for changes to Vivox state for one user in the lobby.
    /// Instead of going through Relay, this will listen to the Vivox service since it will already transmit state changes for all clients.
    /// </summary>
    public class VivoxUserHandler : MonoBehaviour
    {
        [SerializeField]
        private UI.LobbyUserVolumeUI m_lobbyUserVolumeUI;

        private IChannelSession m_ChannelSession;
        private string m_id;
        private string m_vivoxId;

        private const int k_volumeMin = -50, k_volumeMax = 20; // From the Vivox docs, the valid range is [-50, 50] but anything above 25 risks being painfully loud.

        public delegate void ParticipantValueChangedHandler(string username, ChannelId channel, bool value);
        public event ParticipantValueChangedHandler OnSpeechDetectedEvent;

        public delegate void ChannelTextMessageChangedHandler(string sender, IChannelTextMessage channelTextMessage);
        public event ChannelTextMessageChangedHandler OnTextMessageLogReceivedEvent;

        public delegate void ChannelJoined(ChannelId channelId);
        public event ChannelJoined OnChannelIdReady;

        public delegate void LeftChannel();
        public event LeftChannel OnLeftChannel;

        public static float NormalizedVolumeDefault
        {
            get { return (0f - k_volumeMin) / (k_volumeMax - k_volumeMin); }
        }

        public void Start()
        {
            m_lobbyUserVolumeUI.DisableVoice(true);
        }

        public void SetId(string id)
        {
            m_id = id;

            // Vivox appends additional info to the ID we provide, in order to associate it with a specific channel. We'll construct m_vivoxId to match the ID used by Vivox.
            // FUTURE: This isn't yet available. When using Auth, the Vivox ID will match this format:
            // Account account = new Account(id);
            // m_vivoxId = $"sip:.{account.Issuer}.{m_id}.{environmentId}.@{account.Domain}";
            // However, the environment ID from Auth is not exposed anywhere, and Vivox doesn't provide a way to retrieve the ID, either.
            // Instead, when needed, we'll search for the Vivox ID containing this user's Auth ID, which is a GUID so collisions are extremely unlikely.
            // In the future, remove FindVivoxId and pass the environment ID here instead.
            m_vivoxId = null;

            // SetID might be called after we've received the IChannelSession for remote players, which would mean after OnParticipantAdded. So, duplicate the VivoxID work here.
            if (m_ChannelSession != null)
            {
                foreach (var participant in m_ChannelSession.Participants)
                {
                    if (m_id == participant.Account.DisplayName)
                    {
                        m_vivoxId = participant.Key;
                        m_lobbyUserVolumeUI.IsLocalPlayer = participant.IsSelf;
                        m_lobbyUserVolumeUI.EnableVoice(true);
                        break;
                    }
                }
            }
        }

        public void OnChannelJoined(IChannelSession channelSession) // Called after a connection is established, which begins once a lobby is joined.
        {
            //Check if we are muted or not

            m_ChannelSession = channelSession;
            m_ChannelSession.Participants.AfterKeyAdded += OnParticipantAdded;
            m_ChannelSession.Participants.BeforeKeyRemoved += BeforeParticipantRemoved;
            m_ChannelSession.Participants.AfterValueUpdated += OnParticipantValueUpdated;
            m_ChannelSession.MessageLog.AfterItemAdded += OnMessageLogReceived;
            OnChannelIdReady?.Invoke(m_ChannelSession.Channel);
        }

        public void OnChannelLeft() // Called when we leave the lobby.
        {
            if (m_ChannelSession != null) // It's possible we'll attempt to leave a channel that isn't joined, if we leave the lobby while Vivox is connecting.
            {
                m_ChannelSession.Participants.AfterKeyAdded -= OnParticipantAdded;
                m_ChannelSession.Participants.BeforeKeyRemoved -= BeforeParticipantRemoved;
                m_ChannelSession.Participants.AfterValueUpdated -= OnParticipantValueUpdated;
                m_ChannelSession.MessageLog.AfterItemAdded -= OnMessageLogReceived;
                m_ChannelSession = null;
                OnLeftChannel?.Invoke();
            }
        }

        /// <summary>
        /// To be called whenever a new Participant is added to the channel, using the events from Vivox's custom dictionary.
        /// </summary>
        private void OnParticipantAdded(object sender, KeyEventArg<string> keyEventArg)
        {
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
            var participant = source[keyEventArg.Key];
            var username = participant.Account.DisplayName;

            bool isThisUser = username == m_id;
            if (isThisUser)
            {
                m_vivoxId = keyEventArg.Key; // Since we couldn't construct the Vivox ID earlier, retrieve it here.
                m_lobbyUserVolumeUI.IsLocalPlayer = participant.IsSelf;

                if (!participant.IsMutedForAll)
                    m_lobbyUserVolumeUI.EnableVoice(false); //Should check if user is muted or not.
                else
                    m_lobbyUserVolumeUI.DisableVoice(false);
            }
            else
            {
                if (!participant.LocalMute)
                    m_lobbyUserVolumeUI.EnableVoice(false); //Should check if user is muted or not.
                else
                    m_lobbyUserVolumeUI.DisableVoice(false);
            }
        }

        private void BeforeParticipantRemoved(object sender, KeyEventArg<string> keyEventArg)
        {
            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
            var participant = source[keyEventArg.Key];
            var username = participant.Account.DisplayName;

            bool isThisUser = username == m_id;
            if (isThisUser)
            {
                m_lobbyUserVolumeUI.DisableVoice(true);
            }
        }

        private void OnParticipantValueUpdated(object sender, ValueEventArg<string, IParticipant> valueEventArg)
        {
            ValidateArgs(new object[] { sender, valueEventArg });

            var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
            IParticipant participant = source[valueEventArg.Key];
            string username = participant.Account.DisplayName;
            ChannelId channel = valueEventArg.Value.ParentChannelSession.Key;
            string property = valueEventArg.PropertyName;

            if (username == m_id)
            {
                if (property == "UnavailableCaptureDevice")
                {
                    if (participant.UnavailableCaptureDevice)
                    {
                        m_lobbyUserVolumeUI.DisableVoice(false);
                        participant.SetIsMuteForAll(true, null); // Note: If you add more places where a player might be globally muted, a state machine might be required for accurate logic.
                    }
                    else
                    {
                        m_lobbyUserVolumeUI.EnableVoice(false);
                        participant.SetIsMuteForAll(false, null); // Also note: This call is asynchronous, so it's possible to exit the lobby before this completes, resulting in a Vivox error.
                    }
                }
                else if (property == "IsMutedForAll")
                {
                    if (participant.IsMutedForAll)
                    {
                        m_lobbyUserVolumeUI.DisableVoice(false);
                    }
                    else
                    {
                        m_lobbyUserVolumeUI.EnableVoice(false);
                    }
                }
                else
                {
                    switch (property)
                    {
                        case "SpeechDetected":
                        {
                            Debug.Log($"OnSpeechDetectedEvent: {username} in {channel}.");
                            OnSpeechDetectedEvent?.Invoke(username, channel, valueEventArg.Value.SpeechDetected);
                            break;
                        }
                        default:
                            break;
                    }
                }
            }
        }

        public void OnVolumeSlide(float volumeNormalized)
        {
            if (m_ChannelSession == null || m_vivoxId == null) // Verify initialization, since SetId and OnChannelJoined are called at different times for local vs. remote clients.
                return;

            int vol = (int)Mathf.Clamp(k_volumeMin + (k_volumeMax - k_volumeMin) * volumeNormalized, k_volumeMin, k_volumeMax); // Clamping as a precaution; if UserVolume somehow got above 1, listeners could be harmed.
            bool isSelf = m_ChannelSession.Participants[m_vivoxId].IsSelf;
            if (isSelf)
            {
                VivoxService.Instance.Client.AudioInputDevices.VolumeAdjustment = vol;
            }
            else
            {
                m_ChannelSession.Participants[m_vivoxId].LocalVolumeAdjustment = vol;
            }
        }

        static void ValidateArgs(object[] objs)
        {
            foreach (object obj in objs)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException(obj.GetType().ToString(), "Specify a non-null/non-empty argument.");
                }
            }
        }

        public void OnMuteToggle(bool isMuted)
        {
            if (m_ChannelSession == null || m_vivoxId == null)
                return;

            bool isSelf = m_ChannelSession.Participants[m_vivoxId].IsSelf;
            if (isSelf)
            {
                VivoxService.Instance.Client.AudioInputDevices.Muted = isMuted;
            }
            else
            {
                m_ChannelSession.Participants[m_vivoxId].LocalMute = isMuted;
            }
        }

        void OnMessageLogReceived(object sender, QueueItemAddedEventArgs<IChannelTextMessage> textMessage)
        {
            ValidateArgs(new object[] { sender, textMessage });

            IChannelTextMessage channelTextMessage = textMessage.Value;
            VivoxLog(channelTextMessage.Message);
            OnTextMessageLogReceivedEvent?.Invoke(channelTextMessage.Sender.DisplayName, channelTextMessage);
        }

        void VivoxLog(string msg)
        {
            Debug.Log("<color=green>VivoxVoice: </color>: " + msg);
        }

        public void SendTextMessage(string messageToSend, ChannelId channel, string applicationStanzaNamespace = null, string applicationStanzaBody = null)
        {
            if (ChannelId.IsNullOrEmpty(channel))
            {
                throw new ArgumentException("Must provide a valid ChannelId");
            }
            if (string.IsNullOrEmpty(messageToSend))
            {
                throw new ArgumentException("Must provide a message to send");
            }

            if (m_ChannelSession == null)
            {
                Debug.LogError("Channel session is null and unable to send text.");
                return;
            }

            m_ChannelSession.BeginSendText(null, messageToSend, applicationStanzaNamespace, applicationStanzaBody, ar =>
            {
                try
                {
                    m_ChannelSession.EndSendText(ar);
                }
                catch (Exception e)
                {
                    VivoxLog($"SendTextMessage failed with exception {e.Message}");
                }
            });
        }
    }
}
