using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Networking;
using RedRunner.Characters;

public class RoundButton : MonoBehaviour
{

    public void StartGame()
    {
        RoundsManager.Instance.CmdStartGame();
        gameObject.SetActive(false);
    }
}
  
