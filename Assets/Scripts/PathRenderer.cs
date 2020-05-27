using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    public int spawnPoints = 1;

    private List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            if (i < spawnPoints) { Gizmos.color = Color.blue; }
            if (i == spawnPoints) { Gizmos.color = Color.yellow; }
            if (i == spawnPoints + 1) { Gizmos.color = Color.red; }
            if (i == spawnPoints + 2) { Gizmos.color = Color.white; }
            Gizmos.DrawWireSphere(nodes[i].position, 0.3f);
            if (i < nodes.Count - 1)
            {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }
    }
}
