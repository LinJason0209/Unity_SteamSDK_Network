using JasonLin.SteamSDK.User;
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
        private void Start()
        {
            SteamLobbyUtility.OnDisbandLobby += (lobbyID) => LeaveLobby(lobbyID);
            UserInfo = new();

            InitiCreaterService();
            InitiMemberService();

            _defaultLobbyConfig = new(3);
            _defaultLobbyConfig.AddLobbyData("name", "Jason's Home");
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
            };

            _memberService.OnLeaveLobby += (lobbyID) =>
            {
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
    }

}