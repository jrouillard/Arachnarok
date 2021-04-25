using UnityEngine;

public class RotateGun : MonoBehaviour {

    public GrapplingGun grappling;
    public Hook hook;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    void Update() {
        if (!hook.hooked) {
            desiredRotation = transform.parent.rotation;
        }
        else {
            desiredRotation = Quaternion.LookRotation(hook.transform.position - transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}
