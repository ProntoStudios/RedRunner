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
            if (!NetworkManager.IsServer && NetworkManager.ClientCount > 1)
            {
                gameObject.SetActive(false);
            }
        };
    }
    public void StartRound()
    {
        ServerRounds.Instance.ResetRound();
        gameObject.SetActive(false);
    }
}
