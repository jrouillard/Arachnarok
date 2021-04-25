
using UnityEngine;
using System.Collections;

public class BeamProjectile : BaseProjectile {
    public float beamLength = 0.5f;
    public float speed = 200f;
    public LaserLine laser;

    void Start()
    {
        laser.maxDistance = 0;
    }

    void Update()
    {
        laser.maxDistance += speed * Time.deltaTime;
        if (laser.Contact)
        {
            //Explode();
        }
    }
}
