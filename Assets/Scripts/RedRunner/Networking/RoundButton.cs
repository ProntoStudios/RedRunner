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
            if (!NetworkManager.IsServer)
            {
                gameObject.SetActive(false);
            }
        };
    }

    public void StartGame()
    {
        if (NetworkManager.IsServer)
        {
            ServerRounds.Instance.StartGame();
        }
        gameObject.SetActive(false);
    }
}
