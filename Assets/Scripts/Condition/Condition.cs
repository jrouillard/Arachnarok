using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ActionableEvent: UnityEvent<GameObject> {}

public class Condition : MonoBehaviour
{
    public List<GameObject> objectives;
    public UnityEvent success;

    public void ObjectiveComplete(GameObject objective)
    {
        Debug.Log("Objective complete");
        objectives.Remove(objective);
        if (objectives.Count == 0)
        {
            Debug.Log("Try to invoke success");
            success.Invoke();
        }
    }
}
