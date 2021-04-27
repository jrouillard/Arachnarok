
using UnityEngine;
using System.Collections;

public class BeamProjectile : BaseProjectile {
    public float beamLength = 0.5f;
    public float speed = 200f;
    public LaserLine laser;
    public float delayBetweenHits = 0.2f;

    private float elapsedTime = 0f;

    void Start()
    {
        laser.maxDistance = 0;
    }

    void Update()
    {
        laser.maxDistance += speed * Time.deltaTime;
        elapsedTime += Time.deltaTime;
        if (laser.Contact &&  elapsedTime > delayBetweenHits)
        {
            Explode(laser.impact.transform.position, laser.impact.transform.rotation);
            elapsedTime = 0f;
        }
    }
}
