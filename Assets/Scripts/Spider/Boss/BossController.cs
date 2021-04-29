using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public List<Transform> objectives;
    public Transform body;
    public float bodyHeight;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    public float turnSpeed = 2.0f;
    public float speed = 5.0f;
    private List<Matrix4x4> restPosition;
    private Vector3 initialScale;

    //public List<Transform> hands;

    Vector3 GetLocalGroundPosition()
    {
        Vector3 groundPosition = new Vector3();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            Vector3 localPoint = transform.InverseTransformPoint(hit.point);
            groundPosition.y = localPoint.y - 0.5f;
        }
        return groundPosition;
    }

    Vector3 GetGoundPosition()
    {
        return transform.TransformPoint(GetLocalGroundPosition());
    }

    void Start()
    {
        initialScale = transform.parent.localScale;
        restPosition = new List<Matrix4x4>();
        Vector3 groundPosition = GetGoundPosition();
        foreach(Transform objective in objectives)
        {
            objective.position = new Vector3(objective.position.x, groundPosition.y, objective.position.z);
            Vector3 lookAtTarget = new Vector3(transform.position.x, objective.position.y, transform.position.z);
            objective.LookAt(lookAtTarget);
            restPosition.Add(Matrix4x4.TRS(objective.localPosition, objective.localRotation, objective.localScale));
        }
    }

    RaycastHit? FindClosestPoint(Matrix4x4 restPoint, float radius)
    {
        int layerMask = (1 << 8);
        RaycastHit hit;

        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(0, 1 - radius * Mathf.Sin(-1.5f), radius - radius * Mathf.Cos(-1.5f)));
        const float numPoints = 8;
        Vector3 globalA = new Vector3();
        Vector3 globalB = new Vector3();
        for (int i = 1; i < numPoints; i++) {
            float angle = i  * Mathf.PI * 1.3f / numPoints;
            points.Add(new Vector3(0, 1 - radius * Mathf.Sin(-1.5f + angle), radius - radius * Mathf.Cos(-1.5f + angle)));

            globalA = transform.TransformPoint(restPoint.MultiplyPoint(points[i - 1]));
            globalB = transform.TransformPoint(restPoint.MultiplyPoint(points[i]));
            float globalDistance = Vector3.Distance(globalA, globalB);
            if (i == numPoints - 1)
                Debug.DrawLine(globalA, globalB, Color.green);
            else
                Debug.DrawLine(globalA, globalB, Color.red);
            if (Physics.Raycast(globalA, globalB - globalA, out hit, globalDistance, layerMask))
            {
                return hit;
            }
        }
        return null;
    }

    public void FaceTarget(Transform target)
    {
        Vector3 localTarget = transform.InverseTransformPoint(new Vector3(target.position.x, target.position.y, target.position.z));
        Vector3 foo = new Vector3(localTarget.x, 0, localTarget.z);
        Vector3 absoluteFoo = transform.TransformPoint(foo);
        Quaternion targetRotation = Quaternion.LookRotation(absoluteFoo - transform.position, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void UpdatePosition()
    {
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * turnSpeed;
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }

    void ComputeObjectives()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            RaycastHit? hit = FindClosestPoint(restPosition[i], 4f);
            if (hit != null)
            {
                Vector3 position = hit?.point ?? new Vector3();
                objectives[i].position = position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            bodyHeight -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            bodyHeight += 0.1f;
        }
        ComputeObjectives();
    }
}
