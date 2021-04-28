using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 10f;
    public float explosionForce = 1000f;
    public float lifetime = 3f;
    public int damage = 5;
    GameObject ignore;
    List<Collider> collidersIgnore = new List<Collider>();
    void Start()
    {
        if (ignore) 
        {
            foreach(Collider c in ignore.GetComponentsInChildren<Collider>()){
                collidersIgnore.Add(c);
            }
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (collidersIgnore.Contains(collider) || collider.enabled == false)
            {
                continue;
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Add force");
                rb.AddExplosionForce(explosionForce, transform.position, radius, 0f, ForceMode.Impulse);
            }
            DamageableEntity entity = collider.GetComponent<DamageableEntity>();
            if (entity != null)
            {
                entity.InflictDamages(damage);
                Debug.Log("Explosion damage");
            }
        }
        Object.Destroy(gameObject, lifetime);
    }
    public void SetIgnore(GameObject ignore) 
    {
        this.ignore = ignore;
    }
}
