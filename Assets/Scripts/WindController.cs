using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] private List<Transform> checkPoints;
    public Vector3 windDirection;

    public bool currentWindDirection(int currentCheckPointIndex, Transform player)
    {
        if (currentCheckPointIndex + 1 < checkPoints.Count)
        {
            windDirection = (checkPoints[currentCheckPointIndex + 1].position - player.position).normalized;
            return true;
        }
        else
        {
            return false;
        }
    }
}