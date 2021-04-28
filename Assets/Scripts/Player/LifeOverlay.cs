using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeOverlay : MonoBehaviour
{
    public DamageableEntity entity;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        float sine = 0;
        float elapsedTime = 0;
        while(true)
        {
            elapsedTime += Time.deltaTime;
            sine = (Mathf.Sin(elapsedTime * 2) + 1) / 2f;
            Color imageColor = image.color;
            float t = 1f - (entity.lifePoints / entity.maxLife);
            float f = Mathf.Clamp(t - (sine * 0.7f), 0, 1);
            imageColor.a = f;
            image.color = imageColor;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color imageColor = image.color;
        float t = 1f - (entity.lifePoints / entity.maxLife);
        if (t < 0.5f) {
            imageColor.a = t;
        } else {
            imageColor.a = 0;
        }
        image.color = imageColor;
    }
}
