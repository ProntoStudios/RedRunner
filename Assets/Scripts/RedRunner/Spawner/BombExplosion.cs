using RedRunner.TerrainGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [SerializeField]
    protected float m_ExplodeRadius = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // boom goes the dynamite
    public void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, m_ExplodeRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {

            if (hitColliders[i].gameObject.GetComponent(typeof(Deletable)) as Deletable != null)
            {
                Destroy(hitColliders[i].gameObject);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
