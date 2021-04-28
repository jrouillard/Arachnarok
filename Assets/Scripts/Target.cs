using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    
    // Indicator icon
    public Image img;
    // The target (location, enemy, etc..)
    public Transform target;
    // UI Text to display the distance
    public Text distance;
    // To adjust the position of the icon

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTarget(Transform target) 
    {
        this.target = target;
    }

    public void Clear(Transform target) 
    {
        Destroy(img);
    }
    public void SetCanvas(Canvas canvas) 
    {
        transform.SetParent(canvas.transform);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}