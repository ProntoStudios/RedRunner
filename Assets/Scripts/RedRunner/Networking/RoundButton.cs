using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Networking;
public class RoundButton : MonoBehaviour
{
    public void Start()
    {
        NetworkManager.OnConnected += () =>
        {
            if (!NetworkManager.IsServer && NetworkManager.PlayerCount > 0)
            {
                gameObject.SetActive(false);
            }
        };
    }

    public void StartGame()
    {
        ServerRounds.Instance.StartGame();
        gameObject.SetActive(false);
    }
}
