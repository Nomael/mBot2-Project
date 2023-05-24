using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 1.5f;

    public Vector3 offset;

    void Update()
    {

        Vector3 preferredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, preferredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
