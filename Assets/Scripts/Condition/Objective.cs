using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public void ObjectiveMet()
    {
        Debug.Log("Objective met!");
        Object.Destroy(gameObject);
    }
}
