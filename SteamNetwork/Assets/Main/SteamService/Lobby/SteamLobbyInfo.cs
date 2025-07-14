using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Lobby
{
    public struct SteamLobbyInfo
    {
        public string Name;
        public ulong lobbyID;
        public CSteamID CSteamID;
        public bool IsOrner;
    }
}
