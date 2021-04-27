using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiderController : MonoBehaviour
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
        int layerMask = ~(1 << 6);
        RaycastHit hit;
        const float distance = 40;

        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(0, 1 - radius * Mathf.Sin(-1.5f), radius - radius * Mathf.Cos(-1.5f)));
        const float numPoints = 6;
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
        //Vector3 position = new Vector3(restPoint[0,3], restPoint[1,3], restPoint[2,3]);
        //return transform.TransformPoint(position);
    }

    void UpdateBody()
    {
        Vector3 orthogonals = Vector3.zero;

        float avgSurfaceDist = 0;
        Vector3 point, a, b, c;

        for(int i = 1; i < objectives.Count; i++)
        {
            point = objectives[i].position;
            avgSurfaceDist += transform.InverseTransformPoint(point).y;
            a = (transform.position - point).normalized;
            b = ((objectives[i-1].position) - point).normalized;
            c = Vector3.Cross(b, a);
            orthogonals += c;
            Debug.DrawRay(point, a * 5, Color.red, 0);
            Debug.DrawRay(point, b * 5, Color.green, 0);
            Debug.DrawRay(point, c * 5, Color.blue, 0);
        }
        orthogonals /= objectives.Count;
        Debug.DrawRay(transform.position, orthogonals * 1000, Color.yellow);

        float step = 200.0f * Time.deltaTime;
        Quaternion objectiveRotation = Quaternion.FromToRotation(transform.up, orthogonals) * transform.rotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, objectiveRotation, step);

        Vector3 position = new Vector3();
        for (int i = 1; i < objectives.Count; i++)
        {
            position += transform.InverseTransformPoint(objectives[i].position);
        }
        position /= objectives.Count;
        position.x = 0;
        position.z = 0;
        position.y += bodyHeight;
        transform.position = Vector3.MoveTowards(transform.position, transform.TransformPoint(position), 0.3f); //transform.TransformPoint(position); 
    }

    public void FaceTarget(Transform target)
    {
        Vector3 localTarget = transform.InverseTransformPoint(new Vector3(target.position.x, target.position.y, target.position.z));
        Vector3 foo = new Vector3(localTarget.x, 0, localTarget.z);
        Vector3 absoluteFoo = transform.TransformPoint(foo);
        Quaternion targetRotation = Quaternion.LookRotation(absoluteFoo - transform.position, transform.up);
        //float angle = Vector3.SignedAngle(absoluteFoo, transform.position + transform.right, transform.up);
        //Quaternion bar = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
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
            RaycastHit? hit = FindClosestPoint(restPosition[i], 2.5f);
            if (hit != null)
            {
                Transform parent = hit?.transform;
                Vector3 position = hit?.point ?? new Vector3();
                if (parent != null) {
                    objectives[i].SetParent(parent);
                    objectives[i].position = position;
                }
            }
        }
        transform.parent.SetParent(objectives.Last().parent);
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
        //UpdatePosition();
        ComputeObjectives();
        UpdateBody();
    }
}
