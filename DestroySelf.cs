using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float time;

    private void Start()
    {
        Invoke("DestroyObject", time);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
