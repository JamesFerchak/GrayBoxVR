using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManipulator : MonoBehaviour
{

	[SerializeField] GameObject cursor;
	readonly float grabTreshhold = 0.8f;

	private GameObject currentlyHeldObject = null;

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void TryGrab(float grabValue)
	{
		List<Collider> possibleColliders = new(Physics.OverlapSphere(cursor.transform.position, .05f));
		float distanceToNearest = Mathf.Infinity;
		int indexOfNearest = -1;

		//go through list of colliders and 
		foreach (Collider collider in possibleColliders)
		{
			if (collider.gameObject.tag == "Block")
			{
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

		if (possibleColliders[indexOfNearest] != null)
			currentlyHeldObject = possibleColliders[indexOfNearest].gameObject;
	}
}
