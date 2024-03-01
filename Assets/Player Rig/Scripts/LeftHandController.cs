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

	ObjectManipulator myOM;

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
	public bool altControls;

    //  -------------------------------------------------------------------------------------------------------------------  
    // Start is called before the first frame update
    public void Awake() // tutorial used awake so I did as well. Not sure yet if start changes how it works at all.
	{
		Singleton = this;
		xButton.action.started += xToggle; // How the a button is gets its pressed detected.
		yButton.action.started += yToggle;
		lStickClick.action.started += lStickClickToggle;
		menu.action.started += menuToggle;
		myOM = GetComponent<ObjectManipulator>();
		if (myOM == null) Debug.LogError("NO OBJECT MANIPULATOR ON THIS SCRIPT!!!");
	}

	// Update is called once per frame
	void Update()
	{
		float tValue = lTrigger.action.ReadValue<float>();
		float gValue = lGrip.action.ReadValue<float>();

		if (!TourMode.Singleton.getTourModeToggle())
		{
			if (LeftHandController.Singleton.altControls == false)
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

	public void xToggle(InputAction.CallbackContext context)
	{
		//Debug.Log("X button pressed.");
		if (!TourMode.Singleton.getTourModeToggle())
		{
			if (altControls == true)
			{
				altControls = false;
			}
			else
			{
				altControls = true;
			} 
		}
		else
		{

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
				myOM.TryGroup();
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

}
