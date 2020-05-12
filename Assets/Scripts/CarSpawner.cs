using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] spawnObjects;
    public Transform path;
    public GameObject trafficLight;
    public string id;
    public int maxWaitingTraffic = 5;
    public int spawnChangePercentage = 10;
    public float spawnTime = 3.0f;
    public int pressurePlateStartNode = 1;
    public float startAfter = 0.0f;
    private Vector3 spawnPoint;
    private float spawnAngle;

    private void Start()
    {
        spawnPoint = path.GetComponentsInChildren<Transform>().First(pt => pt != path.transform).position; // Get first point of path
        spawnAngle = SpawnHelper.GetSpawnAngle(path, transform);
        float delay = startAfter <= 0.0f ? Random.Range(0.1f, 2.0f) : startAfter;
        InvokeRepeating("SpawnCar", startAfter, spawnTime);
    }

    private void SpawnCar()
    {
        if (Random.Range(0, 100) < spawnChangePercentage && CountBeforeTrafficLight(false) < maxWaitingTraffic)
        {
            GameObject carObject = spawnObjects[Random.Range(0, spawnObjects.Length)];
            CarEngine car = carObject.GetComponent<CarEngine>();
            car.path = path;
            car.id = id;
            car.trafficLight = trafficLight;
            car.pressurePlateEndNode = pressurePlateStartNode + 2;
            carObject.transform.position = spawnPoint;
            carObject.tag = "Car";
            carObject.transform.rotation = Quaternion.identity;
            carObject.transform.Rotate(0.0f, spawnAngle, 0.0f);
            Instantiate(carObject);
        }
    }

    public int CountBeforeTrafficLight(bool usePressurePlateStart)
    {
        int count = 0;
        int ppsn = usePressurePlateStart ? pressurePlateStartNode : 0;
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car"))
        {
            CarEngine engine = car.GetComponent<CarEngine>();
            if (engine.id == id && engine.currentNode > ppsn && engine.currentNode <= pressurePlateStartNode + 2)
            {
                count++;
            }
        }
        return count;
    }
}
