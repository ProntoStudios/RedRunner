using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRadius : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float radius = transform.parent.GetComponent<BombExplosion>().ExplodeRadius;
        transform.localScale = new Vector3(radius*2.0f,radius*2.0f,1f); 
    }
}
