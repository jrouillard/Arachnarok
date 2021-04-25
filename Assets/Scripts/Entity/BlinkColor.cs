using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColor : MonoBehaviour
{
    Dictionary<int, Material> originalMaterials = new Dictionary<int, Material>();
    private int originalColorIndex;
    public Material blinkMaterial;
    public bool useEmission = false;
    private bool blinking = false;
    private float blinkDelay = 0.1f;

    private void Start()
    {
        // Find all the children of the GameObject with MeshRenderers
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
        // Cycle through each child object found with a MeshRenderer
        foreach (MeshRenderer rend in children)
        {
            originalMaterials.Add(rend.gameObject.GetInstanceID(), rend.material);
        }
    }

    public void Blink(float blinkDelay)
    {
        this.blinkDelay = blinkDelay;
        StartCoroutine("DoBlink");
    }

    public void StopBlink()
    {
        StopCoroutine("DoBlink");
        if (blinking) {
            MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();
            RestoreColors(children);
            blinking = false;
        }
    }

    private void RestoreColors(MeshRenderer[] children)
    {
        // Restore default colors or emission
        foreach (MeshRenderer rend in children)
        {
            rend.material = originalMaterials[rend.gameObject.GetInstanceID()];
        }
    }

    private void ChangeColor(MeshRenderer[] children)
    {
        foreach (MeshRenderer rend in children)
        {
            rend.material = blinkMaterial;
        }
    }

    public IEnumerator DoBlink()
    {
        blinking = true;
        MeshRenderer[] children = GetComponentsInChildren<MeshRenderer>();

        ChangeColor(children);
        yield return new WaitForSeconds(blinkDelay);
        RestoreColors(children);

        if (useEmission)
            StopCoroutine("DoBlink");
        else
        {
            // Reset originalColorIndex
            originalColorIndex = 0;
            StopCoroutine("DoBlink");
        }
        blinking = false;
    }
}