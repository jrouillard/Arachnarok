using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class UnityEventVector3: UnityEvent<Vector3> {}

public class ExplosionTrigger : MonoBehaviour
{
    public ParticleSystem explosion;
    public CameraShake cameraShake;
    public Transform player;

    public float maxDistance = 200;
    public float maxShake = .15f;

    public void Explosion(Vector3 position)
    {
        float distance = Vector3.Distance(player.position, position);
        distance = Mathf.Min(distance, maxDistance);
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = position;
        explosion.Emit(emitParams, 1);
        StartCoroutine(cameraShake.Shake(.5f, (1 - (distance / maxDistance)) * maxShake));
    }
}