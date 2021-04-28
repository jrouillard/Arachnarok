using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveLevel : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    public int phase = 1;
    public Level level;
    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Player") 
        {
            level.GoToPhase(phase);
            Object.Destroy(this.gameObject);
        }
    }
}
