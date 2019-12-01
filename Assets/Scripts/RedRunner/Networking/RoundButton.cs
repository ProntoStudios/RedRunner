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
            Debug.Log("read client count" + RoundsManager.Instance.playersConnected);
            if (RoundsManager.Instance.playersConnected > 1)
            {//!NetworkManager.IsServer && NetworkManager.PlayerCount > 0)
                {
                    gameObject.SetActive(false);
                }
            }
        };
    }

    public void StartGame()
    {
        RoundsManager.Instance.CmdStartGame();
        gameObject.SetActive(false);
    }
}
  
