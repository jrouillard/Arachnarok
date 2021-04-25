using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
    public float maxDistance = 80;
    public ParticleSystem impact;
    public LineRenderer[] lineRenderers;
    public bool Contact {get; set;}
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            CastTo(transform.InverseTransformPoint(hit.point));
            if (!impact.isEmitting)
            {
                impact.Play();
            }
            impact.transform.position = hit.point;
            Contact = true;
        }
        else
        {
            CastTo(new Vector3(0, 0, maxDistance));
            impact.Stop();
            Contact = false;
        }
    }

    void CastTo(Vector3 position)
    {
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.positionCount = 2;
            lr.SetPosition(1, new Vector3(0, 0));
            lr.SetPosition(1, position);
        }
    }
}
