using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    public float duration = 0.5f;
    private Image image;
    private IEnumerator coroutine;

    void Start()
    {
        image = GetComponent<Image>();
    }

    IEnumerator BlinkFor(float duration, float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0, 1);
        float elapsedTime = 0f;
        Color imageColor = image.color;
        float currentAlpha = imageColor.a;
        while (elapsedTime < duration)
        {
            imageColor.a = Mathf.Lerp(alpha, 0, elapsedTime / duration);
            image.color = imageColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        imageColor.a = 0;
        image.color = imageColor;
    }

    public void Blink(float alpha)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = BlinkFor(duration, alpha);
        StartCoroutine(coroutine);
    }
}
