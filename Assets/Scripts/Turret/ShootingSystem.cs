using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootingSystem : MonoBehaviour {
    public float fireRate;
    public int damage;
    public float fieldOfView;
    public GameObject projectile;
    public List<Canon> canons;

    float fireTimer = 0.0f;
    GameObject target;

    // Update is called once per frame
    void Update () {
        if(target == null)
        {
            return;
        }

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position));
            if (angle < fieldOfView)
            {
                fireTimer = 0.0f;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (!projectile)
        {
            return;
        }

        foreach (Canon canon in canons)
        {
            canon.Shoot(projectile, target);
        }
    }

    public bool isShooting()
    {
        foreach (Canon canon in canons)
        {
            if (canon.isShooting())
            {
                return true;
            }
        }
        return false;
    }

    public void SetTarget(GameObject target){
        this.target = target;
    }
}