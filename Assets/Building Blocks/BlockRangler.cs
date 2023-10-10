using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.Build;

public class BlockRangler : MonoBehaviour
{

	public static string levelPath;

	private static BlockRangler _singleton;
    public static BlockRangler Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one Block Rangler! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    private static List<GameObject> blocks = new List<GameObject>();

    public List<GameObject> BlockList()
    {
        return blocks;
    }

    public void AddToBlockList(GameObject blockToAdd)
    {
        blocks.Add(blockToAdd);
    }

    public void RemoveFromBlockList(GameObject blockToRemove)
    {
        blocks.Remove(blockToRemove);
        Debug.Log($"{blockToRemove.name} removed from list. he was at {blockToRemove.transform.position}");
    }

	private void Awake()
	{
        Singleton = this;
		levelPath = Application.persistentDataPath + "/SavedLevel.kek";
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SavePlayerLevel();

        if (Input.GetKeyDown(KeyCode.L))
            BuildLevelFromSave();
    }

	//COULD MAKE THIS TAKE A PARAM AS A LEVEL NAME
	private static void SavePlayerLevel()
	{
		BinaryFormatter myFormatter = new();
		FileStream myStream = new(levelPath, FileMode.Create);

		LevelSavedData myData = new();

		myFormatter.Serialize(myStream, myData);
		myStream.Close();
	}

	//COULD MAKE THIS TAKE A PARAM AS A LEVEL NAME
	private static LevelSavedData LoadPlayerLevel()
    {
        if (File.Exists(levelPath))
        {
            BinaryFormatter myFormatter = new();
            FileStream myStream = new(levelPath, FileMode.Open);

            LevelSavedData myData = myFormatter.Deserialize(myStream) as LevelSavedData;
            myStream.Close();

            return myData;
        }
        else
        {
            Debug.LogError($"Tried to load level that doesn't exist!!! {levelPath}");
            return null;
        }
    }

    //COULD MAKE THIS TAKE A PARAM AS A LEVEL NAME
    private void BuildLevelFromSave()
    {
        LevelSavedData levelToLoad = LoadPlayerLevel();
        if (levelToLoad != null)
        {
            foreach (GameObject block in blocks)
            {
                Destroy(block);
            }
            foreach (string blockName in levelToLoad.blockNames)
            {
                Debug.LogWarning($"loading block by name: {blockName}");
                //spawn object
				GameObject instance = Instantiate(Resources.Load($"Blocks/{blockName}", typeof(GameObject))) as GameObject;

                //set location
                instance.transform.position = new Vector3(levelToLoad.blockLocations[0], levelToLoad.blockLocations[1], levelToLoad.blockLocations[2]);
                levelToLoad.blockLocations.RemoveRange(0, 3);

                //set rotation
                instance.transform.rotation = new Quaternion(levelToLoad.blockRotations[0], levelToLoad.blockRotations[1], levelToLoad.blockRotations[2], levelToLoad.blockRotations[3]);
                levelToLoad.blockRotations.RemoveRange(0, 4);

                //set scale
                instance.transform.localScale = new Vector3(levelToLoad.blockScales[0], levelToLoad.blockScales[1], levelToLoad.blockScales[2]);
                levelToLoad.blockScales.RemoveRange(0, 3);
			}
        }

    }
}
