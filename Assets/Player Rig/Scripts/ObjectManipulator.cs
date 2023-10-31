using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ObjectManipulator : MonoBehaviour
{

	[SerializeField] GameObject cursor;
	readonly float grabTreshhold = 0.8f;

	private GameObject currentlyHeldObject = null;
	private GameObject currentlyStretchingObject = null;
	bool triedToGrabAlready = false;
	bool triedToStretchAlready = false;
	Vector3 contStartPos = Vector3.zero;
	Quaternion contStartRot = Quaternion.identity;
	Vector3 objStartPos = Vector3.zero;
	Quaternion objStartRot = Quaternion.identity;
	Quaternion contLastRot = Quaternion.identity;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}



	private Collider GetGrabbedCollider()
	{
		List<Collider> possibleColliders = new(Physics.OverlapSphere(cursor.transform.position, .05f));

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
				float distanceToThis = Vector3.Distance(cursor.transform.position, collider.transform.position);
				if (distanceToThis < distanceToNearest)
				{
					distanceToNearest = distanceToThis;
					indexOfNearest = possibleColliders.IndexOf(collider);
				}
				else
				{
					possibleColliders.Remove(collider);
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
		triedToStretchAlready = true;

		if (currentlyStretchingObject == null && triggerValue >= grabTreshhold && !triedToStretchAlready)
		{
			triedToStretchAlready = true;

			Collider stretchingCollider = GetGrabbedCollider();

			if (stretchingCollider != null) {
				currentlyStretchingObject = stretchingCollider.gameObject;

				Debug.Log($"Man, I sure am stretching rn. {currentlyStretchingObject.transform.localPosition - transform.localPosition}");
			}
		}
		else if (currentlyStretchingObject != null && triggerValue < grabTreshhold)
		{
			currentlyStretchingObject = null;
		}

		if (triggerValue < grabTreshhold) triedToStretchAlready = false;
	}



	public void TryGrab(float grabValue)
	{
		if (currentlyHeldObject == null && grabValue >= grabTreshhold && !triedToGrabAlready)
		{
			triedToGrabAlready = true;

			Collider grabbedCollider = GetGrabbedCollider();

			if (grabbedCollider != null)
			{
				currentlyHeldObject = grabbedCollider.gameObject;
				currentlyHeldObject.transform.parent = transform;
			}

		}
		else if (currentlyHeldObject != null && grabValue < grabTreshhold)
		{
			currentlyHeldObject.transform.parent = null;
			currentlyHeldObject = null;
		}

		//if we've ungrabbed, get ready to try again
		if (grabValue < grabTreshhold) triedToGrabAlready = false;
	}

}
