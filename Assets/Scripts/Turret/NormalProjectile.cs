
using UnityEngine;
using System.Collections;

public class NormalProjectile : BaseProjectile {
    public bool trail;

    void Start()
    {
        if (trail)
        {
            float radius = GetComponent<SphereCollider>().radius;
            TrailRenderer line = GetComponent<TrailRenderer>();
            line.startWidth = radius * 2;
            line.enabled = true;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;
        Explode(position, rotation);
        Object.Destroy(gameObject);
    }
}
