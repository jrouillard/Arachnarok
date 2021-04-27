using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DamageReceivedEvent: UnityEvent<float> {}

public class DamageableEntity : MonoBehaviour
{
    public int lifePoints = 2;
    public GameObject explosionAsset;
    public BlinkColor colorBlinker;
    public SceneFader sceneFader;
    public Camera playerCamera;
    public float hitDelay = 0.1f;
    private bool isHit = false;
    private bool dieing = false;
    public ActionableEvent onDeath;
    public DamageReceivedEvent onDamageReceived;
    private float maxLife;

    private void Start()
    {
        maxLife = lifePoints;
    }

    public void InflictDamages(int damages)
    {
        onDamageReceived.Invoke(damages / 10);
        lifePoints -= damages;
        if (IsAlive())
        {
            Blink();
            isHit = true;
            StartCoroutine(AllowHit(hitDelay));
        }
        else
        {
            onDeath.Invoke(gameObject);
            StopBlink();
            PlayExplosion();
            if (playerCamera == null) {
                ExplodeChildren();
                Object.Destroy(gameObject);
            } else  {
                KillPlayer();
            }
        }
    }

    private void Blink()
    {
        if (colorBlinker != null)
        {
            colorBlinker.Blink(hitDelay);
        }
    }

    private void StopBlink()
    {
        if (colorBlinker != null)
        {
            colorBlinker.StopBlink();
        }
    }

    private void PlayExplosion()
    {
        if (explosionAsset != null)
        {
            GameObject explosion = Instantiate(explosionAsset, transform.position, Quaternion.Euler(transform.forward)) as GameObject;
            if (explosion != null)
            {
                Object.Destroy(explosion, 4f);
            }
        }
    }

    private bool IsAlive()
    {
        return lifePoints > 0;
    }

    public void FindEveryChild(Transform parent, List<Transform> children)
    {
        int count = parent.childCount;
        for (int i = 0; i < count; i++)
        {
            children.Add(parent.GetChild(i));
        }
    }

    private void ExplodeChildren()
    {
        Component[] children = gameObject.GetComponentsInChildren(typeof(Transform));
        foreach (Transform child in children)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                if (meshFilter.mesh)
                {
                    //Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    //GameObject member = Instantiate("RagdollMember", tip.position, Quaternion.Euler(transform.forward)) as GameObject;
                    Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                    child.SetParent(null);
                    rb.mass = 5;
                    rb.AddForce(child.transform.forward * 4000f);
                    Destroy(child.gameObject, 6f);
                }
            }
        }
    }

    IEnumerator AllowHit(float mTime) {
        yield return new WaitForSeconds(mTime);
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseProjectile projectile = other.GetComponent<BaseProjectile>();
        if (projectile != null && !isHit)
        {
            InflictDamages(projectile.Damage);
        }
    }
    void KillPlayer() 
    {
        if (!dieing) {
            dieing = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            playerCamera.GetComponent<CameraController>().enabled = false;
            sceneFader.FadeToLevel("Menu");
        }
    }
}