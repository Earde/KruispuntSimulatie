using NativeWebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    public bool useLog = false;

    WebSocket webSocket;

    string[] laneNames;
    Dictionary<string, List<GameObject>> trafficObjects = new Dictionary<string, List<GameObject>>(); // <Name, List<Lights>> ex. dict["A1"] == (A1.0 + A1.1) GameObjects

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

        // Create <trafficLightName, gameobjects> lookup table
        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            foreach (string name in laneNames)
            {
                if (gameObjects[i].name.Contains("TrafficLight_" + name))
                {
                    if (trafficObjects.ContainsKey(name))
                    {
                        trafficObjects[name].Add(gameObjects[i]);
                    } else
                    {
                        trafficObjects.Add(name, new List<GameObject>() { gameObjects[i] });
                    }
                }
            }
        }

        // Set server callbacks
        webSocket = new WebSocket("ws://localhost:8000");

        webSocket.OnOpen += () =>
        {
            Log("Opened");
        };

        webSocket.OnError += (e) =>
        {
            Log("Error! " + e);
        };

        webSocket.OnClose += (e) =>
        {
            Log("Closed");
        };

        webSocket.OnMessage += (e) =>
        {
            UpdateTrafficLights(System.Text.Encoding.UTF8.GetString(e));
        };

        InvokeRepeating("SendTraffic", 2.0f, 1.0f); // After 2 seconds keep sending traffic quantities every 1 second

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
        finally
        {
            //Debug.Log("Received: " + message);
        }
    }

    async void SendTraffic()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            Dictionary<string, int> lanes = new Dictionary<string, int>();
            foreach (string key in laneNames)
            {
                lanes[key] = 0;
            }
            foreach (GameObject path in GameObject.FindGameObjectsWithTag("Path"))
            {
                CarSpawner cs = path.GetComponent<CarSpawner>();
                if (cs != null)
                {
                    lanes[cs.id] = cs.CountBeforeTrafficLight(true);
                } else
                {
                    HumanSpawner hs = path.GetComponent<HumanSpawner>();
                    lanes[hs.firstId] = hs.CountBeforeFirstTrafficLight(true);
                    lanes[hs.secondId] = hs.CountBeforeSecondTrafficLight();
                }
            }
            string json = JsonConvert.SerializeObject(lanes);
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

    private void Log(string message)
    {
        if (useLog)
        {
            Debug.Log(message);
        }
    }
}
