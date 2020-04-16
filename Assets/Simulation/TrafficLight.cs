using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public Light light;

    // Start is called before the first frame update
    void Start()
    {
        light.enabled = true;
        SetRed();
    }

    public void SetRed()
    {
        light.color = Color.red;
        light.transform.localPosition = new Vector3(0f, 1f, -0.75f);
    }

    public void SetOrange()
    {
        light.color = Color.yellow;
        light.transform.localPosition = new Vector3(0f, 0f, -0.75f);
    }

    public void SetGreen()
    {
        light.color = Color.green;
        light.transform.localPosition = new Vector3(0f, -1f, -0.75f);
    }
}
