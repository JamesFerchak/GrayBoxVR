using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class LevelSavedData
{
	public List<string> blockNames;
	public List<float> blockLocations;
	public List<float> blockRotations;
	public List<float> blockScales;
	public List<string> blockMaterials;

	public LevelSavedData()
	{

		blockNames = new();
		blockLocations = new();
		blockRotations = new();
		blockScales = new();
		blockMaterials = new();

		foreach (GameObject block in BlockRangler.Singleton.BlockList())
		{
			string nameToAdd;
			nameToAdd = block.name.Contains('(') ? block.name.Substring(0, (block.name.IndexOf('('))) : block.name;
			string materialToAdd;
			blockNames.Add(nameToAdd);
			string materialName = block.GetComponent<MeshRenderer>().material.name;
			materialToAdd = materialName.Contains(' ') ? 
				materialName.Substring(0, (materialName.IndexOf(' '))) :
				materialName;
			blockMaterials.Add(materialToAdd);

			blockLocations.Add(block.transform.position.x);
			blockLocations.Add(block.transform.position.y);
			blockLocations.Add(block.transform.position.z);

			blockRotations.Add(block.transform.rotation.x);
			blockRotations.Add(block.transform.rotation.y);
			blockRotations.Add(block.transform.rotation.z);
			blockRotations.Add(block.transform.rotation.w);

			blockScales.Add(block.transform.localScale.x);
			blockScales.Add(block.transform.localScale.y);
			blockScales.Add(block.transform.localScale.z);

			
		}

	}
}
