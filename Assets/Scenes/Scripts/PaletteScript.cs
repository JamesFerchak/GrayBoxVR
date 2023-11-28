using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PaletteScript : MonoBehaviour
{
    public GameObject currentObjectType; // The type of "block" being placed (square, circle, etc.)
    public GameObject cubePrefab; // Cube GameObject
    public GameObject spherePrefab; // Sphere GameObject
    public GameObject cylinderPrefab; // Cylinder GameObject
    public GameObject pyramidPrefab; // Pyramid GameObject
    public GameObject floorPrefab; // Cube GameObject
    public GameObject pillarPrefab; // Long pillar GameObject
    public GameObject shortPillarPrefab; // Short pillar GameObject
    public GameObject wallPrefab; // Wall GameObject
    public GameObject selectedObject; // The selected object in edit mode
    [SerializeField] GameObject cursor;
    Vector3 cursorPosition => cursor.transform.position;

    public Material defaultMaterial; // Material for Cube GameObject
    public Material selectedMaterial; // Material for the selected GameObject
    public string current_wrap;
    public Material red;
    public Material blue;
    public Material white;
    public Material black;
    public Material brown;
    public Material purple;
    public Material green;
    public Material yellow;
    public Material orange;


    public Vector3 savedHandPos; // Vector3 recording the hand position

    public GameObject mainMenuPanel; // Panel for the main menu
    public bool inMenuMode; // True if the menu is open
    public Text placementAssistDegree;
    public Text rotationAssistDegree;
    public Text scalingAssistDegree;

    public float placementAssistValue;
    public float rotationAssistValue;
    public float scalingAssistValue;

    public bool inMoveMode; // True if there is currently a selected object in move mode
    public bool inEditMode; // True if there is currently a selected object in edit mode
    public bool isRightHandController;

    public Vector3 position;
    public Vector3 rotation;

    public HologramDisplay hologramDisplay;

    // Start is called before the first frame update
    void Start()
    {
        currentObjectType = cubePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRightHandController)
        {
            position = cursorPosition;
            position.x = RoundForPlacementAssistance(position.x);
            position.y = RoundForPlacementAssistance(position.y);
            position.z = RoundForPlacementAssistance(position.z);

            rotation = new Vector3(
                RoundForRotationAssistance(gameObject.transform.eulerAngles.x),
                RoundForRotationAssistance(gameObject.transform.eulerAngles.y),
                RoundForRotationAssistance(gameObject.transform.eulerAngles.z));

            hologramDisplay.ShowHologram(position, Quaternion.Euler(rotation));
        }
    }

    public void PlaceObject()
    {
        GameObject block = Instantiate(currentObjectType.gameObject, position, Quaternion.Euler(rotation)); // Places cube in level
        block.tag = "Block";
        BlockRangler.ActionHistory.PushCreateAction(block);
    }

    public void EraseObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Destroy the hit object
            if (hit.transform.gameObject.GetComponent<BuildingBlockBehavior>() != null)
            {
                BlockRangler.ActionHistory.PushDeleteAction(hit.transform.gameObject);
                Destroy(hit.transform.gameObject);
            }
        }
    }

    public void PaintObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward); //casts ray

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Destroy the hit object
            if (hit.transform.gameObject.GetComponent<BuildingBlockBehavior>() != null)
            {
                switch (current_wrap)
                {
                    case "red":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = red;
                        break;
                    case "blue":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = blue;
                        break;
                    case "yellow":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = yellow;
                        break;
                    case "white":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = white;
                        break;
                    case "black":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = black;
                        break;
                    case "green":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = green;
                        break;
                    case "brown":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = brown;
                        break;
                    case "orange":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = orange;
                        break;
                    case "purple":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = purple;
                        break;
                }
            }
                
        }
    }

    public void MoveObject()
    {
        if (inEditMode) // If user hasn't finished using edit mode yet
        {
            inEditMode = false;
            selectedObject.GetComponent<MeshRenderer>().material = defaultMaterial; // Reset the selected object's material
        }

        if (!inMoveMode) // If entering move mode
        {
            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            inMoveMode = true;
            if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
            {
                selectedObject = hit.transform.gameObject; // Select the hit object
                selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial; // Change the hit object's material
            }
            else // If nothing in line of sight
            {
                inMoveMode = false;
            }
        }
        else // If exiting move mode
        {
            selectedObject.transform.position = gameObject.transform.position; // Move selected object to hand location
            selectedObject.GetComponent<MeshRenderer>().material = defaultMaterial; // Reset the selected object's material
            selectedObject = null; // Remove any selected object that may be there
            inMoveMode = false; // Reset bool variables so you can re-use moving function
        }
    }

    public void EditObject()
    {
        if (inMoveMode) // If user hasn't finished using move mode yet
        {
            inMoveMode = false;
            selectedObject.GetComponent<MeshRenderer>().material = defaultMaterial; // Reset the selected object's material
        }

        if (!inEditMode) // If entering edit mode
        {
            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            inEditMode = true;
            if (Physics.Raycast(ray, out RaycastHit hit)) // If there is an object in line of sight
            {
                selectedObject = hit.transform.gameObject; // Select the hit object
                selectedObject.GetComponent<MeshRenderer>().material = selectedMaterial; // Change the hit object's material
                savedHandPos = gameObject.transform.position; // Record current position of hand
            }
            else // If nothing in line of sight
            {
                inEditMode = false;
            }
        }
        else // If exiting edit mode
        {
            selectedObject.transform.localScale = (savedHandPos - gameObject.transform.position); // Change scale of object based on hand movement
            selectedObject.GetComponent<MeshRenderer>().material = defaultMaterial; // Reset the selected object's material
            selectedObject = null; // Remove any selected object that may be there
            inEditMode = false; // Reset bool variables so you can re-use edit function
        }
    }

    public void InteractWithMainMenu()
    {
        if (mainMenuPanel != null) // If panel exists
        {
            if (inMenuMode) // If panel is already open
            {
                mainMenuPanel.SetActive(false); // Close panel
                inMenuMode = false;
            }
            else // If panel is closed
            {
                MenuActions.Singleton.RelocateMainMenu();
                mainMenuPanel.SetActive(true); // Open panel
                inMenuMode = true;
            }
        }
    }

    public void ChangeToSquare()
    {
        currentObjectType = cubePrefab;
    }

    public void ChangeToSphere()
    {
        currentObjectType = spherePrefab;
    }

    public void ChangeToCylinder()
    {
        currentObjectType = cylinderPrefab;
    }

    public void ChangeToPyramid()
    {
        currentObjectType = pyramidPrefab;
    }

    public void ChangeToFloor()
    {
        currentObjectType = floorPrefab;
    }

    public void ChangeToPillar()
    {
        currentObjectType = pillarPrefab;
    }

    public void ChangeToShortPillar()
    {
        currentObjectType = shortPillarPrefab;
    }

    public void ChangeToWall()
    {
        currentObjectType = wallPrefab;
    }

    public float RoundForPlacementAssistance(float realPosition)
    {
        placementAssistValue = float.Parse(placementAssistDegree.text);
        if (placementAssistValue == 0.0f)
        {
            return realPosition;
        }

        float difference = realPosition % placementAssistValue; // Gets the difference between the actual position and the next piece of the grid
        realPosition = realPosition - difference; // Snaps to next lowest grid position

        if (Mathf.Abs(difference) > (placementAssistValue / 2)) // If the real position is closer to the next highest position, snap up
        {
            if (realPosition >= 0)
            {
                realPosition = realPosition + placementAssistValue;
            }
            else // If on the negative side, snap down
            {
                realPosition = realPosition - placementAssistValue;
            }
        }

        return realPosition;
    }

    public float RoundForRotationAssistance(float realRotation)
    {
        rotationAssistValue = float.Parse(rotationAssistDegree.text);

        if (rotationAssistValue == 0.0f)
        {
            return realRotation;
        }

        float difference = realRotation % rotationAssistValue; // Gets the difference between the actual rotation and the next snap value
        realRotation = realRotation - difference; // Snaps to next lowest snap rotation

        if (Mathf.Abs(difference) > (rotationAssistValue / 2)) // If the real rotation is closer to the next highest rotation, snap up
        {
            realRotation = realRotation + rotationAssistValue;
        }

        return realRotation;
    }

    public float RoundForScalingAssistance(float realScale)
    {
        scalingAssistValue = float.Parse(scalingAssistDegree.text);
        if (scalingAssistValue == 0.0f)
        {
            return realScale;
        }

        float difference = realScale % scalingAssistValue; // Gets the difference between the actual scale and the next snap scale
        realScale = realScale - difference; // Snaps to next lowest scale

        if (Mathf.Abs(difference) > (scalingAssistValue / 2)) // If the real position is closer to the next highest position, snap up
        {
            if (realScale >= 0)
            {
                realScale = realScale + scalingAssistValue;
            }
            else // If on the negative side, snap down
            {
                realScale = realScale - scalingAssistValue;
            }
        }

        return realScale;
    }
}
