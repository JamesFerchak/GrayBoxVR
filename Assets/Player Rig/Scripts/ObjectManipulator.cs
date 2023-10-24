using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{

	[SerializeField] GameObject cursor;
	readonly float grabTreshhold = 0.8f;

	private GameObject currentlyHeldObject = null;
	bool triedToGrabAlready = false;
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
		MoveGrabbedObject();
	}

	public void TryGrab(float grabValue)
	{
		if (currentlyHeldObject == null && grabValue >= grabTreshhold && !triedToGrabAlready)
		{
			//mark that we already tried to grab
			triedToGrabAlready = true;

			List<Collider> possibleColliders = new(Physics.OverlapSphere(cursor.transform.position, .05f));

			//get ready to go through list
			float distanceToNearest = Mathf.Infinity;
			int indexOfNearest = -1;

			//go through list of near colliders and find the closest one with the Block tag
			foreach (Collider collider in possibleColliders)
			{
				Debug.Log($"found a collider: {collider.gameObject.name}");
				if (collider.gameObject.GetComponent<BuildingBlockBehavior>() != null)
				{
					Debug.Log($"found a block: {collider.gameObject.name}");
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

			//if there is a viable collider, make it the grabbed object
			if (indexOfNearest >= 0 && 
				indexOfNearest < possibleColliders.Count && 
				possibleColliders[indexOfNearest] != null)
			{
				currentlyHeldObject = possibleColliders[indexOfNearest].gameObject;
				contStartPos = cursor.transform.position;
				objStartPos = currentlyHeldObject.transform.position;
			}
		}
		//if we're holing something and we aren't holding the grab button, ungrab
		else if (currentlyHeldObject != null && grabValue < grabTreshhold)
		{
			currentlyHeldObject = null;
		}

		//if we've ungrabbed, get ready to try again
		if (grabValue < grabTreshhold) triedToGrabAlready = false;
	}

	private void MoveGrabbedObject()
	{
		if (currentlyHeldObject != null)
		{
			Vector3 positionToAdd = cursor.transform.position - contStartPos;
			currentlyHeldObject.transform.position = objStartPos + positionToAdd;

			currentlyHeldObject.transform.rotation = contStartRot * transform.rotation;
			currentlyHeldObject.transform.rotation *= Quaternion.Inverse(contStartRot);

			//currentlyHeldObject.transform.localEulerAngles = contLastRot.eulerAngles - transform.rotation.eulerAngles;

			/*Quaternion rotationToAdd = Quaternion.Inverse(contStartRot) * transform.rotation;
			currentlyHeldObject.transform.rotation = currentlyHeldObject.transform.rotation * rotationToAdd;*/
		}
		contLastRot = transform.rotation;
	}
}
