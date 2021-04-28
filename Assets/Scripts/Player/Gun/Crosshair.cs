using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Crosshair : MonoBehaviour {
    public Image cross;
    
    public Transform maincamera;
    private RaycastHit hit;
    
    public GrapplingGun gun;
    public Text distanceUI;
    // Start is called before the first frame update
    void Start() {
        cross.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update() {
        RaycastHit hit;
        if (Physics.Raycast(maincamera.position, maincamera.forward, out hit, gun.maxDistance, 1<<8)) {
            cross.color = Color.green;
            distanceUI.GetComponent<Text>().text = hit.distance.ToString("0") + "m";
        } else {
            cross.color = Color.white;
            distanceUI.GetComponent<Text>().text = "--m";
        }
    }
}
