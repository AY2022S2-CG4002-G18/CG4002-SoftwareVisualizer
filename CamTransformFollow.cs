using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTransformFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;

    private void Update()
    {
        transform.position = target.position + Quaternion.FromToRotation(Vector3.forward, transform.forward) * offset;

    }
}
