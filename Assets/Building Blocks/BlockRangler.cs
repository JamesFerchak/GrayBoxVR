using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UIElements;

public class BlockRangler : MonoBehaviour
{
	readonly static int actionHistorySize = 50;
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

	#region Undo/Redo

	private enum actionType
	{
		Move,
		Create,
		Delete,
		MaterialChange
	}

	private class Action
	{
		public GameObject myGameObject;
		public string gameObjectName;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 scale;
		public Material material;
		public actionType actionType;

		public Action(GameObject affectedObject)
		{
			myGameObject = affectedObject;
			gameObjectName = SimplifyObjectName(affectedObject.name);
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			material = affectedObject.GetComponent<Renderer>().material;
			actionType = actionType.Move;
		}

		public Action(GameObject affectedObject, actionType thisActionType)
		{
			myGameObject = affectedObject;
			gameObjectName = SimplifyObjectName(affectedObject.name);
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			material = affectedObject.GetComponent<Renderer>().material;
			actionType = thisActionType;
		}

		//this takes the name of an individual object and returns the name of the prefab it came from
		private static string SimplifyObjectName(string input)
		{
			return input.Contains('(') ? input.Substring(0, (input.IndexOf('('))) : input;
		}

	}



	public static class ActionHistory
	{
		private static Action[] actions = new Action[actionHistorySize];
		private static int[] actionObjectIDs = new int[actionHistorySize];
		private static int TopIndex = 0;
		private static int TopIndexPlusOne => TopIndex == actionHistorySize - 1 ? 0 : TopIndex + 1;
		private static int TopIndexMinusOne => TopIndex == 0 ? actionHistorySize - 1 : TopIndex - 1;
		private static int BottomIndex = 1;

		static ActionHistory()
		{
			for (int thisAction = 0; thisAction < actions.Length; thisAction++)
			{
				actions[thisAction] = null;
			}
		}
		
		//private
		private static void IncrementTopIndex()
		{
			TopIndex = TopIndexPlusOne;
		}

		private static void DecrementTopIndex()
		{
			TopIndex = TopIndexMinusOne;
		}

		private static void IncrementBothIndices()
		{
			TopIndex = TopIndexPlusOne;
			BottomIndex = TopIndexPlusOne;
		}
		
		private static void PushAction(Action actionToRecord, GameObject objectToRecord)
		{
			SetActionObject(objectToRecord);
			actions[TopIndex] = actionToRecord;
		}

		private static void SetActionObject(GameObject objectToRecord)
		{
			int objectToReplaceID = actionObjectIDs[TopIndex];
            actionObjectIDs[TopIndex] = objectToRecord.GetInstanceID();

			if (objectToReplaceID == 0)
			{
				return;
			}

			if (actions[TopIndex] != null)
			{
				if (actions[TopIndex].myGameObject == null)
				{
					for (int objectIndex = 0; objectIndex < actionHistorySize; objectIndex++)
					{
						if (actionObjectIDs[objectIndex] == objectToReplaceID)
						{
							actionObjectIDs[objectIndex] = objectToRecord.GetInstanceID();
							actions[objectIndex].myGameObject = objectToRecord;
						}
					}
				}
			}
		}

		//public
		public static void PushMoveAction(GameObject objectToRecord)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord);
			PushAction(actionToPush, objectToRecord);
		}

		public static void PushCreateAction(GameObject objectToRecord)
		{
            IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Create);
			PushAction(actionToPush, objectToRecord); 
		}

		public static void PushDeleteAction(GameObject objectToRecord)
		{
            IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Delete);
			PushAction(actionToPush, objectToRecord); 
		}

		public static void PushMaterialAction(GameObject objectToRecord)
		{
            IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.MaterialChange);
			PushAction(actionToPush, objectToRecord); 
		}

		public static void UndoAction()
		{
			if (actions[TopIndex] == null)
				return;

			DoInverseAction(TopIndex);

			DecrementTopIndex();
		}

		public static void RedoAction()
		{
			if (TopIndex + 1 == BottomIndex || (TopIndex == actionHistorySize - 1 && BottomIndex == 0))
				return;

			IncrementTopIndex();
			DoInverseAction(TopIndex);
		}

		private static void DoInverseAction(int actionToUndoIndex)
		{
			Action actionToUndo = actions[actionToUndoIndex];
			GameObject objectToUndo = actions[actionToUndoIndex].myGameObject;

			if (actionToUndo.actionType == actionType.Delete)
			{

				GameObject block = Instantiate(Resources.Load($"Blocks/{actionToUndo.gameObjectName}", typeof(GameObject))) as GameObject;
				block.transform.position = actionToUndo.position;
				block.transform.rotation = actionToUndo.rotation;
				block.transform.localScale = actionToUndo.scale;
				block.tag = "Block";

				Action undoneAction = new Action(block, actionType.Create);
				PushAction(undoneAction, block);
			}
			else if (actionToUndo.actionType == actionType.Create)
			{
				/*	We are deleting an object, so let's save a reference to that object to swap
					every object with when a replacement of that object might be created. we will
					save this reference to a list of deleted objects.*/
				Action undoneAction = new Action(objectToUndo, actionType.Delete);
				PushAction(undoneAction, objectToUndo);
				Destroy(objectToUndo);
			} 
			else if (actionToUndo.actionType == actionType.MaterialChange)
			{
				Action undoneAction = new Action(objectToUndo, actionType.MaterialChange);
				PushAction(undoneAction, objectToUndo);
				objectToUndo.GetComponent<Renderer>().material = actionToUndo.material;
			}
			else if (/*objectToUndo != null && actionToUndo != null &&*/ actionToUndo.actionType == actionType.Move)
			{
				Action undoneAction = new Action(objectToUndo);
				PushAction(undoneAction, objectToUndo);

				objectToUndo.transform.position = actionToUndo.position;
				objectToUndo.transform.rotation = actionToUndo.rotation;
				objectToUndo.transform.localScale = actionToUndo.scale;
			}
		}
	}

	#endregion

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
		levelPath = Application.persistentDataPath + "/save";
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
			SaveLevel("coolLevel");

		if (Input.GetKeyDown(KeyCode.L))
			LoadLevel("coolLevel");
	}

	public static void SaveLevel(string levelName)
	{
		BinaryFormatter myFormatter = new();
		FileStream myStream = new(levelPath + levelName + ".kek", FileMode.Create);

		LevelSavedData myData = new();

		myFormatter.Serialize(myStream, myData);
		myStream.Close();
	}

	private static LevelSavedData GetLevelFromFile(string levelName)
	{
		if (File.Exists(levelPath + levelName + ".kek"))
		{
			BinaryFormatter myFormatter = new();
			FileStream myStream = new(levelPath + levelName + ".kek", FileMode.Open);

			LevelSavedData myData = myFormatter.Deserialize(myStream) as LevelSavedData;
			myStream.Close();

			return myData;
		}
		else
		{
			Debug.LogError($"Tried to load level that doesn't exist!!! {levelPath + levelName + ".kek"}");
			return null;
		}
	}

	public static void LoadLevel(string levelName)
	{
		LevelSavedData levelToLoad = GetLevelFromFile(levelName);
		if (levelToLoad != null)
		{
			foreach (GameObject block in blocks)
			{
				Destroy(block);
			}
			foreach (string blockName in levelToLoad.blockNames)
			{
				//spawn object
				GameObject instance = Instantiate(Resources.Load($"Blocks/{blockName}", typeof(GameObject))) as GameObject;

				//paint object
				instance.GetComponent<Renderer>().material = Resources.Load("Material/" + levelToLoad.blockMaterials[0], typeof(Material)) as Material;
				levelToLoad.blockMaterials.Remove(levelToLoad.blockMaterials[0]);

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