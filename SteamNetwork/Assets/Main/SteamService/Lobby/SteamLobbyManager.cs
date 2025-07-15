using JasonLin.SteamSDK.User;
using Steamworks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

        private void Start()
        {
            SteamLobbyUtility.OnDisbandLobby += (lobbyID) => LeaveLobby(lobbyID);
            UserInfo = new();

            InitiCreaterService();
            InitiMemberService();

            _defaultLobbyConfig = new(3);
            _defaultLobbyConfig.AddLobbyData("name", "Jason's Home");

            _lobbyChatMsgCallback = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMessageReceived);
        }
        private void InitiCreaterService()
        {
            _createrService = new();
            _createrService.OnLobbyCreated += (lobbyInfo) =>
            { UserInfo.SteamLobbyInfoDic.Add(lobbyInfo.lobbyID, lobbyInfo); };
        }
        private void InitiMemberService()
        {
            _memberService = new();
            _memberService.OnJoinLobby += (lobbyInfo) =>
            {
                if (UserInfo.SteamLobbyInfoDic.ContainsKey(lobbyInfo.lobbyID) is false)
                { UserInfo.SteamLobbyInfoDic.Add(lobbyInfo.lobbyID, lobbyInfo); }

                SendLobbyChatMessage(lobbyInfo.lobbyID, "Hello from " + SteamFriends.GetPersonaName());

            };

            _memberService.OnLeaveLobby += (lobbyID) =>
            {
                Debug.Log("Leave lobbyID=" + lobbyID);
                if (UserInfo.SteamLobbyInfoDic.ContainsKey(lobbyID) is true)
                { UserInfo.SteamLobbyInfoDic.Remove(lobbyID); }
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) { CreateLobby(); }
            if (Input.GetKeyDown(KeyCode.J)) { JoinLobby(JoinLobbyID); }
            if (Input.GetKeyDown(KeyCode.E)) { LeaveLobby(JoinLobbyID); }
        }

        public void CreateLobby()
        { _createrService.CreateLobby(_defaultLobbyConfig); }

        public void JoinLobby(ulong lobbyID) 
        { _memberService.JoinLobby(lobbyID); }

        public void LeaveLobby(ulong lobbyID)
        { 
            _memberService.LeaveLobby(lobbyID);
            if (SteamLobbyUtility.IsLobbyOwner(lobbyID) is true)
            { SteamLobbyUtility.DisbandLobby(lobbyID); }
        }

        public void SendLobbyChatMessage(ulong lobbyID, string message)
        {
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            var steamID = new CSteamID(lobbyID);
            SteamMatchmaking.SendLobbyChatMsg(steamID, messageBytes, messageBytes.Length);
        }

        private void OnLobbyChatMessageReceived(LobbyChatMsg_t callback)
        {
            CSteamID lobbyID = (CSteamID)callback.m_ulSteamIDLobby;
            CSteamID userID = (CSteamID)callback.m_ulSteamIDUser;

            byte[] data = new byte[4096]; // Max message size
            EChatEntryType chatEntryType;
            CSteamID sender;
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

    }

}