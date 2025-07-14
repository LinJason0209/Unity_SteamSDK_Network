using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public static class SteamLobbyUtility
    {
        public static event Action<ulong> OnDisbandLobby;
        public static bool CheckSteamInitialization()
        {
            if (SteamManager.Initialized is false)
            { Debug.LogError("[Steam][Utility] Steam has not been initialized"); return false; }
            else { return true; }
        }
        public static int GetLobbyMemberCount(ulong lobbyID) 
        {
            CSteamID cSteamID = new(lobbyID);
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(cSteamID);
            Debug.Log($"[Steam][Utility] Currently play's count in the lobby: {memberCount} ");
            return memberCount;
        }
        public static Dictionary<CSteamID, string> GetLobbyMemberList(ulong lobbyID)
        {
            CSteamID cSteamID = new(lobbyID);
            Dictionary<CSteamID, string> resultList = new();
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(cSteamID);
            for (int i = 0; i < memberCount; i++)
            {
                CSteamID member = SteamMatchmaking.GetLobbyMemberByIndex(cSteamID, i);
                string name = SteamFriends.GetFriendPersonaName(member);
                Debug.Log($"[Steam][Lobby][Utility] Player Name: {name}");
                resultList.Add(member, name);
            }
            return resultList;
        }
        public static CSteamID GetLobbyOwner(ulong lobbyID) 
        {
            CSteamID cSteamID = new(lobbyID);
            CSteamID hostID = SteamMatchmaking.GetLobbyOwner(cSteamID);
            return hostID;
        }
        public static bool IsLobbyOwner(ulong lobbyID)
        {
            CSteamID cSteamID = new(lobbyID);
            return SteamMatchmaking.GetLobbyOwner(cSteamID) == SteamUser.GetSteamID();
        }
        public static void DisbandLobby(ulong lobbyID)
        {
            CSteamID cSteamID = new(lobbyID);
            if (IsLobbyOwner(lobbyID) is true)
            {
                SteamMatchmaking.SetLobbyData(cSteamID, "closed", "true");
                Debug.LogWarning($"[Steam][Lobby][Utility] Lobby is closed!");
                OnDisbandLobby?.Invoke(lobbyID);
            }
        }
    }
}
