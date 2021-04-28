
using UnityEngine;
using System.Collections;

public class TrackingProjectile : BaseProjectile {
    public float turnSpeed = 0.5f;
    public float speed = 30f;
    public GameObject missile;
    public ParticleSystem smokeTrail;

    void Update()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body)
        {
            body.velocity = transform.forward * speed;
            Quaternion targetRotation = Quaternion.LookRotation(Target.position - transform.position);
            body.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed));
        }
    }

    void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;
        Explode(position, rotation);
        Object.Destroy(missile);
        GetComponent<SphereCollider>().enabled = false;
        smokeTrail.Stop();
        Object.Destroy(gameObject, 2);
    }
}