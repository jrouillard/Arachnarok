
using UnityEngine;
using System.Collections;

public class WarmupLaser : BaseProjectile {
    public GameObject laserBeam;
    public GameObject muzzle;
    public float shootingDuration;

    void OnDestroy()
    {
        GameObject bullet = Instantiate(laserBeam, transform.position, Quaternion.Euler(transform.forward)) as GameObject;
        if (bullet != null && transform.parent)
        {
            bullet.transform.forward = transform.forward;
            BaseProjectile baseProjectile = bullet.GetComponent<BaseProjectile>();
            baseProjectile.Target = Target;
            bullet.transform.SetParent(transform.parent);
            Object.Destroy(bullet, shootingDuration);
        }
        if (muzzle != null)
        {
            GameObject flash = Instantiate(muzzle, transform.position, Quaternion.Euler(transform.forward)) as GameObject;
            if (flash != null)
            {
                flash.transform.forward = transform.forward;
                Object.Destroy(flash, shootingDuration);
            }
        }
    }
}
