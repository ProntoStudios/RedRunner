using RedRunner.Networking;
using RedRunner.TerrainGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    [SerializeField]
    protected float m_ExplodeRadius = 10.0f;

    // boom goes the dynamite
    public void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, m_ExplodeRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject.GetComponent(typeof(Deletable)) as Deletable != null)
            {
                Transform curr = collider.gameObject.transform;
                while(curr.gameObject.GetComponent<ServerSpawnable>() == null)
                {
                    curr = curr.parent;
                }
                Destroy(curr.gameObject);
            }
        }

    }
}
