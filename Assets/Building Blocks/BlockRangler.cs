using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.XR.CoreUtils;
using Environment = System.Environment;

public class BlockRangler : MonoBehaviour
{
	readonly static int actionHistorySize = 500;
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
		MaterialChange,
		AddToGroup,
		Ungroup
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

		public List<Action> groupedActions;
		public List<GameObject> groupedObjects;
		public List<int> groupedObjectIDs;
		public bool isGroupChange => groupedActions != null;

		public Action(GameObject affectedObject, actionType thisActionType)
		{
			myGameObject = affectedObject;
			gameObjectName = SimplifyObjectName(affectedObject.name);
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			if (affectedObject.GetComponent<Renderer>() != null)
				material = affectedObject.GetComponent<Renderer>().material;
			else
				material = null;
			actionType = thisActionType;
		}

		public Action(GameObject affectedObject, actionType thisActionType, List<GameObject> inputGroupedObjects)
		{
			myGameObject = affectedObject;
			gameObjectName = SimplifyObjectName(affectedObject.name);
			position = affectedObject.transform.position;
			rotation = affectedObject.transform.rotation;
			scale = affectedObject.transform.localScale;
			if (affectedObject.GetComponent<Renderer>() != null)
				material = affectedObject.GetComponent<Renderer>().material;
			else
				material = null;
			actionType = thisActionType;
			groupedActions = new List<Action>();
			groupedObjects = new List<GameObject>();
			groupedObjectIDs = new List<int>();
			foreach (GameObject thisObject in inputGroupedObjects)
			{
				groupedObjectIDs.Add(thisObject.GetInstanceID());
				groupedObjects.Add(thisObject);
				Action thisAction = new Action(thisObject, actionType.Move);
				groupedActions.Add(thisAction);
			}
		}
	}

	//this takes the name of an individual object and returns the name of the prefab it came from
	public static string SimplifyObjectName(string input)
	{
		return input.Contains('(') ? input.Substring(0, (input.IndexOf('('))) : input;
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
		
		private static void PushAction(Action actionToRecord)
		{
			SetActionObject(actionToRecord.myGameObject);
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
					{	if (actions[objectIndex] != null)
						{
							if (actionObjectIDs[objectIndex] == objectToReplaceID)
							{
								actionObjectIDs[objectIndex] = objectToRecord.GetInstanceID();
								actions[objectIndex].myGameObject = objectToRecord;
							}
							if (actions[objectIndex].isGroupChange)
							{
								for (int i = 0; i < actions[objectIndex].groupedActions.Count; i++)
								{
									if (actions[objectIndex].groupedObjectIDs[i] == objectToReplaceID)
									{
										actions[objectIndex].groupedObjectIDs[i] = objectToRecord.GetInstanceID();
										actions[objectIndex].groupedActions[i].myGameObject = objectToRecord;
									}
								}
							}
						}
					}
				}
			}
		}

		//public
		public static void PushMoveAction(GameObject objectToRecord)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Move);
			PushAction(actionToPush);
		}

		public static void PushCreateAction(GameObject objectToRecord)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Create);
			PushAction(actionToPush); 
		}

		public static void PushCreateAction(GameObject objectToRecord, List<GameObject> groupedObjects)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Create, groupedObjects);
			PushAction(actionToPush); 
		}

		public static void PushDeleteAction(GameObject objectToRecord)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Delete);
			PushAction(actionToPush); 
		}

		public static void PushDeleteAction(GameObject objectToRecord, List<GameObject> groupedObjects)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.Delete, groupedObjects);
			PushAction(actionToPush); 
		}

		public static void PushMaterialAction(GameObject objectToRecord)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(objectToRecord, actionType.MaterialChange);
			PushAction(actionToPush); 
		}

		public static void PushAddToGroupAction(List<GameObject> groupedObjects)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(ObjectManipulator.parentOfGroup, actionType.AddToGroup, groupedObjects);
			PushAction(actionToPush);
		}

		public static void PushUngroupAction(List<GameObject> groupedObjects)
		{
			IncrementBothIndices();
			Action actionToPush = new Action(ObjectManipulator.parentOfGroup, actionType.Ungroup, groupedObjects);
			PushAction(actionToPush);
		}

		public static void UndoAction()
		{
			if (actions[TopIndex] == null)
				return;
			Effects.Singleton.PlaySound(LeftHandController.Singleton.transform.position, 11);
			DoInverseAction(TopIndex);
			DecrementTopIndex();
		}

		public static void RedoAction()
		{
			if (TopIndex + 1 == BottomIndex || (TopIndex == actionHistorySize - 1 && BottomIndex == 0))
				return;
            Effects.Singleton.PlaySound(RightHandController.Singleton.transform.position, 12);
            IncrementTopIndex();
			DoInverseAction(TopIndex);
		}

		private static void DoInverseAction(int actionToUndoIndex)
		{
			Action actionToUndo = actions[actionToUndoIndex];
			GameObject objectToUndo = actions[actionToUndoIndex].myGameObject;

			if (actionToUndo.actionType == actionType.Delete)
			{
				GameObject block;
				if (!actionToUndo.isGroupChange)
				{
					block = Instantiate(Resources.Load($"Blocks/{actionToUndo.gameObjectName}", typeof(GameObject))) as GameObject;
					block.tag = "Block";
				}
				else
				{
					block = new GameObject();
					block.name = "Group";
				}
				
				if (block.GetComponent<Renderer>() != null)
					block.GetComponent<Renderer>().material = actionToUndo.material;

				if (actionToUndo.isGroupChange)
				{
					for (int i = 0; i < actionToUndo.groupedActions.Count; i++)
					{
                        GameObject childBlock = Instantiate(Resources.Load($"Blocks/{actionToUndo.groupedActions[i].gameObjectName}", typeof(GameObject))) as GameObject;
                        childBlock.transform.position = actionToUndo.groupedActions[i].position;
                        childBlock.transform.rotation = actionToUndo.groupedActions[i].rotation;
                        childBlock.transform.localScale = actionToUndo.groupedActions[i].scale;
                        childBlock.GetComponent<Renderer>().material = actionToUndo.groupedActions[i].material;
                        childBlock.tag = "Block";
                        childBlock.transform.parent = block.transform;

                        actionToUndo.groupedObjects[i] = childBlock;
                    }
				}
				
				//have set the transform of the parent block *after* we create all the children so they enherit the transform of the parent
				block.transform.position = actionToUndo.position;
				block.transform.rotation = actionToUndo.rotation;
				block.transform.localScale = actionToUndo.scale;

				Action undoneAction;
				if (!actionToUndo.isGroupChange)
					undoneAction = new Action(block, actionType.Create);
				else
					undoneAction = new Action(block, actionType.Create, actionToUndo.groupedObjects);
				PushAction(undoneAction);
			}
			else if (actionToUndo.actionType == actionType.Create)
			{
				if (actionToUndo.isGroupChange)
				{
					Action undoneAction = new Action(objectToUndo, actionType.Delete, actionToUndo.groupedObjects);
					PushAction(undoneAction);
					for (int thisChild = 0; thisChild < objectToUndo.transform.childCount; thisChild++)
					{
						Destroy(objectToUndo.transform.GetChild(thisChild));
					}
					Destroy(objectToUndo);
				}else
				{
					Action undoneAction = new Action(objectToUndo, actionType.Delete);
					PushAction(undoneAction);
					Destroy(objectToUndo);
				}
			} 
			else if (actionToUndo.actionType == actionType.MaterialChange)
			{
				Action undoneAction = new Action(objectToUndo, actionType.MaterialChange);
				PushAction(undoneAction);
				objectToUndo.GetComponent<Renderer>().material = actionToUndo.material;
			}
			else if (actionToUndo.actionType == actionType.AddToGroup)
			{

			}
			else if (actionToUndo.actionType == actionType.Ungroup)
			{

			}
			else if (actionToUndo.actionType == actionType.Move)
			{
				Action undoneAction = new Action(objectToUndo, actionType.Move);
				PushAction(undoneAction);

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
		levelPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/");
		levelPath += "/GrayboxVR/";

		// Check if the directory exists, if not, create it
        if (!Directory.Exists(levelPath))
        {
            Directory.CreateDirectory(levelPath);
        }
    }

    // Update is called once per frame
    void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
			SaveLevel("coolLevel");

		if (Input.GetKeyDown(KeyCode.L))
			LoadLevel("coolLevel");
	}

	#region SAVING

	public static void SaveLevel(string levelName)
	{
		BinaryFormatter myFormatter = new();
		FileStream myStream = new(levelPath + levelName + ".kek", FileMode.Create);
        Debug.Log("Level Path: " + levelPath + levelName + ".kek");

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

				instance.tag = "Block";
			}
		}

	}

	#endregion
}