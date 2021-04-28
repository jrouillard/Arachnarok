
using UnityEngine;
using TMPro;

/// Thanks for downloading my projectile gun script! :D
/// Feel free to use it in any project you like!
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave
public class ProjectileGun : MonoBehaviour
{
    public GameObject gun;

    //bullet 
    public GameObject bullet;
    public PlayerSounds playerSounds;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    public int TTL;
    //Recoil
    public GameObject player;
    public float recoilForce;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    private Animator animator;

    //bug fixing :D
    public bool allowInvoke = true;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
    private void Awake()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
        
    }

    private void Update()
    {
        MyInput();

        //Set ammo display, if it exists :D
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        Vector3 pos = attackPoint.position + player.GetComponent<Rigidbody>().velocity * Time.deltaTime;
        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - pos;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, pos, Quaternion.identity); //store instantiated bullet in currentBullet
        BaseProjectile baseProjectile = currentBullet.GetComponent<BaseProjectile>();
        if (baseProjectile != null)
        {
            baseProjectile.SetIgnore(player);
        }
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.transform.Rotate(90, 0, 0, Space.Self);
        Object.Destroy(currentBullet, TTL);
        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash, if you have one
        if (muzzleFlash != null) {
            GameObject muzzle = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            muzzle.transform.forward = directionWithoutSpread.normalized;
            muzzle.transform.Rotate(0, 90, 0, Space.Self);
            muzzle.transform.parent = transform;
            Object.Destroy(muzzle, 0.02f);
        }

        bulletsLeft--;
        bulletsShot++;

        animator.SetTrigger("Shoot");
        playerSounds.PlayGunShoot();
        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            player.GetComponent<Rigidbody>().AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

    }
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        Debug.Log("reload");
        reloading = true;
        animator.SetTrigger("Reload");
        Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished()
    {
        //Fill magazine
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
