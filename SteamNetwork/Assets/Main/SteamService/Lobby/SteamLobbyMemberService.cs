using Steamworks;
using System;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public class SteamLobbyMemberService 
    {
        protected Callback<LobbyEnter_t> _lobbyEntered;
        public Action<ulong, CSteamID> OnJoinLobby;
        public SteamLobbyMemberService() 
        {
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
        public void JoinLobby(ulong lobbyID)
        {
            if (SteamUtility.CheckSteamInitialization() is false) return;
            CSteamID cSteamID = new(lobbyID);
            SteamMatchmaking.JoinLobby(cSteamID);
        }
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            ulong id = callback.m_ulSteamIDLobby;
            CSteamID cSteamID = new(id);

            Debug.Log($"[Steam][Member] Join Lobby: {id} ");
            OnJoinLobby?.Invoke(id, cSteamID);
        }
    }
}
