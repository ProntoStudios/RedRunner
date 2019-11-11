﻿using System.Collections;
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
    public void StartRound()
    {
        if (NetworkManager.IsServer)
        {
            RoundsManager.Local.ResetRound();
        }
        gameObject.SetActive(false);
    }
}