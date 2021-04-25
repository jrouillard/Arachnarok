using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public PlayerMovement player;
    public AudioSource audioSourceSteps;
    public AudioSource audioSourceJump;
    public AudioSource audioSourceFly;
    public AudioSource audioSourceGrapple;
    public AudioSource audioSourceGun;
    public AudioSource audioSourceCollision;
    public AudioClip[] clipsSteps; 
    public AudioClip[] clipsJumping; 
    public AudioClip[] clipsLanding; 
    bool soundFlyStarted;
    public float timerStep = 0.3f;
    private float timeSinceLastStep = 0f;
    private Rigidbody rb;
    private Vector3 oldPos;
    private Vector3 dampPos;
    private Vector3 dampVelocity;
    public float smoothSoundTime;

    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        oldPos = player.transform.position;
        audioSourceFly.Play();
    }

    void FixedUpdate()
    {
        if (player.mode == PlayerMovement.Mode.Walking) {
            PlayStepSound();
        } else if (player.mode == PlayerMovement.Mode.Wallruning) {
            PlayStepSound();
        } else if (player.mode == PlayerMovement.Mode.Flying) {
            
        }
        
        PlaySpeedSound();
    }

    public void PlayJumpSound() {
        int rint = (int) Random.Range(0, clipsJumping.Length);
        audioSourceJump.clip = clipsJumping[rint];
        audioSourceJump.Play();
    }

    void PlayStepSound()
    {
        if (rb.velocity.magnitude > 0.5 && (oldPos - transform.position).magnitude > 1 ) {
            if (Time.time - timeSinceLastStep > timerStep) {
                oldPos = transform.position;
                int rint = (int) Random.Range(0, clipsSteps.Length);
                audioSourceSteps.clip = clipsSteps[rint];
                audioSourceSteps.Play();
                timeSinceLastStep = Time.time;
            }
        }
    }

    public void PlayGrappleSound() {
        audioSourceGrapple.Play();
    }
    void PlaySpeedSound() {
        dampPos = Vector3.SmoothDamp(dampPos, player.transform.position, ref dampVelocity, smoothSoundTime);

        float volume = (dampVelocity.magnitude) / (player.maxSpeed * 10f);
        audioSourceFly.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        
        float pitch = (rb.velocity.magnitude) / (player.maxSpeed * 1f);
        audioSourceFly.pitch = Mathf.Clamp(pitch, 0.8f, 1.0f);
    }

    public void PlayGunShoot() {
        audioSourceGun.PlayOneShot(audioSourceGun.clip);
    }

    public void PlayCollision(Collision collision) {
        if (collision.relativeVelocity.magnitude > 5) {
            int rint = (int) Random.Range(0, clipsLanding.Length);
            audioSourceCollision.clip = clipsLanding[rint];
            audioSourceCollision.volume = Mathf.Clamp(collision.relativeVelocity.magnitude/player.maxSpeed/20, 0f , 0.2f);
            audioSourceCollision.Play();
        }
    }
}
