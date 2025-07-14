using Steamworks;
using System;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public class SteamLobbyMemberService 
    {
        protected Callback<LobbyEnter_t> _lobbyEntered;
        public event Action<SteamLobbyInfo> OnJoinLobby;
        public event Action<ulong> OnLeaveLobby;
        public SteamLobbyMemberService() 
        {
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
        public void JoinLobby(ulong lobbyID)
        {
            if (SteamLobbyUtility.CheckSteamInitialization() is false) return;
            CSteamID cSteamID = new(lobbyID);
            SteamMatchmaking.JoinLobby(cSteamID);
        }
        public void LeaveLobby(ulong lobbyID)
        {
            if (SteamLobbyUtility.CheckSteamInitialization() is false) return;
            CSteamID cSteamID = new(lobbyID);
            SteamMatchmaking.LeaveLobby(cSteamID);
            OnLeaveLobby?.Invoke(lobbyID);
        }
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            ulong id = callback.m_ulSteamIDLobby;
            CSteamID cSteamID = new(id);

            Debug.Log($"[Steam][Member] Join Lobby: {id} ");
            SteamLobbyInfo lobbyInfo = new();
            lobbyInfo.lobbyID = id;
            lobbyInfo.CSteamID = cSteamID;
            lobbyInfo.Name = SteamMatchmaking.GetLobbyData(cSteamID, "name");
            lobbyInfo.IsOrner = SteamLobbyUtility.IsLobbyOwner(id);
            OnJoinLobby?.Invoke(lobbyInfo);
        }
    }
}
