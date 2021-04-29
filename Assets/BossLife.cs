using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLife : MonoBehaviour
{
   public DamageableEntity lastHeart;
   public bool IsAlive()
   {
       if (!lastHeart)
       { 
           return false;
       }
       return lastHeart.IsAlive();
   }
}
