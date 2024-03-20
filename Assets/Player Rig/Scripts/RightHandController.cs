using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;

public class RightHandController : MonoBehaviour
{
    private static RightHandController _singleton;
    public static RightHandController Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one right hand! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    public ObjectManipulator myOM { get; private set; }

    //  -------------------------------------------------------------------------------------------------------------------  
    // Input references
    public InputActionReference aButton = null; // reference to the A button action in the input map.
    public InputActionReference rTrigger = null; // reference to the trigger action in the input map.
    public InputActionReference bButton = null;
    public InputActionReference rGrip = null;
    public InputActionReference stick = null;
    public InputActionReference rStickClick = null;
    public GameObject cam; //reference to camera offset
    public GameObject rig; //reference to XR rig
    public GameObject cursor; // Cursor for placement
    bool teleportToggle = false; // Prevents user from constantly teleporting when holding up on the right stick
    bool tourToggle = false;


    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
    {
        Singleton = this;
        aButton.action.started += aToggle; // How the a button is gets its pressed detected.
        bButton.action.started += bToggle;
        rStickClick.action.started += rStickClickToggle;
        myOM = GetComponent<ObjectManipulator>();
        if (myOM == null) Debug.LogError("NO OBJECT MANIPULATOR ON THIS SCRIPT!!!");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 svalue = stick.action.ReadValue<Vector2>();
        float tValue = rTrigger.action.ReadValue<float>();
        float gValue = rGrip.action.ReadValue<float>();
        if (!TourMode.Singleton.getTourModeToggle())
        {
            // Edit Mode

            if (LeftHandController.altControls == false)
            {//normal controls
                if (!teleportToggle)
                {
                    if (svalue.y > .8 && svalue.y <= 1)
                    {
                        Teleport();
                        teleportToggle = true;
                    }
                }
                if (tValue > 0 || gValue > 0)
                {
                    //Debug.Log("Right: \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
                }

                if (svalue.y <= 0.8)
                {
                    teleportToggle = false;
                }

                myOM.TryGrab(gValue);
                myOM.TryStretch(tValue);
            }
            else
            {
                if (!tourToggle)
                {
                    if (svalue.y > 0.8 && svalue.y <= 1)
                    {
                        TourMode.Singleton.TouristMode();
                        tourToggle = true;
                    }

                }
                if (svalue.y <= 0.8)
                {
                    tourToggle = false;
                }
                myOM.TryDuplicate(gValue);
            }
            
        }
        else
        {
            if (svalue.y > .8 && svalue.y <= 1)
            {
                //TourMode.Singleton.BackToEditMode();
            }
            if (tValue > 0 || gValue > 0)
            {
                //Debug.Log("Right: \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
                TourMode.Singleton.BackToEditMode();
            }
        }
    }
    public void aToggle(InputAction.CallbackContext context)
    {
        if (!TourMode.Singleton.getTourModeToggle())
        {
            //Debug.Log("A button pressed.");
            if (!LeftHandController.Singleton.getAltControl())
            {
                ObjectCreator.Singleton.PlaceObject();
            }
            else
            {
                
            }
        }
        else
        {
            TourMode.Singleton.BackToEditMode();
        }

    }
    public void bToggle(InputAction.CallbackContext context)
    {
        if (!TourMode.Singleton.getTourModeToggle())
        {
            //Debug.Log("B button pressed.");
            if (!LeftHandController.Singleton.getAltControl())
            {
                ObjectCreator.Singleton.EraseObject();
            }
            else
            {
                ObjectManipulator.TryUngroup();
            }
            //p.EraseObject();
        }
        else
        {
            TourMode.Singleton.BackToEditMode();
        }
    }

    public void Teleport()
    {
        //Debug.Log("Stick y value = 1.");
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
        {
            cam.transform.position = new Vector3(hit.point.x, cam.transform.position.y, hit.point.z); // Select the hit object
        }
        else // If nothing in line of sight
        {

        }

    }

	public void rStickClickToggle(InputAction.CallbackContext context)
	{
        BlockRangler.ActionHistory.RedoAction();
	}

    public GameObject GetRightHandObject()
    {
        return cursor;
    }
}
