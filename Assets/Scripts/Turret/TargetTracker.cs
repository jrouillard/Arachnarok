using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    public float trackingSpeed = 10f;
    public Vector2 extremumX;
    public Vector2 extremumY;

    GameObject target = null;
    Vector3 lastKnownPosition = Vector3.zero;
    Quaternion lookAtRotation;

    private bool slow;

    void Update()
    {
        if (target)
        {
            if (lastKnownPosition != target.transform.position)
            {
                lastKnownPosition = target.transform.position;
                lastKnownPosition.y -= 1f;
                lookAtRotation = Quaternion.LookRotation(lastKnownPosition - transform.position);
            }
            float speed = slow ? trackingSpeed / 4f : trackingSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, speed * Time.deltaTime);
        }
    }

    public void FocusOn(GameObject target)
    {
        this.target = target;
    }

    public void SetSlowSpeed(bool slowSpeed)
    {
        slow = slowSpeed;
    }
}
