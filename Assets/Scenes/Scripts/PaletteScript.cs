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

    public Material defaultMaterial; // Material for Cube GameObject
    public Material selectedMaterial; // Material for the selected GameObject
    public Vector3 savedHandPos; // Vector3 recording the hand position

    public GameObject mainMenuPanel; // Panel for the main menu
    public bool inMenuMode; // True if the menu is open
    public Text placementAssistDegree;
    public Text rotationAssistDegree;
    public Text scalingAssistDegree;

    public bool inMoveMode; // True if there is currently a selected object in move mode
    public bool inEditMode; // True if there is currently a selected object in edit mode

    // Start is called before the first frame update
    void Start()
    {
        currentObjectType = cubePrefab;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaceObject()
    {
        Vector3 position = gameObject.transform.position; // Needs to record location of block placement instead

        Vector3 rotation = new Vector3(0, gameObject.transform.rotation.y * 90, 0); // Places block flat, only keeping y rotation of controller

        GameObject block = Instantiate(currentObjectType.gameObject, position, Quaternion.Euler(rotation)); // Places cube in level
        block.tag = "Block";
    }

    public void EraseObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Destroy the hit object
            Destroy(hit.transform.gameObject);
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
}
