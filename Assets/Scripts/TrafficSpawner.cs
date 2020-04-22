using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    public GameObject[] carObjects;
    public Transform path;
    public GameObject trafficLight;
    public string id;
    public int maxWaitingTraffic = 5;
    public int spawnChangePercentage = 10;
    public float spawnTime = 3.0f;
    public int pressurePlateStartNode = 1;
    private Vector3 spawnPoint;

    private void Start()
    {
        spawnPoint = path.GetComponentsInChildren<Transform>().First(pt => pt != path.transform).position; // Get first point of path

        InvokeRepeating("SpawnCar", Random.Range(0.0f, 2.0f), spawnTime);
    }

    private void SpawnCar()
    {
        if (Random.Range(0, 100) < spawnChangePercentage && CarCountOnPressurePlate() < maxWaitingTraffic)
        {
            GameObject carObject = carObjects[Random.Range(0, carObjects.Length)];
            CarEngine car = carObject.GetComponent<CarEngine>();
            car.path = path;
            car.id = id;
            car.trafficLight = trafficLight;
            car.pressurePlateEndNode = pressurePlateStartNode + 2;
            carObject.transform.position = spawnPoint;
            carObject.tag = "Car";
            Instantiate(carObject);
        }
    }

    public int CarCountOnPressurePlate()
    {
        int count = 0;
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car"))
        {
            CarEngine engine = car.GetComponent<CarEngine>();
            if (engine.id == id && engine.currentNode > pressurePlateStartNode && engine.currentNode <= pressurePlateStartNode + 2)
            {
                count++;
            }
        }
        return count;
    }
}
