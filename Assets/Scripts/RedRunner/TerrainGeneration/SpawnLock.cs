using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLock : MonoBehaviour
{
    [SerializeField]
    private Animator animator = default;
    [SerializeField]
    private EdgeCollider2D edgeCollider = default;
    [SerializeField]
    private InvertedCircleCollider circleCollider = default;
    [SerializeField]
    public GameEvent destroySpawnLock = default;

    void Start()
    {
        destroySpawnLock.RegisterAction(Finish);
    }

    public void Unlock()
    {
        edgeCollider.enabled = false;
        animator.SetTrigger("Unlock");
    }

    public void Finish()
    {
        Destroy(gameObject);
    }

    public void SetWidth(float width)
    {
        float scale = width / 2.56f;
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
