using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

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

	public ObjectManipulator myOM { get; private set; }

	//  -------------------------------------------------------------------------------------------------------------------  
	// Input references
	public InputActionReference xButton = null; // reference to the A button action in the input map.
	public InputActionReference lTrigger = null; // reference to the trigger action in the input map.
	public InputActionReference yButton = null;
	public InputActionReference lGrip = null;
	public InputActionReference menu = null;
	public InputActionReference lStickClick = null;
    public GameObject cursor; // Cursor for placement
	public ActionBasedContinuousMoveProvider continousMove;
	public static bool altControls { get; private set; }
	public static bool altControlsHold { get; private set; }
    public GameObject RightControlUI;
    public GameObject RightAltControlUI;
    public GameObject LeftControlUI;
    public GameObject LeftAltControlUI;


    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
	{
		Singleton = this;
		xButton.action.started += xToggle; // How the a button is gets its pressed detected.
		yButton.action.started += yToggle;
		lStickClick.action.started += lStickClickToggle;
		menu.action.started += menuToggle;
		altControlsHold = false;
		myOM = GetComponent<ObjectManipulator>();
		if (myOM == null) Debug.LogError("NO OBJECT MANIPULATOR ON THIS SCRIPT!!!");
	}

	// Update is called once per frame
	void Update()
	{
		float tValue = lTrigger.action.ReadValue<float>();
		float gValue = lGrip.action.ReadValue<float>();

		if (altControlsHold == true)
		{
			if (myOM.heldObject == null && RightHandController.Singleton.myOM.heldObject == null)
			{
                float xValue = xButton.action.ReadValue<float>();
                Debug.Log(xValue);
                if (xValue == 1)
                {
                    altControls = true;
                }
                else
                {
                    altControls = false;
                }

                checkAltControls();
            }
		}


		if (!TourMode.Singleton.getTourModeToggle())
		{
			if (altControls == false)
			{
                if (tValue > 0 || gValue > 0)
                {
                    //Debug.Log("Left \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
                }

                myOM.TryGrab(gValue);
                myOM.TryStretch(tValue);
            }
			else
			{
				if (tValue > .7)
				{
                    continousMove.enableFly = true;
                }
				else
				{
					continousMove.enableFly = false;
				}
				myOM.TryDuplicate(gValue);
			}


            
        }
		else
		{
            if (tValue > 0 || gValue > 0)
            {
				//Debug.Log("Left \nTrigger Value = " + tValue + "\n" + "Grip Value = " + gValue);
				TourMode.Singleton.BackToEditMode();
            }
        }
		
	}


	void checkAltControls()
	{
        if (altControls == true)
        {
            RightAltControlUI.SetActive(true);
            LeftAltControlUI.SetActive(true);
            RightControlUI.SetActive(false);
            LeftControlUI.SetActive(false);
        }
        else
        {
            RightAltControlUI.SetActive(false);
            LeftAltControlUI.SetActive(false);
            RightControlUI.SetActive(true);
            LeftControlUI.SetActive(true);
        }
    }

	public void xToggle(InputAction.CallbackContext context)
	{
		if (myOM.heldObject == null && RightHandController.Singleton.myOM.heldObject == null)
		{
            if (altControlsHold == false)
            {
                //Debug.Log("X button pressed.");
                if (!TourMode.Singleton.getTourModeToggle())
                {
                    if (altControls == true)
                    {
                        altControls = false;
                        RightAltControlUI.SetActive(false);
                        LeftAltControlUI.SetActive(false);
                        RightControlUI.SetActive(true);
                        LeftControlUI.SetActive(true);



                    }
                    else
                    {
                        altControls = true;
                        RightAltControlUI.SetActive(true);
                        LeftAltControlUI.SetActive(true);
                        RightControlUI.SetActive(false);
                        LeftControlUI.SetActive(false);

                    }
                }
                else
                {

                }
            }
        }
		
	}
	public void yToggle(InputAction.CallbackContext context)
	{
		if (!TourMode.Singleton.getTourModeToggle())
		{
            if (!altControls)
            {
                ObjectPainter.Singleton.PaintObject();
            }
            else
            {
				ObjectManipulator.TryGroup();
            }
        }
		else
		{
			TourMode.Singleton.BackToEditMode();
		}
    }

	public void menuToggle(InputAction.CallbackContext context)
	{
        MenuActions.Singleton.InteractWithMainMenu();
	}

	public void lStickClickToggle(InputAction.CallbackContext context)
	{
		BlockRangler.ActionHistory.UndoAction();
	}

	public GameObject GetLeftHandObject()
	{
		return cursor;
	}

	public bool getAltControl()
	{
		return altControls;
	}

	public void switchAltControlScheme()
	{
		if (altControlsHold == true)
		{
			altControlsHold = false;
		}
		else
		{
			altControlsHold = true;
		}
	}


}
