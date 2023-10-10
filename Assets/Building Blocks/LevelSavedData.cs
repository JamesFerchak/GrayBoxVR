using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelSavedData
{
	public List<string> blockNames;
	public List<float> blockLocations;
	public List<float> blockRotations;
	public List<float> blockScales;

	public LevelSavedData()
	{

		blockNames = new();
		blockLocations = new();
		blockRotations = new();
		blockScales = new();

		foreach (GameObject block in BlockRangler.Singleton.BlockList())
		{
			blockNames.Add(block.name);

			blockLocations.Add(block.transform.position.x);
			blockLocations.Add(block.transform.position.y);
			blockLocations.Add(block.transform.position.z);

			blockRotations.Add(block.transform.rotation.w);
			blockRotations.Add(block.transform.rotation.x);
			blockRotations.Add(block.transform.rotation.y);
			blockRotations.Add(block.transform.rotation.z);

			blockScales.Add(block.transform.localScale.x);
			blockScales.Add(block.transform.localScale.y);
			blockScales.Add(block.transform.localScale.z);
		}

	}
}
