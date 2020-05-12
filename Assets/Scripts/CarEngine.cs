using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    [Header("General")]
    public string id = "";

    [Header("Traffic Light")]
    public GameObject trafficLight;

    [Header("Path")]
    public Transform path;
    public float wayPointDistance;
    public int pressurePlateEndNode;

    [Header("Motor")]
    public float maxSteerAngle;
    public WheelCollider wheelFL; // Front Left Wheel
    public WheelCollider wheelFR; // Front Right Wheel
    public float maxMotorBrakeTorque;
    public float maxMotorTorque;
    public float maxSpeed;
    public Vector3 centerOfMass;

    [Header("Sensor")]
    public float sensorLength = 10f;
    public float frontSensorDistance = 2.1f;

    [Header("Do not edit")]
    public bool delete = false;
    public int currentNode = 0;
    public float currentSpeed;

    private List<Transform> nodes;
    private bool isBraking = false;
    private float tempMaxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        sensorLength = sensorLength * UnityEngine.Random.Range(1.0f, 1.05f); // Zorgt ervoor dat niet al het verkeeer precies even dicht op elkaar gaat staan
        maxSpeed *= UnityEngine.Random.Range(0.95f, 1f); // Zorgt ervoor dat niet al het verkeer even snel rijdt

        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        CheckWaypointDistance();
        Sensors(ApplySteer());
        Drive();
        Brake();
        isBraking = false;
    }

    private void Sensors(float wheelAngle)
    {
        RaycastHit hit;
        Vector3 sensorStartPosition = this.gameObject.transform.localPosition;
        sensorStartPosition += transform.forward * frontSensorDistance;
        sensorStartPosition.y += 1.0f;

        if (Physics.Raycast(sensorStartPosition, Quaternion.Euler(0.0f, wheelAngle, 0.0f) * transform.forward, out hit, sensorLength))
        {
            CarEngine carInFront = hit.collider.gameObject.GetComponentInParent<CarEngine>();
            if (carInFront != null)
            {
                // Geen bumperklevers
                tempMaxSpeed = Mathf.Min(carInFront.currentSpeed, carInFront.tempMaxSpeed) * 0.75f; // Get speed of car in front
                Debug.DrawLine(sensorStartPosition, hit.point, Color.white);
            }
        } else
        {
            //Debug.DrawLine(sensorStartPosition, sensorStartPosition + Quaternion.Euler(0.0f, wheelAngle, 0.0f) * transform.forward, Color.white);
            tempMaxSpeed = maxSpeed;
        }
    }

    private float ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
        return newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < tempMaxSpeed && !isBraking)
        {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        } else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
        if (currentSpeed >= tempMaxSpeed)
        {
            isBraking = true;
        }
    }

    private void Brake()
    {
        if (isBraking)
        {
            wheelFR.brakeTorque = maxMotorBrakeTorque;
            wheelFL.brakeTorque = maxMotorBrakeTorque;
        } else
        {
            wheelFR.brakeTorque = 0;
            wheelFL.brakeTorque = 0;
        }
    }

    private void CheckWaypointDistance()
    {
        if (currentNode == pressurePlateEndNode && trafficLight.GetComponent<TrafficLight>().trafficLight.color != Color.green)
        {
            isBraking = true;
        }
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < wayPointDistance)
        {
            currentNode++;
            if (currentNode == nodes.Count)
            {
                currentNode = 0;
                delete = true;
            }
        }
    }
}
