using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public GameObject[] spawnObjects;
    public Transform path;
    public GameObject firstTrafficLight;
    public string firstId;
    public GameObject secondTrafficLight;
    public string secondId;
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
        InvokeRepeating("SpawnHuman", startAfter, spawnTime);
    }

    private void SpawnHuman()
    {
        if (Random.Range(0, 100) < spawnChangePercentage && CountBeforeFirstTrafficLight(false) < maxWaitingTraffic)
        {
            GameObject humanObject = spawnObjects[Random.Range(0, spawnObjects.Length)];
            HumanEngine human = humanObject.GetComponent<HumanEngine>();
            human.path = path;
            human.firstId = firstId;
            human.firstTrafficLight = firstTrafficLight;
            human.secondId = secondId;
            human.secondTrafficLight = secondTrafficLight;
            human.pressurePlateEndNode = pressurePlateStartNode + 2;
            humanObject.transform.position = spawnPoint;
            humanObject.tag = "Human";
            humanObject.transform.rotation = Quaternion.identity;
            humanObject.transform.Rotate(0.0f, spawnAngle, 0.0f);
            Instantiate(humanObject);
        }
    }

    public int CountBeforeFirstTrafficLight(bool usePressurePlateStart)
    {
        int count = 0;
        int ppsn = usePressurePlateStart ? pressurePlateStartNode : 0;
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Human"))
        {
            HumanEngine engine = car.GetComponent<HumanEngine>();
            if (engine.firstId == firstId && engine.currentNode > ppsn && engine.currentNode <= pressurePlateStartNode + 2)
            {
                count++;
            }
        }
        return count;
    }

    public int CountBeforeSecondTrafficLight()
    {
        int count = 0;
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Human"))
        {
            HumanEngine engine = car.GetComponent<HumanEngine>();
            if (engine.secondId == secondId && engine.currentNode > pressurePlateStartNode + 2 && engine.currentNode <= pressurePlateStartNode + 4)
            {
                count++;
            }
        }
        return count;
    }
}
