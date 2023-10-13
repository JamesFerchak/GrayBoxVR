using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LeftHandController : MonoBehaviour
{
    private static LeftHandController _singleton;
    public static LeftHandController Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one left hand! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }
    //  -------------------------------------------------------------------------------------------------------------------  
    // Input references
    public InputActionReference xButton = null; // reference to the A button action in the input map.
    public InputActionReference lTrigger = null; // reference to the trigger action in the input map.
    public InputActionReference yButton = null;
    public InputActionReference lGrip = null;
    public InputActionReference menu = null;

    // Other script
    PaletteScript p;

    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
    {
        Singleton = this;
        xButton.action.started += xToggle; // How the a button is gets its pressed detected.
        yButton.action.started += yToggle;
        menu.action.started += menuToggle;
    }

    // Update is called once per frame
    void Update()
    {
        float tValue = lTrigger.action.ReadValue<float>();
        float gValue = lGrip.action.ReadValue<float>();
        if (tValue > 0 || gValue > 0)
        {
            Debug.Log("Left \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
        }
    }

    public void xToggle(InputAction.CallbackContext context)
    {
        Debug.Log("X button pressed.");
        GetComponent<PaletteScript>().EditObject();
        //p.EditObject();
    }
    public void yToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Y button pressed.");
        GetComponent<PaletteScript>().MoveObject();
        //p.MoveObject();
    }

    public void menuToggle(InputAction.CallbackContext context)
    {
        Debug.Log("Menu button pressed.");
        GetComponent<PaletteScript>().InteractWithMainMenu();
    }
}
