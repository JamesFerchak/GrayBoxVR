using System.Collections;
using System.Collections.Generic;
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
    //  -------------------------------------------------------------------------------------------------------------------  
    // Input references
    public InputActionReference aButton = null; // reference to the A button action in the input map.
    public InputActionReference rTrigger = null; // reference to the trigger action in the input map.
    public InputActionReference bButton = null;
    public InputActionReference rGrip = null;

    // Other script
    PaletteScript p; 

    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
    {
        Singleton = this;
        aButton.action.started += aToggle; // How the a button is gets its pressed detected.
        bButton.action.started += bToggle;
    }

    // Update is called once per frame
    void Update()
    {
        float tValue = rTrigger.action.ReadValue<float>();
        float gValue = rGrip.action.ReadValue<float>();
        if (tValue > 0 || gValue > 0)
        {
            Debug.Log("Right: \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
        }
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
}
