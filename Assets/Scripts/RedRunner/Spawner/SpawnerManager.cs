using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField]
    BitBenderGames.TouchInputController inputController;
    [SerializeField]
    BitBenderGames.MobileTouchCamera mobileTouchCamera;

    void EnableScrolling()
    {
        inputController.gameObject.SetActive(true);
        mobileTouchCamera.gameObject.SetActive(true);
    }

    void DisableScrolling()
    {
        mobileTouchCamera.gameObject.SetActive(false);
        inputController.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
