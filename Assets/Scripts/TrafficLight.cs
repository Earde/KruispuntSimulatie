using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public Light trafficLight;

    // Start is called before the first frame update
    void Start()
    {
        trafficLight.enabled = true;
        SetRed();
    }

    public void SetRed()
    {
        trafficLight.color = Color.red;
        trafficLight.transform.localPosition = new Vector3(0f, 1f, -1.5f);
    }

    public void SetOrange()
    {
        trafficLight.color = Color.yellow;
        trafficLight.transform.localPosition = new Vector3(0f, 0f, -1.5f);
    }

    public void SetGreen()
    {
        trafficLight.color = Color.green;
        trafficLight.transform.localPosition = new Vector3(0f, -1f, -1.5f);
    }
}
