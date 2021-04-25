using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Hook hook;
    public PlayerSounds playerSounds;
    //Ground
    public float groundSpeed =18f;
    public float runSpeed = 25f;
    public float grAccel = 55f;

     //Air
    public float airSpeed = 15f;
    public float airAccel = 40f;

     //Jump
    public float jumpUpSpeed = 18.2f;
    public float dashSpeed = 12f;

     //Wall
    public float wallSpeed = 30f;
    public float wallClimbSpeed = 10f;
    public float wallAccel = 40f;
    public float maxSpeed = 60f;
    public float wallRunTime = 3f;
    public float wallStickiness = 150f;
    public float wallStickDistance = 2f;
    public float wallFloorBarrier = 60f;
    public float wallBanTime = 1f;
    Vector3 bannedGroundNormal;

    //Cooldowns
    bool canJump = true;
    bool jumping = false;
    bool canDJump = true;
    float wallBan = 0f;
    float wrTimer = 0f;
    float wallStickTimer = 0f;

    //States
    bool jump;
    bool grounded;

    Vector3 groundNormal = Vector3.up;

    CapsuleCollider col;

    public enum Mode
    {
        Walking,
        Grappling,
        Flying,
        Wallruning
    }
    public Mode mode = Mode.Flying;

    CameraController camCon;
    Rigidbody rb;
    Vector3 dir = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camCon = GetComponentInChildren<CameraController>();
        col = GetComponent<CapsuleCollider>();
    }

    //void OnGUI()
    //{
    //    GUILayout.Label("Spid: " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
    //    GUILayout.Label("SpidUp: " + rb.velocity.y);
    //}

    void Update()
    {
        col.material.dynamicFriction = 0f;
        dir = Direction();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        //Special use
        //if (Input.GetKeyDown(KeyCode.T)) transform.position = new Vector3(0f, 30f, 0f);
        //if (Input.GetKeyDown(KeyCode.X)) rb.velocity = new Vector3(rb.velocity.x, 40f, rb.velocity.z);
        //if (Input.GetKeyDown(KeyCode.V)) rb.AddForce(dir * 20f, ForceMode.VelocityChange);
    }

    void FixedUpdate()
    {

        if (wallStickTimer == 0f && wallBan > 0f)
        {
            bannedGroundNormal = groundNormal;
        }
        else
        {
            bannedGroundNormal = Vector3.zero;
        }

        wrTimer = Mathf.Max(wrTimer - Time.deltaTime, 0f);
        wallStickTimer = Mathf.Max(wallStickTimer - Time.deltaTime, 0f);
        wallBan = Mathf.Max(wallBan - Time.deltaTime, 0f);

        switch (mode)
        {
            case Mode.Grappling:
                camCon.SetTilt(0);
                break;

            case Mode.Wallruning:
                camCon.SetTilt(WallrunCameraAngle());
                Wallrun(dir, wallSpeed, wallClimbSpeed, wallAccel);
                break;

            case Mode.Walking:
                camCon.SetTilt(0);
                Walk(dir, groundSpeed, grAccel);
                break;

            case Mode.Flying:
                camCon.SetTilt(0);
                AirMove(dir, airSpeed, airAccel);
                break;
        }

        jump = false;
        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private Vector3 Direction()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hAxis, 0, vAxis);
        return rb.transform.TransformDirection(direction);
    }

    #region Collisions
    void OnCollisionEnter(Collision collision)
    {
        playerSounds.PlayCollision(collision);
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            float angle;

            foreach (ContactPoint contact in collision.contacts)
            {
                angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < wallFloorBarrier)
                {
                    EnterWalking();
                    grounded = true;
                    groundNormal = contact.normal;
                    return;
                }
            }

            if (VectorToGround().magnitude > 0.2f)
            {
                grounded = false;
            }

            if (grounded == false)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.otherCollider.tag != "NoWallrun" && contact.otherCollider.tag != "Player" && mode != Mode.Walking)
                    {
                        angle = Vector3.Angle(contact.normal, Vector3.up);
                        if (angle > wallFloorBarrier && angle < 120f)
                        {
                            grounded = true;
                            groundNormal = contact.normal;
                            EnterWallrun();
                            return;
                        }
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            EnterFlying();
        }
    }
    #endregion

    #region Entering States
    void EnterWalking()
    {
        if (mode != Mode.Walking && canJump)
        {
            if (rb.velocity.y < -1.2f)
            {
                //camCon.Punch(new Vector2(0, -3f));
            }
            //StartCoroutine(bHopCoroutine(bhopLeniency));
            // gameObject.SendMessage("OnStartWalking");
            mode = Mode.Walking;
        }
    }

    void EnterFlying(bool wishFly = false)
    {
        grounded = false;
        if (mode == Mode.Wallruning && VectorToWall().magnitude < wallStickDistance && !wishFly)
        {
            return;
        }
        else if (mode != Mode.Flying)
        {

            wallBan = wallBanTime;
            canDJump = true;
            mode = Mode.Flying;
        }
    }

    void EnterWallrun()
    {
        if (mode != Mode.Wallruning)
        {
            if (VectorToGround().magnitude > 0.2f && CanRunOnThisWall(bannedGroundNormal) && wallStickTimer == 0f)
            {
                // gameObject.SendMessage("OnStartWallrunning");
                wrTimer = wallRunTime;
                canDJump = true;
                mode = Mode.Wallruning;
            }
            else
            {
                EnterFlying(true);
            }
        }
    }
    #endregion

    #region Movement Types
    void Walk(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (jump && canJump)
        {
            //gameObject.SendMessage("OnJump");
            Jump();
        }
        else
        {
            wishDir = wishDir.normalized;
            Vector3 spid = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (spid.magnitude > maxSpeed) acceleration *= spid.magnitude/maxSpeed;
            Vector3 direction = wishDir * maxSpeed - spid;

            if (direction.magnitude < 0.5f)
            {
                acceleration *= direction.magnitude / 0.5f;
            }

            direction = direction.normalized * acceleration;
            float magn = direction.magnitude;
            direction = direction.normalized;
            direction *= magn;

            Vector3 slopeCorrection = groundNormal * Physics.gravity.y / groundNormal.y;
            slopeCorrection.y = 0f;
            direction += slopeCorrection;

            rb.AddForce(direction, ForceMode.Acceleration);
        }
    }

    void AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (!hook.hooked && jump)
        {
            //gameObject.SendMessage("OnDoubleJump");
            DoubleJump(wishDir);
        }

        float projVel = Vector3.Dot(new Vector3(rb.velocity.x, 0f, rb.velocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        rb.AddForce(wishDir.normalized * accelVel, ForceMode.VelocityChange);
    }

    void Wallrun(Vector3 wishDir, float maxSpeed, float climbSpeed, float acceleration)
    {
        if (jump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            Vector3 jumpOffWall = groundNormal.normalized;
            jumpOffWall *= dashSpeed;
            jumpOffWall.y = 0;
            rb.AddForce(jumpOffWall, ForceMode.VelocityChange);
            wrTimer = 0f;
            EnterFlying(true);
            PlayJumpSound();
        }
        else if (wrTimer == 0f)
        {
            rb.AddForce(groundNormal * 3f, ForceMode.VelocityChange);
            EnterFlying(true);
            PlayJumpSound();
        }
        else
        {
            //Horizontal
            Vector3 distance = VectorToWall();
            wishDir = RotateToPlane(wishDir, -distance.normalized);
            wishDir = wishDir.normalized * maxSpeed;
            wishDir.y = Mathf.Clamp(wishDir.y, -climbSpeed, climbSpeed);
            Vector3 wallrunForce = wishDir - rb.velocity;
            wallrunForce = wallrunForce.normalized * acceleration;
            if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude > maxSpeed) wallrunForce /= 2f;

            //Vertical
            if (rb.velocity.y < 0f && wishDir.y > 0f) wallrunForce.y = 2f * acceleration;

            //Anti-gravity force
            Vector3 antiGravityForce = -Physics.gravity;
            if (wrTimer < 0.33 * wallRunTime)
            {
                antiGravityForce *= wrTimer / wallRunTime;
                wallrunForce += antiGravityForce + Physics.gravity;
            }
            if (distance.magnitude > wallStickDistance) distance = Vector3.zero;

            //Adding forces
            rb.AddForce(antiGravityForce, ForceMode.Acceleration);
            rb.AddForce(distance.normalized * wallStickiness * Mathf.Clamp(distance.magnitude / wallStickDistance, 0, 1), ForceMode.Acceleration);
            rb.AddForce(wallrunForce, ForceMode.Acceleration);
        }
        if (!grounded)
        {
            wallStickTimer = 0.2f;
            EnterFlying();
        }
    }

    void Jump()
    {
        if (mode == Mode.Walking && canJump)
        {
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);
            StartCoroutine(jumpCooldownCoroutine(0.2f));
            EnterFlying(true);
            PlayJumpSound();
        }
    }

    void DoubleJump(Vector3 wishDir)
    {
        if (canDJump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);

            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            if (wishDir != Vector3.zero)
            {
                Vector3 horSpid = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                Vector3 newSpid = wishDir.normalized;
                float newSpidMagnitude = dashSpeed;

                if (horSpid.magnitude > dashSpeed)
                {
                    float dot = Vector3.Dot(wishDir.normalized, horSpid.normalized);
                    if (dot > 0)
                    {
                        newSpidMagnitude = dashSpeed + (horSpid.magnitude - dashSpeed) * dot;
                    }
                    else
                    {
                        newSpidMagnitude = Mathf.Clamp(dashSpeed * (1 + dot), dashSpeed * (dashSpeed/horSpid.magnitude) , dashSpeed);
                    }
                }

                newSpid *= newSpidMagnitude;

                rb.AddForce(newSpid - horSpid, ForceMode.VelocityChange);
            }
            PlayJumpSound();
            canDJump = false;
        }
    }
    #endregion
    void PlayJumpSound() {
        playerSounds.PlayJumpSound();
    }
    Vector3 RotateToPlane(Vector3 vect, Vector3 normal)
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(normal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = -Vector3.Angle(Vector3.up, normal);
        rotation = Quaternion.AngleAxis(angle, rotDir);
        vect = rotation * vect;
        return vect;
    }

    float WallrunCameraAngle()
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = Vector3.SignedAngle(Vector3.up, groundNormal, Quaternion.AngleAxis(90f, rotDir) * groundNormal);
        angle -= 90;
        angle /= 180;
        Vector3 playerDir = transform.forward;
        Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

        return Vector3.Cross(playerDir, normal).y * angle;
    }

    bool CanRunOnThisWall(Vector3 normal)
    {
        return (Vector3.Angle(normal, groundNormal) > 10 || wallBan == 0f);
    }

    Vector3 VectorToWall()
    {
        Vector3 direction;
        Vector3 position = transform.position + Vector3.down * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(position, -groundNormal, out hit, wallStickDistance) && Vector3.Angle(groundNormal, hit.normal) < 70)
        {
            groundNormal = hit.normal;
            direction = hit.point - position;
            return direction;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    Vector3 VectorToGround()
    {
        Vector3 position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, wallStickDistance))
        {
            return hit.point - position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    #region Coroutines
    IEnumerator jumpCooldownCoroutine(float time)
    {
        canJump = false;
        yield return new WaitForSeconds(time);
        canJump = true;
    }
    #endregion
}