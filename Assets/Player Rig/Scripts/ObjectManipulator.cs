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

public class ObjectManipulator : MonoBehaviour
{

	[SerializeField] GameObject cursor;
	[SerializeField] GameObject rightHandController;

	Vector3 cursorPosition => cursor.transform.position;
	readonly float grabTreshhold = 0.8f;

	float grabRadius;

	private GameObject heldObject = null;
	bool triedToGrabAlready = false;
	//Vector3 cursorStartPosition = Vector3.zero;
	//Quaternion controllerStartRotation = Quaternion.identity;
	//Vector3 objectStartPosition = Vector3.zero;
	//Quaternion objectStartRotation = Quaternion.identity;
	//Quaternion controllerLastRotation = Quaternion.identity;
	
	//stretching
	bool triedToStretchAlready = false;
	private GameObject stretchingObject = null;
	Vector3 objectToCursorAtStart = Vector3.zero;
	Vector3 objectScaleAtStart = Vector3.one;
	Vector3 objectPositionAtStart = Vector3.zero;
	float XScalar = 0;
	float YScalar = 0;
	float ZScalar = 0;
	float startingScalarDot = 0;

	private void Awake()
	{
		grabRadius = cursor.transform.localScale.x;
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		StretchObject();
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

		if (stretchingObject == null && triggerValue >= grabTreshhold && !triedToStretchAlready) {
		
			triedToStretchAlready = true;

			Collider stretchingCollider = GetGrabbedCollider();

			if (stretchingCollider != null) {

				stretchingObject = stretchingCollider.gameObject;
				Vector3 objectToCursor = stretchingObject.transform.position - cursorPosition;

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

				objectToCursorAtStart = objectToCursor;
				objectScaleAtStart = stretchingObject.transform.localScale;
				objectPositionAtStart = stretchingObject.transform.position;

				Debug.DrawRay(stretchingObject.transform.position, -objectToCursor, Color.red, 30f, false);

			}
		}
		else if (stretchingObject != null && triggerValue < grabTreshhold)
		{
			stretchingObject = null;
			XScalar = 0;
			YScalar = 0;
			ZScalar = 0;
		}

		if (triggerValue < grabTreshhold) triedToStretchAlready = false;
	}

	private void StretchObject()
	{
		if (stretchingObject != null)
		{
			Vector3 objectToCursor = objectPositionAtStart - cursorPosition;
			//objectToCursor = stretchingObject.transform.rotation * objectToCursor;

			Debug.DrawRay(stretchingObject.transform.position, -objectToCursor, Color.green, 30f, false);

			Vector3 setScaleTo = new Vector3(
				rightHandController.gameObject.GetComponent<PaletteScript>().RoundForScalingAssistance(objectScaleAtStart.x + (objectToCursor.x - objectToCursorAtStart.x) * XScalar),
				rightHandController.gameObject.GetComponent<PaletteScript>().RoundForScalingAssistance(objectScaleAtStart.y + (objectToCursor.y - objectToCursorAtStart.y) * YScalar),
				rightHandController.gameObject.GetComponent<PaletteScript>().RoundForScalingAssistance(objectScaleAtStart.z + (objectToCursor.z - objectToCursorAtStart.z) * ZScalar));

			//overwriting setScaleTo for testing
			setScaleTo = new Vector3(
				objectScaleAtStart.x + (Vector3.Dot(objectToCursor, stretchingObject.transform.right) - startingScalarDot) * XScalar,
				objectScaleAtStart.y + (Vector3.Dot(objectToCursor, stretchingObject.transform.up) - startingScalarDot) * YScalar,
				objectScaleAtStart.z + (Vector3.Dot(objectToCursor, stretchingObject.transform.forward) - startingScalarDot) * ZScalar);
			stretchingObject.transform.localScale = setScaleTo;

			Vector3 moveObjectTo = new Vector3(
				objectPositionAtStart.x + (objectToCursor.x - objectToCursorAtStart.x) * 0.5f * -Mathf.Abs(XScalar),
				objectPositionAtStart.y + (objectToCursor.y - objectToCursorAtStart.y) * 0.5f * -Mathf.Abs(YScalar),
				objectPositionAtStart.z + (objectToCursor.z - objectToCursorAtStart.z) * 0.5f * -Mathf.Abs(ZScalar));

			/*moveObjectTo = objectPositionAtStart +
			((stretchingObject.transform.right * XScalar) +
			(stretchingObject.transform.up * YScalar) +
			(stretchingObject.transform.forward * ZScalar)) * objectToCursor.magnitude;*/
			//stretchingObject.transform.position = moveObjectTo;

			// NOTE: COULD add RoundForPlacementAssistance to moveObjectTo code - Peter
		}
	}



	public void TryGrab(float grabValue)
	{
		if (heldObject == null && grabValue >= grabTreshhold && !triedToGrabAlready)
		{
			triedToGrabAlready = true;

			Collider grabbedCollider = GetGrabbedCollider();

			if (grabbedCollider != null)
			{
				heldObject = grabbedCollider.gameObject;
				heldObject.transform.parent = transform;
			}

		}
		else if (heldObject != null && grabValue < grabTreshhold)
		{
			// Object scaling code, uses the RoundForPlacementAssistance and RoundForRotation functions from the rightHandController's palette script
            float xPosition = rightHandController.gameObject.GetComponent<PaletteScript>().RoundForPlacementAssistance(heldObject.transform.position.x);
            float yPosition = rightHandController.gameObject.GetComponent<PaletteScript>().RoundForPlacementAssistance(heldObject.transform.position.y);
			float zPosition = rightHandController.gameObject.GetComponent<PaletteScript>().RoundForPlacementAssistance(heldObject.transform.position.z);
			heldObject.transform.position = new Vector3(xPosition, yPosition, zPosition);
            Vector3 rotation = new Vector3(
				rightHandController.gameObject.GetComponent<PaletteScript>().RoundForRotationAssistance(heldObject.transform.rotation.x * 90),
            rightHandController.gameObject.GetComponent<PaletteScript>().RoundForRotationAssistance(heldObject.transform.rotation.y * 90),
            rightHandController.gameObject.GetComponent<PaletteScript>().RoundForRotationAssistance(heldObject.transform.rotation.z * 90));
			heldObject.transform.rotation = Quaternion.Euler(rotation);

			heldObject.transform.parent = null;
			heldObject = null;
		}

		//if we've ungrabbed, get ready to try again
		if (grabValue < grabTreshhold) triedToGrabAlready = false;
	}

}
