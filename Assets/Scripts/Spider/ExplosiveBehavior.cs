using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBehavior : MonoBehaviour
{
    public GameObject explosionPrefab;
    public void Explode()
    {
        if (explosionPrefab != null)
        {
            DamageableEntity entity = GetComponent<DamageableEntity>();
            if (entity != null)
            {
                entity.InflictDamages(entity.lifePoints);
            }
            else
            {
                if (explosionPrefab != null) {
                    GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
                    Object.Destroy(explosion, 3f);
                }
                Object.Destroy(gameObject);
            }
        }
    }
}
