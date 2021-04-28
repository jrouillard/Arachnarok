using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour {
    public GameObject explosionPrefab;
    private Collider ignore;
    public int explosionForce;
    public int Damage {get; set;}
    public int explosionRadius;

    public Transform Target { get; set; }

    public void SetIgnore(Collider ignore) 
    {
        this.ignore = ignore;
    }
    protected void Explode(Vector3 position, Quaternion rotation)
    {
        if (explosionPrefab)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, rotation);

            if (ignore) 
            {
                explosion.GetComponent<Explosion>().SetIgnore(ignore);
            }

            explosion.GetComponent<Explosion>().enabled = true;
            Object.Destroy(explosion, 3f);
        }
    }
}