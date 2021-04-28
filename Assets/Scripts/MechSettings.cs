using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public RangeChecker[] rangeCheckers;
    public SpiderAI spiderAi;
    public Vector3 offset;

    public DamageableEntity damaged;
    
    public void Awake()
    {
        if (!spiderAi.target)
        {
            spiderAi.enabled = false;
        }
        foreach(RangeChecker rangeChecker in rangeCheckers) 
        {
            if (!rangeChecker.target)
            {
                rangeChecker.enabled = false;
            }
        }
    }
    public void SetTarget(Transform target)
    {
        foreach(RangeChecker rangeChecker in rangeCheckers) 
        {
            rangeChecker.target = target;
            rangeChecker.enabled = true;
        }
        spiderAi.target = target;
        spiderAi.enabled = true;
    }    
    public void Clean()
    {
        Destroy(gameObject, 6f);
    }

    public bool IsAlive()
    {
        return damaged.IsAlive();
    }

}
