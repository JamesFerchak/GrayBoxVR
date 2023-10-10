using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockBehavior : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		if (BlockRangler.Singleton != null)
			BlockRangler.Singleton.AddToBlockList(gameObject);
		else
			Debug.LogWarning("THERE'S NO BLOCK RANGLER HERE!!!");
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	private void OnDestroy()
	{
		BlockRangler.Singleton.RemoveFromBlockList(gameObject);
	}
}
