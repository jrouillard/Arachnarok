using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public SingleTargetRangeChecker[] rangeCheckers;
    public SpiderAI spiderAi;
    public BossAI bossAi;
    public Vector3 offset;

    public DamageableEntity damaged;
    
    public void Awake()
    {
        if (spiderAi)
        {
            if (!spiderAi.target)
            {
                spiderAi.enabled = false;
            }
        }
        foreach(SingleTargetRangeChecker rangeChecker in rangeCheckers) 
        {
            if (!rangeChecker.target)
            {
                rangeChecker.enabled = false;
            }
        }
    }
    public void SetTarget(Transform target)
    {        
        foreach(SingleTargetRangeChecker rangeChecker in rangeCheckers) 
        {
            rangeChecker.target = target;
            rangeChecker.enabled = true;
        }
        if (spiderAi)
        {
            spiderAi.target = target;
            spiderAi.enabled = true;
        }
    }    
    public void Clean()
    {
        Destroy(gameObject, 6f);
    }

    public bool IsAlive()
    {
        if (!damaged)
        {
            return false;
        }
        return damaged.IsAlive();
    }

}
