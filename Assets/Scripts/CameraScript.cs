﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform objectPosition;
    public Vector3 offset = Vector3.zero;
    public float speed = 2.0f;

    void Start()
    {
        transform.position = objectPosition.position + offset;
    }

    void Update()
    {
        transform.LookAt(objectPosition);
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
}
