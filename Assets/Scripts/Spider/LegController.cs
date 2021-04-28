using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine;

public class LegJump
{
    private Vector3 initial;
    private Vector3 destination;
    private float animationDuration = 0.2f;
    private float startAnimationTime;
    public AnimationCurve curve;
    public float maxAltitude { get; set; }

    private bool moving = false;

    public void Start(AnimationCurve ac, float duration)
    {
        startAnimationTime = Time.time - 20;
        animationDuration = duration;
        curve = ac;
    }

    public void UpdatePositions(Vector3 initial, Vector3 destination)
    {
        this.initial = initial;
        this.destination = destination;
        startAnimationTime = Time.time;
        moving = true;
    }

    public Vector3 ComputePosition()
    {
        if (Time.time >= startAnimationTime + animationDuration)
        {
            moving = false;
            return destination;
        }

        Vector3 localInitial = initial;
        Vector3 localDestination = destination;
        Vector3 position = new Vector3();
        float t = (Time.time - startAnimationTime) / animationDuration;
        position.x = Mathf.Lerp(localInitial.x, localDestination.x, t);
        position.z = Mathf.Lerp(localInitial.z, localDestination.z, t);
        position.y = Mathf.Lerp(localInitial.y, localDestination.y, t) + curve.Evaluate(t * curve.keys.Select(k => k.time).Max()) * maxAltitude;
        return position;
    }

    public bool isMoving()
    {
        return moving;
    }
}

//[ExecuteInEditMode]
public class LegController : MonoBehaviour
{
	public Transform shoulder;
	public Transform upperArm;
	public Transform forearm;
	public Transform hand;
    public bool elbowUp;
    public AnimationCurve curve;
    public GameObject globalTarget;

    //public UnityEventVector3 groundHit;

    // hop control
    public float hopDuration = 0.2f;
    public float maxDistance;
    public float hopAltitude;
    public Transform objective;
    private LegJump hop = new LegJump();

    void Start()
    {
        globalTarget.transform.SetParent(null);
        globalTarget.transform.position = objective.position;
        hop.maxAltitude = hopAltitude;
        hop.Start(curve, hopDuration);
    }

    void CheckDistance()
    {
        if (!hop.isMoving())
        {
            float distance = Vector3.Distance(globalTarget.transform.position, objective.position);
            if (distance > maxDistance * transform.lossyScale.x)
            {
                hop.UpdatePositions(transform.InverseTransformPoint(globalTarget.transform.position), transform.InverseTransformPoint(objective.position));
            }
        }
    }

    void MoveLeg()
    {
        if (hop.isMoving())
        {
            globalTarget.transform.position = transform.TransformPoint(hop.ComputePosition());
            if (!hop.isMoving())
            {
                globalTarget.transform.SetParent(objective.parent);
                /*RaycastHit hit;
                if (Physics.Raycast(transform.position, -transform.up, out hit))
                {
                    globalTarget.SetParent(hit.collider.gameObject.transform);
                }*/
                //groundHit.Invoke(globalTarget.transform.position);
            }
        }
    }

    bool ComputeCircleCircleIntersection(Vector2 center1, float radius1, Vector2 center2, float radius2, out Vector2 p1, out Vector2 p2)
    {
        DrawPoint(center1, Color.white);
        DrawPoint(center2, Color.black);
        float distance = Vector2.Distance(center1, center2);
        p1 = new Vector2();
        p2 = new Vector2();
        if (distance > radius1 + radius2)
        {
            return false;
        }
        else if (distance < Mathf.Abs(radius1 - radius2))
        {
            return false;
        }
        else if (radius1 == radius2 && center1 == center2)
        {
            return false;
        }
        else
        {
            float a = (radius1 * radius1 - radius2 * radius2 + distance * distance) / (2 * distance);
            float h = Mathf.Sqrt(radius1 * radius1 - a * a);
            Vector2 p = center1 + a * (center2 - center1) / distance;
            p1.x = p.x + h * (center2.y - center1.y) / distance;
            p1.y = p.y - h * (center2.x - center1.x) / distance;
            p2.x = p.x - h * (center2.y - center1.y) / distance;
            p2.y = p.y + h * (center2.x - center1.x) / distance;

            float cross = CrossProduct(center2 - center1, p1 - center1);
            if (cross < 0) {
                Vector2 tmp = p1;
                p1 = p2;
                p2 = tmp;
            }
            return true;
        }
    }

    // Rotate shoulder horizontally (relative to the arm)
    void UpdateShoulder(Transform currentTransform, Vector3 localTarget)
    {

        Vector3 localShoulderObjective = new Vector3(localTarget.x, shoulder.localPosition.y, localTarget.z);
        Vector3 shoulderObjective = currentTransform.TransformPoint(localShoulderObjective);
        shoulder.transform.LookAt(shoulderObjective, shoulder.up);
        /*Vector3 eulerRotation = shoulder.transform.localRotation.ToEulerAngles();
        if (eulerRotation.y > Mathf.PI / 2)
            shoulder.transform.localRotation = Quaternion.Euler(eulerRotation.x, 90, eulerRotation.z);
        if (eulerRotation.y < -Mathf.PI / 2)
            shoulder.transform.localRotation = Quaternion.Euler(eulerRotation.x,- 90, eulerRotation.z);*/
    }

    float CrossProduct(Vector2 v1, Vector2 v2)
    {
        v1.Normalize();
        v2.Normalize();
        return (v1.x * v2.y) - (v1.y * v2.x);
    }

    void DrawPoint(Vector2 point, Color color)
    {
        Debug.DrawLine(transform.TransformPoint(new Vector3(0, point.y, point.x - 1)), transform.TransformPoint(new Vector3(0, point.y, point.x + 1)), color);
        Debug.DrawLine(transform.TransformPoint(new Vector3(0, point.y - 1, point.x)), transform.TransformPoint(new Vector3(0, point.y + 1, point.x)), color);
    }

    void UpdateIK(Transform currentTransform, Vector3 localTarget)
    {
        Vector3 localUpperArm = currentTransform.InverseTransformPoint(upperArm.position);
        Vector3 localForearm = currentTransform.InverseTransformPoint(forearm.position);
        Vector3 localHand = currentTransform.InverseTransformPoint(hand.position);
        Vector3 localShoulder = currentTransform.InverseTransformPoint(shoulder.position);
        float distance = Vector3.Distance(localUpperArm, localTarget);
        Vector2 distanceUpperArmTarget2d = new Vector2(localUpperArm.x - localTarget.x, localUpperArm.z - localTarget.z);
        Vector2 localUpperArm2d = new Vector2(0, localUpperArm.y);
        Vector2 localTarget2d = new Vector2(distanceUpperArmTarget2d.magnitude, localTarget.y);
        //Vector2 localTarget2d = new Vector2(localTarget.z - localUpperArm.z, localTarget.y);

        float upperArmLength = Vector3.Distance(localUpperArm, localForearm);
        float forearmLength = Vector3.Distance(localForearm, localHand);
        Vector2 p1, p2;

        if (ComputeCircleCircleIntersection(localUpperArm2d, upperArmLength, localTarget2d, forearmLength, out p1, out p2))
        {
            DrawPoint(new Vector2(localUpperArm.z, localUpperArm.y), Color.blue);
            DrawPoint(new Vector2(localUpperArm.z + p1.x, p1.y), Color.green);
            DrawPoint(new Vector2(localUpperArm.z + p2.x, p2.y), Color.red);
            DrawPoint(new Vector2(localUpperArm.z + localTarget2d.x,localTarget2d.y), Color.red);
            Vector3 origin = new Vector3(localUpperArm2d.x, localUpperArm2d.y, 0);
            Vector3 targetdebug = new Vector3(localTarget2d.x, localTarget2d.y, 0);
            Vector2 p3;
            float a = CrossProduct(p1 - localUpperArm2d, localTarget2d - localUpperArm2d);
            if (elbowUp)
            {
                p3 = a > 0 ? p2 : p1;
            }
            else
            {
                p3 = a > 0 ? p1 : p2;
            }
            float t = p3.x / localTarget2d.x;
            Vector3 localForearm3d = new Vector3(Mathf.LerpUnclamped(localUpperArm.x, localTarget.x, t), p3.y, Mathf.LerpUnclamped(localUpperArm.z, localTarget.z, t));
            upperArm.LookAt(currentTransform.TransformPoint(localForearm3d), shoulder.up);
            forearm.LookAt(globalTarget.transform.position, shoulder.up);
        } else {
            upperArm.LookAt(globalTarget.transform.position, shoulder.up);
            forearm.LookAt(globalTarget.transform.position, upperArm.up);
        }
    }

    void LateUpdate()
    {
        Transform currentTransform = GetComponent<Transform>();
        if (!currentTransform || !globalTarget) 
        {
            return;
        }
        Vector3 localTarget = currentTransform.InverseTransformPoint(globalTarget.transform.position);
        UpdateShoulder(currentTransform, localTarget);
        UpdateIK(currentTransform, localTarget);
        // hop control
        CheckDistance();
        MoveLeg();
    }
}
