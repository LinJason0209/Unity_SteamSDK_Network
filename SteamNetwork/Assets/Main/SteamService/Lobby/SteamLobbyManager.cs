using JasonLin.SteamSDK.User;
using Steamworks;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public class SteamLobbyManager : MonoBehaviour
    {
        public ulong JoinLobbyID;
        public SteamUserInfo UserInfo;

        private SteamLobbyCreaterService _createrService;
        private SteamLobbyMemberService _memberService;
        private LobbyConfig _defaultLobbyConfig;

        private Callback<LobbyChatMsg_t> _lobbyChatMsgCallback;
        private Callback<P2PSessionRequest_t> _p2pSessionRequestCallback;
        private Callback<P2PSessionConnectFail_t> _p2pSessionFailCallback;

        private void Start()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam not initialized.");
                return;
            }

            SteamNetworking.AllowP2PPacketRelay(true); // relay = NAT fallback

            SteamLobbyUtility.OnDisbandLobby += (lobbyID) => LeaveLobby(lobbyID);
            UserInfo = new();

            InitiCreaterService();
            InitiMemberService();

            _defaultLobbyConfig = new(3);
            _defaultLobbyConfig.AddLobbyData("name", "Jason's Home");

            _lobbyChatMsgCallback = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessageReceived);
            _p2pSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnSessionRequest);
            _p2pSessionFailCallback = Callback<P2PSessionConnectFail_t>.Create(OnSessionConnectFail);
        }

        private void InitiCreaterService()
        {
            _createrService = new();
            _createrService.OnLobbyCreated += (lobbyInfo) =>
            {
                UserInfo.SteamLobbyInfoDic.Add(lobbyInfo.lobbyID, lobbyInfo);
            };
        }

        private void InitiMemberService()
        {
            _memberService = new();
            _memberService.OnJoinLobby += (lobbyInfo) =>
            {
                if (!UserInfo.SteamLobbyInfoDic.ContainsKey(lobbyInfo.lobbyID))
                    UserInfo.SteamLobbyInfoDic.Add(lobbyInfo.lobbyID, lobbyInfo);

                SendLobbyChatMessage(lobbyInfo.lobbyID, "Hello from " + SteamFriends.GetPersonaName());
            };

            _memberService.OnLeaveLobby += (lobbyID) =>
            {
                Debug.Log("Leave lobbyID=" + lobbyID);
                if (UserInfo.SteamLobbyInfoDic.ContainsKey(lobbyID))
                    UserInfo.SteamLobbyInfoDic.Remove(lobbyID);
            };
        }

        private void Update()
        {
            if (!SteamManager.Initialized) return;

            // Input actions
            if (Input.GetKeyDown(KeyCode.C)) { CreateLobby(); }
            if (Input.GetKeyDown(KeyCode.J)) { JoinLobby(JoinLobbyID); }
            if (Input.GetKeyDown(KeyCode.E)) { LeaveLobby(JoinLobbyID); }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var target = SteamLobbyUtility.GetLobbyOwner(JoinLobbyID);
                Debug.Log("Sending P2P to: " + target.m_SteamID);
                SendP2PMessage(target, "Test P2P Packet");
            }

            // P2P receiving
            uint msgSize;
            while (SteamNetworking.IsP2PPacketAvailable(out msgSize))
            {
                byte[] buffer = new byte[msgSize];
                CSteamID sender;
                if (SteamNetworking.ReadP2PPacket(buffer, msgSize, out uint bytesRead, out sender))
                {
                    SteamNetworking.AcceptP2PSessionWithUser(sender); // VERY IMPORTANT
                    string msg = System.Text.Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
                    Debug.Log($"[P2P] From {sender.m_SteamID}: {msg}");
                }
            }
        }

        public void CreateLobby()
        {
            _createrService.CreateLobby(_defaultLobbyConfig);
        }

        public void JoinLobby(ulong lobbyID)
        {
            _memberService.JoinLobby(lobbyID);
        }

        public void LeaveLobby(ulong lobbyID)
        {
            _memberService.LeaveLobby(lobbyID);
            if (SteamLobbyUtility.IsLobbyOwner(lobbyID))
            {
                SteamLobbyUtility.DisbandLobby(lobbyID);
            }
        }

        public void SendLobbyChatMessage(ulong lobbyID, string message)
        {
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            var steamID = new CSteamID(lobbyID);
            SteamMatchmaking.SendLobbyChatMsg(steamID, messageBytes, messageBytes.Length);
        }

        public void SendP2PMessage(CSteamID targetUser, string message)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            bool success = SteamNetworking.SendP2PPacket(targetUser, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
            if (!success)
                Debug.LogWarning("P2P packet failed to send.");
        }

        private void OnLobbyChatMessageReceived(LobbyChatMsg_t callback)
        {
            CSteamID lobbyID = (CSteamID)callback.m_ulSteamIDLobby;
            byte[] data = new byte[4096];
            CSteamID sender;
            EChatEntryType chatEntryType;

            int chatID = unchecked((int)callback.m_iChatID);
            int bytesRead = SteamMatchmaking.GetLobbyChatEntry(
                lobbyID,
                chatID,
                out sender,
                data,
                data.Length,
                out chatEntryType
            );

            if (chatEntryType == EChatEntryType.k_EChatEntryTypeChatMsg)
            {
                string message = System.Text.Encoding.UTF8.GetString(data, 0, bytesRead);
                Debug.Log($"[Lobby Chat] {SteamFriends.GetFriendPersonaName(sender)}: {message}");
            }
        }

        private void OnSessionRequest(P2PSessionRequest_t request)
        {
            Debug.Log($"Incoming P2P session from {request.m_steamIDRemote}");
            SteamNetworking.AcceptP2PSessionWithUser(request.m_steamIDRemote);
        }

        private void OnSessionConnectFail(P2PSessionConnectFail_t fail)
        {
            Debug.LogWarning($"P2P connection failed from {fail.m_steamIDRemote}, reason: {fail.m_eP2PSessionError}");
        }
    }
}
