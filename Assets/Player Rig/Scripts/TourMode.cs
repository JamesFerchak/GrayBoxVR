using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourMode : MonoBehaviour
{
    public GameObject cam; //reference to camera offset
    public GameObject rig; //reference to XR rig
    public int tourModeShrinkMultiplier = 5;
    Vector3 LastEditModePosition;

    bool inTourMode = false;

    public void TouristMode()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
        {
            LastEditModePosition = cam.transform.position;
            rig.transform.localScale = new Vector3(rig.transform.localScale.x / tourModeShrinkMultiplier, rig.transform.localScale.y / tourModeShrinkMultiplier, rig.transform.localScale.z / tourModeShrinkMultiplier);
            cam.transform.position = hit.point; // Select the hit object
            inTourMode = true;
        }
        else // If nothing in line of sight
        {

        }
    }

    public void BackToEditMode()
    {
        Debug.Log("Back to Edit Mode Called");
        rig.transform.localScale = new Vector3(rig.transform.localScale.x * tourModeShrinkMultiplier, rig.transform.localScale.y * tourModeShrinkMultiplier, rig.transform.localScale.z * tourModeShrinkMultiplier);
        cam.transform.position = LastEditModePosition;

        inTourMode = false;
    }


}
