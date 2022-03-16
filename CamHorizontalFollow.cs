using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamHorizontalFollow : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0, target.localRotation.eulerAngles.y, 0);
    }
}
