using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JasonLin.SteamSDK.Network
{
    public class SteamNetwork : MonoBehaviour
    {
        private void Update()
        {
            CheckMySteamName();
        }

        private void CheckMySteamName()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (SteamManager.Initialized)
                {
                    string userName = SteamFriends.GetPersonaName();
                    Debug.LogWarning($"User Name: {userName}");
                }
                else { Debug.LogWarning("Steam has not been initialzation!"); }
            }
        }
        private void CreateLobby()
        {

        }
    }
}
