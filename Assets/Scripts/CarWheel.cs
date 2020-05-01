using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour
{
    public WheelCollider targetWheel;
    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();
    private Vector3 initWheelRotation;

    private void Start()
    {
        initWheelRotation = targetWheel.transform.localRotation.eulerAngles;
        //Debug.Log(initWheelRotation);
    }

    void Update()
    {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
        transform.Rotate(initWheelRotation);
    }
}
