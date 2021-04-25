using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour {
    public GameObject explosionPrefab;
    public int explosionForce;
    public int explosionRadius;
    public int Damage {get; set;}

    public GameObject Target { get; set; }

    protected void Explode(Vector3 position, Quaternion rotation)
    {
        if (explosionPrefab){
            GameObject explosion = Instantiate(explosionPrefab, position, rotation);
            Object.Destroy(explosion, 1.5f);
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        // foreach (Collider collider in colliders)
        // {
        //     Rigidbody rb = collider.GetComponent<Rigidbody>();
        //     if (rb != null)
        //     {
        //         rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        //     }
        //     // DamageableEntity entity = collider.GetComponent<DamageableEntity>();
        //     // if (entity != null)
        //     // {
        //     //     entity.inflictDamage(Damage);
        //     // }
        // }
    }
}