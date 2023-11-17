using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor.Build;
using UnityEditor;

public class BlockRangler : MonoBehaviour
{
	readonly static int changeHistorySize = 25;
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

	private class action
	{
		public GameObject myGameObject;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public bool wasDeleted;
		public bool wasCreated;

		public action(GameObject affectedObject)
		{
			myGameObject = affectedObject;
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			wasDeleted = false;
			wasCreated = false;
		}

		public action(GameObject affectedObject, bool deleted)
		{
			myGameObject = affectedObject;
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			wasDeleted = deleted;
			wasCreated = false;
		}

		public action(GameObject affectedObject, bool deleted, bool created)
		{
			myGameObject = affectedObject;
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			wasDeleted = deleted;
			wasCreated = created;
		}

	}

	public static class ChangeHistory
	{
		private static action[] actions = new action[changeHistorySize];
		private static Stack<action> undoneActions = new Stack<action>();
		private static int TopIndex = 0;

		static ChangeHistory()
		{
			for (int thisAction = 0; thisAction < actions.Length; thisAction++)
			{
				actions[thisAction] = null;
			}
		}
		
		//private
		private static void IncrementTopIndex()
		{
			TopIndex = TopIndex == changeHistorySize - 1 ? 0 : TopIndex + 1;
		}

		private static void DecrementTopIndex()
		{
			TopIndex = TopIndex == 0 ? changeHistorySize - 1 : TopIndex - 1;
		}
		
		private static void PushRedoneAction(GameObject objectToRecord)
		{
			IncrementTopIndex();
			action actionToPush = new action(objectToRecord);
			actions[TopIndex] = actionToPush;
		}

		//public
		public static void PushAction(GameObject objectToRecord)
		{
			IncrementTopIndex();
			action actionToPush = new action(objectToRecord);
			actions[TopIndex] = actionToPush;
			undoneActions.Clear();
		}

		public static void Undo()
		{
			GameObject objectToUndo = actions[TopIndex].myGameObject;
			action actionToUndo = actions[TopIndex];
			//this if statement will have to be changed/removed when we are undoing block creation/deletion
			if (objectToUndo != null && actionToUndo != null)
			{
				action undoneAction = new action(objectToUndo);
				undoneActions.Push(undoneAction);

				objectToUndo.transform.position = actionToUndo.position;
				objectToUndo.transform.rotation = actionToUndo.rotation;
				objectToUndo.transform.localScale = actionToUndo.scale;
			}
			actions[TopIndex] = null;
			DecrementTopIndex();
		}

		public static void Redo()
		{
			GameObject objectToRedo = actions[TopIndex].myGameObject;
			action actionToRedo = actions[TopIndex];
			//this if statement will have to be changed/removed when we are undoing block creation/deletion
			if (objectToRedo != null && actionToRedo != null)
			{
				PushRedoneAction(objectToRedo);

				objectToRedo.transform.position = actionToRedo.position;
				objectToRedo.transform.rotation = actionToRedo.rotation;
				objectToRedo.transform.localScale = actionToRedo.scale;
			}
			actions[TopIndex] = null;
			DecrementTopIndex();
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
			SaveLevel();

		if (Input.GetKeyDown(KeyCode.L))
			LoadLevel();
	}

	//COULD MAKE THIS TAKE A PARAM AS A LEVEL NAME
	public static void SaveLevel()
	{
		BinaryFormatter myFormatter = new();
		FileStream myStream = new(levelPath, FileMode.Create);

		LevelSavedData myData = new();

		myFormatter.Serialize(myStream, myData);
		myStream.Close();
	}

	//COULD MAKE THIS TAKE A PARAM AS A LEVEL NAME
	private static LevelSavedData GetLevelFromFile()
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
	public static void LoadLevel()
	{
		LevelSavedData levelToLoad = GetLevelFromFile();
		if (levelToLoad != null)
		{
			foreach (GameObject block in blocks)
			{
				Destroy(block);
			}
			foreach (string blockName in levelToLoad.blockNames)
			{
				//Debug.LogWarning($"loading block by name: {blockName}");
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
