using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectCreator : MonoBehaviour
{
    private static ObjectCreator _singleton;
    public static ObjectCreator Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one ObjectCreator! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public GameObject currentObjectType; // The type of "block" being placed (square, circle, etc.)
    public GameObject selectedObject; // The selected object in edit mode
    public GameObject cubePrefab; // Cube GameObject
    public GameObject spherePrefab; // Sphere GameObject
    public GameObject cylinderPrefab; // Cylinder GameObject
    public GameObject pyramidPrefab; // Pyramid GameObject
    public GameObject floorPrefab; // Cube GameObject
    public GameObject pillarPrefab; // Long pillar GameObject
    public GameObject shortPillarPrefab; // Short pillar GameObject
    public GameObject wallPrefab; // Wall GameObject

    public Material defaultMaterial; // Material for Cube GameObject
    public Material selectedMaterial; // Material for the selected GameObject

    public Vector3 savedHandPos; // Vector3 recording the hand position

    public Text placementAssistDegree;
    public Text rotationAssistDegree;
    public Text scalingAssistDegree;

    public float placementAssistValue;
    public float rotationAssistValue;
    public float scalingAssistValue;

    public bool inMoveMode; // True if there is currently a selected object in move mode
    public bool inEditMode; // True if there is currently a selected object in edit mode

    public HologramDisplay hologramDisplay;

    // Start is called before the first frame update
    void Start()
    {
        currentObjectType = cubePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        Transform RightHand = RightHandController.Singleton.GetRightHandObject().transform;

        Vector3 position = RightHand.position;
        position.x = RoundForPlacementAssistance(position.x);
        position.y = RoundForPlacementAssistance(position.y);
        position.z = RoundForPlacementAssistance(position.z);

        Vector3 rotation = new Vector3(
            RoundForRotationAssistance(RightHand.eulerAngles.x),
            RoundForRotationAssistance(RightHand.transform.eulerAngles.y),
            RoundForRotationAssistance(RightHand.transform.eulerAngles.z));

        hologramDisplay.ShowHologram(position, Quaternion.Euler(rotation));
    }

    public void PlaceObject()
    {
        Transform RightHand = RightHandController.Singleton.GetRightHandObject().transform;

        Vector3 position = RightHand.position;
        position.x = RoundForPlacementAssistance(position.x);
        position.y = RoundForPlacementAssistance(position.y);
        position.z = RoundForPlacementAssistance(position.z);

        Vector3 rotation = new Vector3(
            RoundForRotationAssistance(RightHand.eulerAngles.x),
            RoundForRotationAssistance(RightHand.transform.eulerAngles.y),
            RoundForRotationAssistance(RightHand.transform.eulerAngles.z));

        GameObject block = Instantiate(currentObjectType, position, Quaternion.Euler(rotation)); // Places cube in level
        block.tag = "Block";
        BlockRangler.ActionHistory.PushCreateAction(block);
    }

    public void EraseObject()
    {
        Transform RightHandObject = RightHandController.Singleton.GetRightHandObject().transform;
        Ray ray = new Ray(RightHandObject.position, RightHandObject.forward);

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

    public void ChangeToShape(string shapeID)
    {
        switch (shapeID)
        {
            case ("cube"):
                currentObjectType = cubePrefab;
                break;
            case ("sphere"):
                currentObjectType = spherePrefab;
                break;
            case ("cylinder"):
                currentObjectType = cylinderPrefab;
                break;
            case ("pyramid"):
                currentObjectType = pyramidPrefab;
                break;
            case ("floor"):
                currentObjectType = floorPrefab;
                break;
            case ("pillar"):
                currentObjectType = pillarPrefab;
                break;
            case ("shortpillar"):
                currentObjectType = shortPillarPrefab;
                break;
            case ("wall"):
                currentObjectType = wallPrefab;
                break;
            default:
                currentObjectType = cubePrefab;
                break;
        }
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
