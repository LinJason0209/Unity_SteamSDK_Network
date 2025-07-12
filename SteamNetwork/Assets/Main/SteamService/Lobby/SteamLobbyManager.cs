using JasonLin.SteamSDK.User;
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
        private void Start()
        {
            UserInfo = new();
            _createrService = new();
            _defaultLobbyConfig = new(3);
            _defaultLobbyConfig.AddLobbyData("name", "Jason's Home");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) { CreateLobby(); }
            if (Input.GetKeyDown(KeyCode.J)) { JoinLobby(JoinLobbyID); }
        }

        public void CreateLobby()
        { _createrService.CreateLobby(_defaultLobbyConfig); }

        public void JoinLobby(ulong lobbyID) 
        { _memberService.JoinLobby(lobbyID); }
    }

}