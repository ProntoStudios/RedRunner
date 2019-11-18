using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Networking;
using RedRunner.Characters;

public class RoundButton : MonoBehaviour
{
    public void Start()
    {
        NetworkManager.OnConnected += () =>
        {
            if (false) {//!NetworkManager.IsServer && NetworkManager.PlayerCount > 0)
            {
                gameObject.SetActive(false);
            }
        };
    }

    public void StartGame()
    {
        RedCharacter.Local.LeaderManager.CmdS
        gameObject.SetActive(false);
    }
}
