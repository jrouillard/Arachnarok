using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFeedback : MonoBehaviour
{
    public Overlay overlay;
    //public ScreenShacker screenShacker;

    public void TakeDamage()
    {
        overlay.Blink(1f);
        //screenShacker.shakeScreen();
    }
}
