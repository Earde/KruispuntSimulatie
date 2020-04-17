using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform objectPosition;
    public Vector3 offset;

    void Update()
    {
        transform.position = objectPosition.position + offset;
        transform.LookAt(objectPosition);
    }
}
