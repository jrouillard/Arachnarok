using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileEffect : MonoBehaviour
{
    public void EmitSmoke(Vector3 position)
    {
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = position;
        GetComponent<ParticleSystem>().Emit(emitParams, 1);
    }
}
