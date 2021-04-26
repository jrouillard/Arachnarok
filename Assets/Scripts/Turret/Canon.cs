using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour
{
    public Transform tip;
    public GameObject muzzleFlash;
    public bool beam;
    public float shootingDuration = 10.0f;
    public float shootForce;
    public float upwardForce;

    private Animator animator;
    private float shootingTimer = 0.0f;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        shootingTimer += Time.deltaTime;
    }

    public void Shoot(GameObject projectile, Transform target)
    {
        shootingTimer = 0f;
        GameObject muzzle = Instantiate(muzzleFlash, tip.position, Quaternion.Euler(transform.forward)) as GameObject;
        Object.Destroy(muzzle, 2f);
        GameObject bullet = Instantiate(projectile, tip.position, Quaternion.Euler(transform.forward)) as GameObject;
        if (bullet != null)
        {
            bullet.transform.forward = transform.forward;
            BaseProjectile baseProjectile = bullet.GetComponent<BaseProjectile>();
            baseProjectile.Target = target;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddForce(transform.forward.normalized * shootForce, ForceMode.Impulse);
            }
            if (beam)
            {
                bullet.transform.SetParent(this.transform);
            }
            Object.Destroy(bullet, shootingDuration);
        }
        //animator.SetTrigger("Shoot");
    }

    public bool isShooting()
    {
        return shootingTimer < shootingDuration;
    }
}
