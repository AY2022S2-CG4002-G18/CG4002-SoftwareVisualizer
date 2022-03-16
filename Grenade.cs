using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float angle;
    public float force;
    public float explodeHeight;
    public Rigidbody body;
    public GameObject explosion;
    public GameObject self;
    public float existTimeAfterExplosion;

    void Start()
    {
        Throw();
    }

    private void Update()
    {
        if (body.position.y < explodeHeight)
        {
            self.SetActive(false);
            body.isKinematic = true;
            explosion.SetActive(true);

            Destroy(this.gameObject, existTimeAfterExplosion);
        }
    }

    void Throw()
    {
        Vector3 forceVector = force * new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
        body.AddRelativeForce(forceVector, ForceMode.Impulse);
    }

}
