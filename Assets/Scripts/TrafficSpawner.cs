using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    public bool isCar = true;
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
        spawnAngle = GetSpawnAngle();
        float delay = startAfter <= 0.0f ? Random.Range(0.1f, 2.0f) : startAfter;
        if (isCar)
        {
            InvokeRepeating("SpawnCar", startAfter, spawnTime);
        } else
        {
            InvokeRepeating("SpawnHuman", startAfter, spawnTime);
        }
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

    private void SpawnHuman()
    {
        if (Random.Range(0, 100) < spawnChangePercentage && CountBeforeTrafficLight(false) < maxWaitingTraffic)
        {
            GameObject humanObject = spawnObjects[Random.Range(0, spawnObjects.Length)];
            HumanEngine human = humanObject.GetComponent<HumanEngine>();
            human.path = path;
            human.id = id;
            human.trafficLight = trafficLight;
            human.pressurePlateEndNode = pressurePlateStartNode + 2;
            humanObject.transform.position = spawnPoint;
            humanObject.tag = "Human";
            humanObject.transform.rotation = Quaternion.identity;
            humanObject.transform.Rotate(0.0f, spawnAngle, 0.0f);
            Instantiate(humanObject);
        }
    }

    private float GetSpawnAngle()
    {
        Vector3? start = null, end = null;
        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != path.transform)
            {
                if (!start.HasValue)
                {
                    start = pathTransform[i].position;
                }
                else if (!end.HasValue)
                {
                    end = pathTransform[i].position;
                } else
                {
                    break;
                }
            }
        }
        if (end.HasValue)
        {
            Vector3 referenceRight = Vector3.Cross(Vector3.up, transform.forward);
            float angle = Vector3.Angle(end.Value - start.Value, transform.forward);
            float sign = Mathf.Sign(Vector3.Dot(end.Value - start.Value, referenceRight));
            return sign * angle; // -180 to 180 degrees
        } else
        {
            return 0.0f;
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
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Human"))
        {
            HumanEngine engine = car.GetComponent<HumanEngine>();
            if (engine.id == id && engine.currentNode > ppsn && engine.currentNode <= pressurePlateStartNode + 2)
            {
                count++;
            }
        }
        return count;
    }
}
