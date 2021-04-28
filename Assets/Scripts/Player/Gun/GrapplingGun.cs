using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    public PlayerSounds playerSounds;
    private Vector3 grapplePoint;
    private Transform stuckTo;
    private Quaternion offset;
    private float distance;
    private bool grappling;
    public int button;
    public Transform gunTip, player;
    public Camera maincamera;
    public float maxDistance = 100f;
    private SpringJoint springJoint;
    public Hook hook;
    private bool readyToShoot = true;
    private bool dashing = false;
    private bool jumping = false;
    private bool grappling_rewind = false;
    public float shootForce;
    public float dashDistanceBySecond;
    private Animator animator;
    private Vector3 localPoint;
    public float springForce;
    public float damper;
    public float massScale;
    
    public float maxForce;
    public float curveSize;
    public float scrollSpeed;
    public float animSpeed;
    public AnimationCurve effectOverTime;
    public AnimationCurve curve;
    public AnimationCurve curveEffectOverDistance;
    public float segments;
    private float _time;
    private float breakAt;
    private List<GameObject> spheres;
    private Rigidbody player_rb;
    private void ProcessBounce() {
        var vectors = new List<Vector3>();

        _time = Mathf.MoveTowards(_time, 1f,
            Mathf.Max(Mathf.Lerp(_time, 1f, animSpeed * Time.deltaTime) - _time, 0.2f * Time.deltaTime));
        
        vectors.Add(gunTip.position);

        var forward = Quaternion.LookRotation(grapplePoint - gunTip.position);
        var up = forward * Vector3.up;

        for (var i = 1; i < segments + 1; i++) {
            var delta = 1f / segments * i;
            var realDelta = delta * curveSize;
            while (realDelta > 1f) realDelta -= 1f;
            var calcTime = realDelta + -scrollSpeed * _time;
            while (calcTime < 0f) calcTime += 1f;

            var defaultPos = GetPos(delta);
            var effect = Eval(effectOverTime, _time) * Eval(curveEffectOverDistance, delta) * Eval(curve, calcTime);
                
            // spheres.ElementAt(i - 1).transform.position = defaultPos + up * effect;
            vectors.Add(defaultPos + up * effect);
        }  

        lr.positionCount = vectors.Count;
        lr.SetPositions(vectors.ToArray());
    }

    private Vector3 GetPos(float d) {
        return Vector3.Lerp(gunTip.position, grapplePoint, d);
    }

    private static float Eval(AnimationCurve ac, float t) {
        return ac.Evaluate(t * ac.keys.Select(k => k.time).Max());
    }


    void Start()
    {
        
        lr.useWorldSpace = true;
        animator = gameObject.GetComponent<Animator>();
        // spheres = new List<GameObject>();
        // for (var i = 1; i < segments + 1; i++) {
        //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //     sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //     sphere.layer = 10;
        //     spheres.Add(sphere);
        // }
        player_rb = player.GetComponent<Rigidbody>();
        // Debug.Log(spheres.Count);
    }

    void Awake() {
        lr = GetComponent<LineRenderer>();
    
        lr.material.color = Color.red;
        lr.material.SetColor("_EmissionColor", Color.red * 4);
        lr.material.EnableKeyword("_EMISSION");
    }

    public void Hooked() {
        grapplePoint = hook.transform.position;
        localPoint = hook.objectHit.InverseTransformPoint(grapplePoint);
        
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
        AddSpringJoint(grapplePoint, null, distanceFromPoint * 0.8f, distanceFromPoint * 0.8f, springForce, damper, massScale);
        
        // float alpha = 1.0f;
        // Gradient gradient = new Gradient();
        // gradient.SetKeys(
        //     new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f),  new GradientColorKey(Color.magenta, 0.03f), new GradientColorKey(Color.red, 0.3f) },
        //     new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        // );
        // lr.colorGradient = gradient;
        // lr.material.SetColor("_EmissionColor", Color.red);

        
        lr.material.color = Color.green;
        lr.material.SetColor("_EmissionColor", Color.green * 4);
        lr.material.EnableKeyword("_EMISSION");

    }

    void Update() {
        grapplePoint = hook.transform.position;
        jumping = Input.GetButton("Jump");
        if (!dashing && grappling && jumping && hook.hooked) {
            // Vector3 dir = (hook.transform.position - player.position).normalized;
            // Debug.Log((hook.transform.position - player.position).magnitude);
            // // float force = Mathf.Max(3000, maxForce * (1 - (maxDistance - (hook.transform.position - player.position).magnitude)/maxDistance));
            // float force = 8;
            // Debug.Log(force);
            // player.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange );
            dashing = true;
            player_rb.velocity = player_rb.velocity / 1.2f;
            player_rb.angularVelocity =  player_rb.angularVelocity / 1.2f; 
        }
        if (Input.GetMouseButtonDown(button) && readyToShoot) {
            Vector3 point = CastRay();
            ShootHook(point);
        }
        else if (Input.GetMouseButtonUp(button) && hook.hooked) {
            RecallHook();
        }
    }
    void GoToHook() {
        springJoint.minDistance = Mathf.Max(0, springJoint.minDistance - dashDistanceBySecond * Time.deltaTime);
        springJoint.maxDistance = Mathf.Max(0, springJoint.maxDistance - dashDistanceBySecond * Time.deltaTime);
        lr.material.color = Color.cyan;
        lr.material.SetColor("_EmissionColor", Color.cyan * 4);
        lr.material.EnableKeyword("_EMISSION");
    }

    void LateUpdate() {
        if (dashing && jumping) {
            GoToHook();
        }
        if (grappling_rewind) {
            float step = 2 * shootForce * Time.deltaTime; // calculate distance to move
            hook.transform.position = Vector3.MoveTowards(hook.transform.position, gunTip.position, step);
            
            float distanceHook = Vector3.Distance(gunTip.position, hook.transform.position);
            if (distanceHook < 0.005f)
            {
                Rigidbody hook_rb = hook.transform.GetComponent<Rigidbody>();
                Destroy(hook_rb);
                grappling_rewind = false;
                grappling = false;
                stuckTo = null;
                readyToShoot = true;
                lr.positionCount = 0;
                hook.transform.parent = transform;
            }
        }
        DrawRope();
    }

    void RecallHook() {
        dashing = false;
        Destroy(springJoint);
        Rigidbody hook_rb = hook.transform.GetComponent<Rigidbody>();
        hook_rb.velocity = Vector3.zero;
        grappling_rewind = true;
        hook.hooked = false;
        if (hook.launched) {
            hook.launched = false;
        }
        lr.material.color = Color.red;
        lr.material.SetColor("_EmissionColor", Color.red * 4);
        lr.material.EnableKeyword("_EMISSION");
    }

    void ShootHook(Vector3 targetPoint) {
        readyToShoot = false;
        hook.launched = true;
        hook.transform.parent = null;
        animator.SetTrigger("Grapple");
        grappling = true;
        Vector3 direction = targetPoint - gunTip.position;
        grapplePoint = hook.transform.position;
        Rigidbody hook_rb = hook.gameObject.AddComponent<Rigidbody>() as Rigidbody; 
        // Add the rigidbody.
        // hook_rb.isKinematic = false;
        hook_rb.interpolation = RigidbodyInterpolation.Interpolate;
        hook_rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        hook_rb.useGravity = true;
        localPoint = hook.transform.InverseTransformPoint(grapplePoint);
        stuckTo = hook.transform.transform;
        hook_rb.AddForce(direction.normalized * shootForce, ForceMode.Impulse);

        playerSounds.PlayGrappleSound();
        _time = 0f;

    }


    Vector3 CastRay() {
        RaycastHit hit;
        
        Ray ray = new Ray(maincamera.transform.position, maincamera.transform.forward);
        if (Physics.Raycast(ray, out hit, maxDistance, ~(1<<11 | 1<<6))) {
            return hit.point;
        } else {
            return ray.GetPoint(75);
        }
    }

    void AddSpringJoint(Vector3 grapplePoint, Rigidbody rb, float minDistance,  float maxDistance,  float spring, float damper, float massScale) {
        Destroy(springJoint);
        springJoint = player.gameObject.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = grapplePoint;
        springJoint.connectedBody = rb;
        springJoint.maxDistance = maxDistance;
        springJoint.minDistance = minDistance;
        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.massScale = massScale;
    }
    
    private Vector3 currentGrapplePosition;
    

    void DrawRope() {
        //If not grappling, don't draw rope
        if (!stuckTo) return;
        
        float distanceHook = Vector3.Distance(gunTip.position, hook.transform.position);
        if (grappling && !hook.hooked) {
            if (distanceHook > maxDistance) {
                RecallHook();
            }
        }
        

        Vector3 target = stuckTo.TransformPoint(localPoint);

        if (springJoint) {
            springJoint.connectedAnchor = target;
        }
        if (!hook.hooked && distanceHook > 0.1f) {
            
            // float alpha = 1.0f;
            // Gradient gradient = new Gradient();

            // gradient.SetKeys(
            //     new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f),  new GradientColorKey(Color.magenta, 0.03f), new GradientColorKey(Color.red, 0.3f) },
            //     new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            // );
            // lr.colorGradient = gradient;
            
            ProcessBounce();
            // DrawSimpleLine();
        } else {
            DrawSimpleLine();
        }
    }
    void DrawSimpleLine() {
        Vector3 middle =  (gunTip.position + grapplePoint)/2;
        lr.positionCount = 3;
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, middle);
        lr.SetPosition(2, grapplePoint);
    }
    public bool IsGrappling() {
        return stuckTo != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
