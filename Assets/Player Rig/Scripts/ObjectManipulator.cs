using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using System;

using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;

public class ObjectManipulator : MonoBehaviour
{
	[SerializeField] GameObject cursor;

	Vector3 cursorPosition => cursor.transform.position;
	readonly float grabTreshhold = 0.8f;

	float grabRadius;

	public GameObject heldObject { get; private set; }
	bool isHoldingDuplicate;
	bool triedToGrabAlready = false;
	bool hologramIsTempDisabled = false;

	//stretching
	bool triedToStretchAlready = false;
	private GameObject stretchingObject = null;
	Vector3 objectScaleAtStart = Vector3.one;
	Vector3 objectPositionAtStart = Vector3.zero;
	float XScalar = 0;
	float YScalar = 0;
	float ZScalar = 0;
	float startingScalarDot = 0;

	//grouping
	public static GameObject parentOfGroup {get; private set;}
	
	private void Awake()
	{
		parentOfGroup = null;
		heldObject = null;
		grabRadius = cursor.transform.localScale.x;
	}

	// Update is called once per frame
	void Update()
	{
		StretchObjectUpdate();
	}

	static public void TryGroup()
	{
		Transform leftHandObject = LeftHandController.Singleton.GetLeftHandObject().transform;
		Ray ray = new Ray(leftHandObject.position, leftHandObject.forward); //casts ray

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
            Effects.Singleton.PlaySound(RightHandController.Singleton.transform.position, 9);
            GameObject objectToAddToGroup = hit.collider.gameObject;
			if (objectToAddToGroup.GetComponent<BuildingBlockBehavior>())
			{
				if (parentOfGroup == null)
				{
					parentOfGroup = new GameObject();
					parentOfGroup.transform.position = objectToAddToGroup.transform.position;
					parentOfGroup.transform.rotation = objectToAddToGroup.transform.rotation;
					parentOfGroup.name = "Group";
				}

				List<GameObject> groupedObjects = new List<GameObject>();
				for (int thisObject = 0; thisObject < parentOfGroup.transform.childCount; thisObject++)
				{
					groupedObjects.Add(parentOfGroup.transform.GetChild(thisObject).gameObject);
				}

				objectToAddToGroup.transform.parent = parentOfGroup.transform;

				groupedObjects.Add(objectToAddToGroup);

				BlockRangler.ActionHistory.PushAddToGroupAction(groupedObjects);
			}
		}
	}

	public static void SetParentOfGroup(GameObject newParent)
	{
        if (parentOfGroup != null)
        {
            parentOfGroup.transform.DetachChildren();
            Destroy(parentOfGroup);
        }
		parentOfGroup = newParent;
    }

	public static void RemoveParentOfGroup()
	{
		if (parentOfGroup != null)
		{
			parentOfGroup.transform.DetachChildren();
			Destroy(parentOfGroup);
		}
	}

	public static List<GameObject> GetGroupedObjects()
	{
		if (parentOfGroup == null) return null;

		List<GameObject> groupedObjects = new List<GameObject>();
		for (int thisObject = 0; thisObject < parentOfGroup.transform.childCount; thisObject++)
		{
			groupedObjects.Add(parentOfGroup.transform.GetChild(thisObject).gameObject);
		}

		return groupedObjects;
	}

	static public void TryUngroup()
	{
		if (parentOfGroup != null)
		{
			List<GameObject> groupedObjects = new List<GameObject>();
			for (int thisObject = 0; thisObject < parentOfGroup.transform.childCount; thisObject++)
			{
				groupedObjects.Add(parentOfGroup.transform.GetChild(thisObject).gameObject);
			}
			BlockRangler.ActionHistory.PushUngroupAction(groupedObjects);
			parentOfGroup.transform.DetachChildren();
			Effects.Singleton.PlaySound(RightHandController.Singleton.transform.position, 10);
			Destroy(parentOfGroup);
		}
	}

	public void TryDuplicate(float grabValue)
	{
		if (heldObject == null && grabValue >= grabTreshhold && !triedToGrabAlready && stretchingObject == null)
		{
			triedToGrabAlready = true;

			Collider objectToDuplicateCollider = GetGrabbedCollider();

			if (objectToDuplicateCollider != null)
			{

				if (objectToDuplicateCollider.gameObject.transform.parent == null)
				{
					GameObject newBlock = ObjectCreator.Singleton.PlaceObject(objectToDuplicateCollider.gameObject);
					heldObject = newBlock;
					heldObject.transform.parent = transform;
				}
				else
				{
					GameObject parent = objectToDuplicateCollider.gameObject.transform.parent.gameObject;
					List<GameObject> theseChildren = new List<GameObject>();

					for (int childIndex = 0; childIndex < parent.transform.childCount; childIndex++)
                    {
                        Transform thisChild = parent.transform.GetChild(childIndex);
                        GameObject newChild = Instantiate(Resources.Load($"Blocks/{BlockRangler.SimplifyObjectName(thisChild.name)}", typeof(GameObject)), thisChild.position, thisChild.rotation) as GameObject;
						newChild.GetComponent<Renderer>().material = thisChild.GetComponent<Renderer>().material;
						newChild.transform.localScale = thisChild.localScale;
						theseChildren.Add(newChild);
                    }
					parentOfGroup.transform.DetachChildren();
					Vector3 oldParentScale = parentOfGroup.transform.localScale;
					parentOfGroup.transform.localScale = Vector3.one;
					parentOfGroup.transform.rotation = theseChildren[0].transform.rotation;
					parentOfGroup.transform.position = theseChildren[0].transform.position;
					for (int thisChild = 0; thisChild < theseChildren.Count; thisChild++)
					{
						theseChildren[thisChild].transform.parent = parentOfGroup.transform;
					}
					parent.transform.localScale = oldParentScale;

					heldObject = parentOfGroup;
					heldObject.transform.parent = transform;

					isHoldingDuplicate = true;
                }
                if (HologramDisplay.Singleton.GetHologramState())
                {
					HologramDisplay.Singleton.ToggleHologram();
					hologramIsTempDisabled = true;
				}
			}

		}
		else if (heldObject != null && grabValue < grabTreshhold)
		{
			// Object scaling code, uses the RoundForPlacementAssistance and RoundForRotation functions from the rightHandController's palette script
			float xPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.x);
			float yPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.y);
			float zPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.z);
			heldObject.transform.position = new Vector3(xPosition, yPosition, zPosition);
			Vector3 rotation = new Vector3(
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.x),
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.y),
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.z));
			heldObject.transform.rotation = Quaternion.Euler(rotation);

			BlockRangler.ActionHistory.PushCreateAction(heldObject);

			heldObject.transform.parent = null;
			heldObject = null;

			if (hologramIsTempDisabled)
			{
				HologramDisplay.Singleton.ToggleHologram();
				hologramIsTempDisabled = false;
			}
		}

		//if we've ungrabbed, get ready to try again
		if (grabValue < grabTreshhold) triedToGrabAlready = false;
	}

	private Collider GetGrabbedCollider()
	{
		List<Collider> possibleColliders = new(Physics.OverlapSphere(cursor.transform.position, grabRadius));

		//get ready to go through list
		float distanceToNearest = Mathf.Infinity;
		int indexOfNearest = -1;

		//go through list of near colliders and find the closest one with the Block tag
		foreach (Collider collider in possibleColliders)
		{
			//Debug.Log($"found a collider: {collider.gameObject.name}");
			if (collider.gameObject.GetComponent<BuildingBlockBehavior>() != null)
			{
				//Debug.Log($"found a block: {collider.gameObject.name}");
				float distanceToThis = Vector3.Distance(cursorPosition, collider.transform.position);
				if (distanceToThis < distanceToNearest)
				{
					distanceToNearest = distanceToThis;
					indexOfNearest = possibleColliders.IndexOf(collider);
				}
			}
		}
		if (indexOfNearest >= 0 &&
		indexOfNearest < possibleColliders.Count &&
		possibleColliders[indexOfNearest] != null)
		{
			return possibleColliders[indexOfNearest];
		}
		else
			return null;
	}

	public void TryStretch(float triggerValue)
	{

		if (stretchingObject == null && triggerValue >= grabTreshhold && !triedToStretchAlready && heldObject == null) {
		
			triedToStretchAlready = true;

			Collider stretchingCollider = GetGrabbedCollider();

			if (stretchingCollider != null) {

				if (stretchingCollider.transform.parent == null)
					stretchingObject = stretchingCollider.gameObject;
				else
					stretchingObject = stretchingCollider.transform.parent.gameObject;
				Vector3 objectToCursor = stretchingObject.transform.position - cursorPosition;
				BlockRangler.ActionHistory.PushMoveAction(stretchingObject);

				float closestDot = 0;

				float XDotProduct = Vector3.Dot(stretchingObject.transform.right, objectToCursor.normalized);
				float XScale = stretchingObject.transform.localScale.x;
				if (Mathf.Abs(XDotProduct) / XScale > closestDot)
				{
					closestDot = Mathf.Abs(XDotProduct) / XScale;
					startingScalarDot = Vector3.Dot(objectToCursor, stretchingObject.transform.right);
					XScalar = XDotProduct < 0 ? -1 : 1;
					YScalar = 0;
					ZScalar = 0;
				}
				float YDotProduct = Vector3.Dot(stretchingObject.transform.up, objectToCursor.normalized);
				float YScale = stretchingObject.transform.localScale.y;
				if (Mathf.Abs(YDotProduct) / YScale > closestDot)
				{
					closestDot = Mathf.Abs(YDotProduct) / YScale;
					startingScalarDot = Vector3.Dot(objectToCursor, stretchingObject.transform.up);
					XScalar = 0;
					YScalar = YDotProduct < 0 ? -1 : 1;
					ZScalar = 0;
				}
				float ZDotProduct = Vector3.Dot(stretchingObject.transform.forward, objectToCursor.normalized);
				float ZScale = stretchingObject.transform.localScale.z;
				if (Mathf.Abs(ZDotProduct) / ZScale > closestDot)
				{
					startingScalarDot = Vector3.Dot(objectToCursor, stretchingObject.transform.forward);
					XScalar = 0;
					YScalar = 0;
					ZScalar = ZDotProduct < 0 ? -1 : 1;
				}

				objectScaleAtStart = stretchingObject.transform.localScale;
				objectPositionAtStart = stretchingObject.transform.position;

				Debug.DrawRay(stretchingObject.transform.position, -objectToCursor, Color.red, 30f, false);

				if (HologramDisplay.Singleton.GetHologramState())
				{
					HologramDisplay.Singleton.ToggleHologram();
					hologramIsTempDisabled = true;
				}
			}
		}
		else if (stretchingObject != null && triggerValue < grabTreshhold)
		{
			stretchingObject = null;
			XScalar = 0;
			YScalar = 0;
			ZScalar = 0;

			if (hologramIsTempDisabled)
			{
				HologramDisplay.Singleton.ToggleHologram();
				hologramIsTempDisabled = false;
			}
		}

		if (triggerValue < grabTreshhold) triedToStretchAlready = false;
	}

	private void StretchObjectUpdate()
	{
		if (stretchingObject != null)
		{
			Vector3 objectToCursor = objectPositionAtStart - cursorPosition;
			stretchingObject.transform.localScale = CalculateStretchedScale(objectToCursor);

			Vector3 moveObjectTo = (objectPositionAtStart + (Vector3.Dot(objectToCursor, stretchingObject.transform.right) * stretchingObject.transform.right * -Mathf.Abs(XScalar) * 0.5f
				+ Mathf.Abs(XScalar) * stretchingObject.transform.right * startingScalarDot * 0.5f) +
				(Vector3.Dot(objectToCursor, stretchingObject.transform.up) * stretchingObject.transform.up * -Mathf.Abs(YScalar) * 0.5f
				+ Mathf.Abs(YScalar) * stretchingObject.transform.up * startingScalarDot * 0.5f) +
				(Vector3.Dot(objectToCursor, stretchingObject.transform.forward) * stretchingObject.transform.forward * -Mathf.Abs(ZScalar) * 0.5f
				+ Mathf.Abs(ZScalar) * stretchingObject.transform.forward * startingScalarDot * 0.5f));
			stretchingObject.transform.position = moveObjectTo;
		}
	}

	public void TryGrab(float grabValue)
	{
		if (heldObject == null && grabValue >= grabTreshhold && !triedToGrabAlready && stretchingObject == null)
		{
			triedToGrabAlready = true;

			Collider grabbedCollider = GetGrabbedCollider();

			if (grabbedCollider != null)
			{
				if (grabbedCollider.gameObject.transform.parent == null)
				{
					heldObject = grabbedCollider.gameObject;
					BlockRangler.ActionHistory.PushMoveAction(heldObject);
					heldObject.transform.parent = transform;
				}
				else
				{
					heldObject = grabbedCollider.gameObject.transform.parent.gameObject;
					heldObject.transform.parent = transform;
				}


				if (HologramDisplay.Singleton.GetHologramState())
				{
					HologramDisplay.Singleton.ToggleHologram();
					hologramIsTempDisabled = true;
				}
			}

		}
		else if (heldObject != null && grabValue < grabTreshhold)
		{
			// Object scaling code, uses the RoundForPlacementAssistance and RoundForRotation functions from the rightHandController's palette script
			float xPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.x);
			float yPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.y);
			float zPosition = ObjectCreator.Singleton.RoundForPlacementAssistance(heldObject.transform.position.z);
			heldObject.transform.position = new Vector3(xPosition, yPosition, zPosition);
			Vector3 rotation = new Vector3(
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.x),
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.y),
			ObjectCreator.Singleton.RoundForRotationAssistance(heldObject.transform.eulerAngles.z));
			heldObject.transform.rotation = Quaternion.Euler(rotation);

			if (heldObject.transform.childCount != 0)
			{
				if (isHoldingDuplicate)
					BlockRangler.ActionHistory.PushCreateAction(heldObject, GetGroupedObjects());
			} else
			{
				if (isHoldingDuplicate)
					BlockRangler.ActionHistory.PushCreateAction(heldObject);
			}

			isHoldingDuplicate = false;
			heldObject.transform.parent = null;
			heldObject = null;

			if (hologramIsTempDisabled)
			{
				HologramDisplay.Singleton.ToggleHologram();
				hologramIsTempDisabled = false;
			}
		}
		
		//if we've ungrabbed, get ready to try again
		if (grabValue < grabTreshhold) triedToGrabAlready = false;
	}

	// Used in StretchObject
	private Vector3 CalculateStretchedScale(Vector3 rawValue)
	{
		float unroundedXvalue = objectScaleAtStart.x + (Vector3.Dot(rawValue, stretchingObject.transform.right) - startingScalarDot) * XScalar;
		float unroundedYvalue = objectScaleAtStart.y + (Vector3.Dot(rawValue, stretchingObject.transform.up) - startingScalarDot) * YScalar;
		float unroundedZvalue = objectScaleAtStart.z + (Vector3.Dot(rawValue, stretchingObject.transform.forward) - startingScalarDot) * ZScalar;
		float roundedXvalue = ObjectCreator.Singleton.RoundForScalingAssistance(unroundedXvalue);
		float roundedYvalue = ObjectCreator.Singleton.RoundForScalingAssistance(unroundedYvalue);
		float roundedZvalue = ObjectCreator.Singleton.RoundForScalingAssistance(unroundedZvalue);
		return new Vector3(roundedXvalue, roundedYvalue, roundedZvalue);
	}
}
