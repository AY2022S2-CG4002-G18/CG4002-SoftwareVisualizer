using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrenadeNew : MonoBehaviour
{
    public GameObject explosion;
    public GameObject self;
    public float existTimeAfterExplosion;
    public float explodeDistance;
    bool beginMove = false;
    Vector3 finalPos;

    private void Update()
    {
        if (beginMove)
        {
            if (Vector3.Distance(transform.position, finalPos) <= explodeDistance)
            {
                beginMove = false;
                Explode();
            }
        }
    }

    public void BeginMovement(Vector3[] path)
    {
        beginMove = true;
        finalPos = path[path.Length - 1];
        transform.DOPath(path, 2f);
    }

    void Explode()
    {
        self.SetActive(false);
        explosion.SetActive(true);
        Destroy(this.gameObject, existTimeAfterExplosion);
    }
}
