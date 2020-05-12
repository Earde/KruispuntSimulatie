using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class SpawnHelper
{
    public static float GetSpawnAngle(Transform path, Transform obj)
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
                }
                else
                {
                    break;
                }
            }
        }
        if (end.HasValue)
        {
            Vector3 referenceRight = Vector3.Cross(Vector3.up, obj.forward);
            float angle = Vector3.Angle(end.Value - start.Value, obj.forward);
            float sign = Mathf.Sign(Vector3.Dot(end.Value - start.Value, referenceRight));
            return sign * angle; // -180 to 180 degrees
        }
        else
        {
            return 0.0f;
        }
    }
}