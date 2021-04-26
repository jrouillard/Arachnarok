using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // pick a random color
        Color newColor = new Color( Random.value, Random.value, Random.value, 1.0f );
        // apply it on current object's material
        GetComponent<Renderer>().material.color = newColor;
    }
}
