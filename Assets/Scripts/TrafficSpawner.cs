using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficSpawner : MonoBehaviour
{
    public GameObject carObject;
    public Transform path;

    private Vector3 spawnPoint;

    private void Start()
    {
        spawnPoint = path.GetComponentsInChildren<Transform>().First(pt => pt != path.transform).position;

        InvokeRepeating("SpawnCar", 0.0f, 3.0f);
    }

    private void SpawnCar()
    {
        CarEngine car = carObject.GetComponent<CarEngine>();
        car.path = path;
        carObject.transform.position = spawnPoint;
        carObject.tag = "Car";
        Instantiate(carObject);
    }
}
