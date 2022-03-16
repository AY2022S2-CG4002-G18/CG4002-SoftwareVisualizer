using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeControl : MonoBehaviour
{
    public Transform target;
    public GameObject grenadeObject;
    private float height;
    public int resolution;
    private Vector3[] path;


    void Update()
    {
        if (!GamePlay.enemyInSight) return;

        Vector3 startPoint = transform.position;
        Vector3 endPoint = target.position;

        height = Vector3.Distance(startPoint, endPoint) / 2f;

        Vector3 bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * height);

        path = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float t = (i + 1) / (float)resolution;
            path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);
        }

    }

    Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }

    public void ThrowGrenade()
    {
        GameObject gameObject = Instantiate(grenadeObject);
        gameObject.GetComponent<GrenadeNew>().BeginMovement(path);
    }
}
