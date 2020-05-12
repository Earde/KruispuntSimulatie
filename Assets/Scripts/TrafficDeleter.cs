using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficDeleter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckForDelete", 0.0f, 1.0f);
    }

    private void CheckForDelete()
    {
        List<GameObject> objectsToDelete = new List<GameObject>();
        foreach (GameObject car in GameObject.FindGameObjectsWithTag("Car"))
        {
            if (car.GetComponent<CarEngine>().delete)
            {
                objectsToDelete.Add(car);
            }
        }
        foreach (GameObject human in GameObject.FindGameObjectsWithTag("Human"))
        {
            if (human.GetComponent<HumanEngine>().delete)
            {
                objectsToDelete.Add(human);
            }
        }
        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
    }
}
