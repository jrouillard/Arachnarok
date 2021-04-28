using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWaypoint : MonoBehaviour
{
    public Vector3 offset;

    public List<Target> targets;

    public void AddTarget(Target target)
    {
        this.targets.Add(target);
    }
    public void RemoveTarget(Target targetRemove)
    {
        targets.Remove(targetRemove);
    }
    public void ClearTargets()
    {
        foreach (Target target in targets) {
            Destroy(target.gameObject);
        }
        targets.Clear();
    }
    
    
    private void Update()
    {
        foreach (Target target in targets) {
            float minX = target.img.GetPixelAdjustedRect().width / 2;
            float maxX = Screen.width - minX;
            float minY = target.img.GetPixelAdjustedRect().height / 2;
            float maxY = Screen.height - minY;

            Vector2 pos = Camera.main.WorldToScreenPoint(target.target.position + offset);

            // Check if the target is behind us, to only show the icon once the target is in front
            if(Vector3.Dot((target.target.position - transform.position), transform.forward) < 0)
            {
                // Check if the target is on the left side of the screen
                if(pos.x < Screen.width / 2)
                {
                    // Place it on the right (Since it's behind the player, it's the opposite)
                    pos.x = maxX;
                }
                else
                {
                    // Place it on the left side
                    pos.x = minX;
                }
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            target.img.transform.position = pos;
            target.distance.text = ((int)Vector3.Distance(target.target.position, transform.position)).ToString() + "m";
        }
    }
}