using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public static class SteamUtility
    {
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
                Debug.Log($"[Steam][Utility] Player Name: {name}");
                resultList.Add(member, name);
            }
            return resultList;
        }
    }
}
