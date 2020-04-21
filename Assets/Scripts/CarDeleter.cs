using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDeleter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckForDelete", 0.0f, 1.0f);
    }

    private void CheckForDelete()
    {
        List<GameObject> carsToDelete = new List<GameObject>();
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car"))
        {
            if (car.GetComponent<CarEngine>().delete)
            {
                carsToDelete.Add(car);
                Debug.Log("Delete");
            }
        }
        foreach (GameObject car in carsToDelete)
        {
            Destroy(car);
        }
    }
}
