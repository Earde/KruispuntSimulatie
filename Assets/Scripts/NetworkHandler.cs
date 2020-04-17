using NativeWebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    WebSocket webSocket;

    string[] laneNames;
    Dictionary<string, List<GameObject>> trafficObjects = new Dictionary<string, List<GameObject>>();

    // Start is called before the first frame update
    async void Start()
    {
        // Traffic light names
        laneNames = new string[] { 
            "A1", "A2", "A3", "A4", "AB1", "AB2",
            "B1", "B2", "B3", "B4", "B5", "BB1", 
            "C1", "C2", "C3", 
            "D1", "D2", "D3", 
            "E1", "EV1", "EV2", "EV3", "EV4", 
            "FF1", "FF2", "FV1", "FV2", "FV3", "FV4", 
            "GF1", "GF2", "GV1", "GV2", "GV3", "GV4" };

        // Create <lightname, gameobjects> lookup table
        GameObject[] trafficLightObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < trafficLightObjects.Length; i++)
        {
            foreach (string name in laneNames)
            {
                if (trafficLightObjects[i].name.Contains("TrafficLight_" + name))
                {
                    if (trafficObjects.ContainsKey(name))
                    {
                        trafficObjects[name].Add(trafficLightObjects[i]);
                    } else
                    {
                        trafficObjects.Add(name, new List<GameObject>() { trafficLightObjects[i] });
                    }
                }
            }
        }

        // Set server callbacks
        webSocket = new WebSocket("ws://localhost:8000");

        webSocket.OnOpen += () =>
        {
            Debug.Log("Opened");
        };

        webSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Debug.Log("Closed");
        };

        webSocket.OnMessage += (e) =>
        {
            UpdateTrafficLights(System.Text.Encoding.UTF8.GetString(e));
        };

        while (true)
        {
            await webSocket.Connect();
        }
    }

    void UpdateTrafficLights(string message)
    {
        try
        {
            Dictionary<string, int> lightStatus = JsonConvert.DeserializeObject<Dictionary<string, int>>(message);
            foreach (string key in lightStatus.Keys)
            {
                if (!trafficObjects.ContainsKey(key)) { continue; }
                foreach (GameObject go in trafficObjects[key])
                {
                    switch (lightStatus[key])
                    {
                        case 0:
                            go.GetComponent<TrafficLight>().SetRed();
                            break;
                        case 1:
                            go.GetComponent<TrafficLight>().SetOrange();
                            break;
                        case 2:
                            go.GetComponent<TrafficLight>().SetGreen();
                            break;
                    }
                }
            }
        }
        catch
        {
            Debug.Log("Error parsing json: " + message);
        }
    }

    async void SendTraffic(Dictionary<string, int> lanes)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            foreach (string key in laneNames)
            {
                lanes[key] = 0;
            }
            string json = JsonConvert.SerializeObject(lanes);
            //Debug.Log(json);
            await webSocket.SendText(json);
        }
    }

    private async void OnApplicationQuit()
    {
        if (webSocket != null)
        {
            await webSocket.Close();
        }
    }
}
