using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public class SteamLobbyManager : MonoBehaviour
    {
        protected Callback<LobbyCreated_t> _lobbyCreated;
        protected Callback<LobbyEnter_t> _lobbyEntered;

        public const int MaxPlayerCount = 4;
        public ulong JoinLobbyID;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) { CreateLobby(); }
            if (Input.GetKeyDown(KeyCode.J)) { JoinLobby(); }
        }

        public void CreateLobby()
        {
            CheckSteaminitialization();
            _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreate);
            _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, MaxPlayerCount);
        }
        public void JoinLobby() 
        {
            CSteamID lobbyID = new(JoinLobbyID);
            SteamMatchmaking.JoinLobby(lobbyID);
        }
        private void CheckSteaminitialization()
        {
            if (SteamManager.Initialized is false)
            {
                Debug.LogError("[Steam Lobby] Steam has not been initialized");
                return;
            }
        }
        private void OnLobbyCreate(LobbyCreated_t callback)
        { 
            if(callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Create the lobby fail¡G" + callback.m_eResult);
                return;
            }
            Debug.Log($"Create the lobby success. \nLobby ID: {callback.m_ulSteamIDLobby}");
            SetLobby(callback, "name", "HaHa Loby");
        }
        private void SetLobby(LobbyCreated_t callback, string phKey, string phValue)
        { 
            CSteamID lobbyID =new(callback.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyData(lobbyID, phKey, phValue);
        }
        private void OnLobbyEntered(LobbyEnter_t callback) 
        {
            CSteamID lobbyID = new(callback.m_ulSteamIDLobby);
            Debug.Log($"join Lobby: {lobbyID} ");

            int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            Debug.Log($"Currently play's count in the lobby: {memberCount} ");

            for (int i = 0; i < memberCount; i++) 
            {
                CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, i);
                string name = SteamFriends.GetFriendPersonaName(member);
                Debug.Log($"Player Name: {name}");
            }
        }
    }

}