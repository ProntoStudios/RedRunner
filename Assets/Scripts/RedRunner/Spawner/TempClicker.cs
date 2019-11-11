using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedRunner.Networking;
public class TempClicker : MonoBehaviour
{
    public void startSelection()
    {
        ChooserManager.Local.InitiateChoosing();
    }
    public void sendSelection()
    {
        ChooserManager.Local.TrySubmitChoice(0, 2);
    }
}
