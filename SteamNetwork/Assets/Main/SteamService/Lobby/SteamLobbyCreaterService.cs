using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public class LobbyConfig
    {
        public int MaxPlayerCount { get; private set; }
        public Dictionary<string, string>LobbyDataDic { get; private set; }
        public void AddLobbyData(string key,string value) { LobbyDataDic.Add(key, value); }
        public LobbyConfig(int maxPlayerCount)
        {
            MaxPlayerCount = maxPlayerCount;
            LobbyDataDic = new();
        }
    }
    public class SteamLobbyCreaterService
    {
        public Action<ulong, CSteamID> OnLobbyCreated;
        protected Callback<LobbyCreated_t> _lobbyCreated;
        public LobbyConfig LobbyConfig { get; private set; }
        public SteamLobbyCreaterService()
        {
            _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreate);
        }
        public void CreateLobby(LobbyConfig config)
        {
            if (SteamUtility.CheckSteamInitialization() is false) return;
            LobbyConfig = config;
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, LobbyConfig.MaxPlayerCount);
            Debug.Log($"[Steam][Host] Start create lobby...");
        }
        private void OnLobbyCreate(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            { Debug.LogError("[Steam][Host] Create the lobby fail¡G" + callback.m_eResult); return;}

            ulong id = callback.m_ulSteamIDLobby;
            CSteamID cSteamID = new(id);

            foreach (var element in LobbyConfig.LobbyDataDic)
            { SteamMatchmaking.SetLobbyData(cSteamID, element.Key, element.Value); }

            Debug.Log($"[Steam][Host] Create the lobby success. \nLobby ID: {id}");
            OnLobbyCreated?.Invoke(id, cSteamID);
        }
    }

}