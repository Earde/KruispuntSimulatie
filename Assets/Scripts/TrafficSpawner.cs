using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    private void Start()
    {
        InvokeRepeating("SendTraffic", 0.0f, 1.0f);
    }

    private void SendTraffic()
    {

    }
}
