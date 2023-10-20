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

    ObjectManipulator myOM;

    //  -------------------------------------------------------------------------------------------------------------------  
    // Input references
    public InputActionReference aButton = null; // reference to the A button action in the input map.
    public InputActionReference rTrigger = null; // reference to the trigger action in the input map.
    public InputActionReference bButton = null;
    public InputActionReference rGrip = null;
    public InputActionReference stick = null;
    public GameObject cam;
    // Other script
    PaletteScript p; 

    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
    {
        Singleton = this;
        aButton.action.started += aToggle; // How the a button is gets its pressed detected.
        bButton.action.started += bToggle;
		myOM = GetComponent<ObjectManipulator>();
		if (myOM == null) Debug.LogError("NO OBJECT MANIPULATOR ON THIS SCRIPT!!!");
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 svalue = stick.action.ReadValue<Vector2>();
        float tValue = rTrigger.action.ReadValue<float>();
        float gValue = rGrip.action.ReadValue<float>();
        if (tValue > 0 || gValue > 0)
        {
            Debug.Log("Right: \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
        }
        
        if (svalue.y > .8 && svalue.y <= 1)
        {
            Teleport();
        }

        myOM.TryGrab(gValue);
    }

    public void aToggle(InputAction.CallbackContext context)
    {
        Debug.Log("A button pressed.");
        GetComponent<PaletteScript>().PlaceObject();
        //p.PlaceObject();
    }
    public void bToggle(InputAction.CallbackContext context)
    {
        Debug.Log("B button pressed.");
        GetComponent<PaletteScript>().EraseObject();
        //p.EraseObject();
    }

    public void Teleport()
    {
        Debug.Log("Stick y value = 1.");
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
        {
            cam.transform.position = hit.point; // Select the hit object
            
        }
        else // If nothing in line of sight
        {
            
        }

    }
}
