using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveSpiderAI : MonoBehaviour
{
    public GameObject explosionPrefab;
    void Explode()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, rotation);
            Object.Destroy(explosion, 3f);
        }
    }
}
