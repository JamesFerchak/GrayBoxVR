using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourMode : MonoBehaviour
{

    private static TourMode _singleton;
    public static TourMode Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one TourMode! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }
    private void Awake()
    {
        Singleton = this;
    }

    public GameObject cam; //reference to camera offset
    public GameObject rig; //reference to XR rig
    public int tourModeShrinkMultiplier = 5;
    Vector3 LastEditModePosition;
    public bool inTourMode = false;
    bool reenableHologram;
    public void TouristMode()
    {
        Transform rightHandObject = RightHandController.Singleton.GetRightHandObject().transform;
        Ray ray = new Ray(rightHandObject.position, rightHandObject.forward); //casts ray

        if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
        {
            LastEditModePosition = cam.transform.position;
            rig.transform.localScale = new Vector3(rig.transform.localScale.x / tourModeShrinkMultiplier, rig.transform.localScale.y / tourModeShrinkMultiplier, rig.transform.localScale.z / tourModeShrinkMultiplier);
            rig.transform.position = hit.point;
            cam.transform.position = hit.point; // Select the hit object
            if (HologramDisplay.Singleton.hologramEnabled == true)
            {
                HologramDisplay.Singleton.hologramEnabled = false;
                reenableHologram = true;
            }
            else
            {
                reenableHologram = false;
            }
            
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
        rig.transform.position = LastEditModePosition;
        cam.transform.position = LastEditModePosition;
        if (reenableHologram == true)
        {
            HologramDisplay.Singleton.hologramEnabled = true;
        }
        inTourMode = false;
    }

    public bool getTourModeToggle()
    {
        return inTourMode;
    }
}
