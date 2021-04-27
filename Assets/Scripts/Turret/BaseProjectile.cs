using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour {
    public GameObject explosionPrefab;
    public int explosionForce;
    public int explosionRadius;
    public int Damage {get; set;}

    public Transform Target { get; set; }

    protected void Explode(Vector3 position, Quaternion rotation)
    {
        if (explosionPrefab){
            GameObject explosion = Instantiate(explosionPrefab, position, rotation);
            Object.Destroy(explosion, 3f);
        }
    }
}