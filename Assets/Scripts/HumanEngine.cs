﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HumanEngine : MonoBehaviour
{
    [Header("General")]
    public string id = "";

    [Header("Traffic Light")]
    public GameObject trafficLight;

    [Header("Path")]
    public Transform path;
    public float wayPointDistance;
    public int pressurePlateEndNode;

    [Header("Human")]
    public float m_moveSpeed = 2;
    public Animator m_animator;
    public Rigidbody m_rigidBody;

    [Header("Sensor")]
    public float sensorLength = 10f;
    public float frontSensorDistance = 2.1f;

    [Header("Do not edit")]
    public bool delete = false;
    public int currentNode = 0;
    public float currentSpeed;

    private List<Transform> nodes;
    private bool isBraking = false;
    private Vector3 m_currentDirection;
    private float m_interpolation = 10;

    void Start()
    {
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
        m_animator.SetBool("Grounded", true);

        Sensors();
        CheckWaypointDistance();
        Move();
    }

    private void Move()
    {
        if (!isBraking)
        {
            Vector3 direction = nodes[currentNode].position - transform.position;
            direction /= direction.magnitude;

            float directionLength = direction.magnitude;
            direction.y = 0;
            direction = direction.normalized * directionLength;

            if (direction != Vector3.zero)
            {
                m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

                transform.rotation = Quaternion.LookRotation(m_currentDirection);
                transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

                m_animator.SetFloat("MoveSpeed", direction.magnitude / 2f);
            }
        } else
        {
            m_animator.SetFloat("MoveSpeed", 0.0f);
        }
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPosition = this.gameObject.transform.localPosition;
        sensorStartPosition += transform.forward * frontSensorDistance;
        sensorStartPosition.y += 1.0f;

        if (Physics.Raycast(sensorStartPosition, transform.forward, out hit, sensorLength))
        {
            HumanEngine humanInFront = hit.collider.gameObject.GetComponentInParent<HumanEngine>();
            if (humanInFront != null)
            {
                isBraking = true;
                Debug.DrawLine(sensorStartPosition, hit.point, Color.white);
            }
        }
        else
        {
            isBraking = false;
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