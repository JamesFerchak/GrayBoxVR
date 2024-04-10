using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDefinitions : MonoBehaviour
{
    private static ObjectDefinitions _singleton;
    public static ObjectDefinitions Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one ObjectDefinitions! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
        allObjects = primitiveObjects;
    }

    [SerializeField] List<ObjectType> primitiveObjects; // Pre-created
    List<ObjectType> allObjects;

    [SerializeField] static int maximumObjects;
    [SerializeField] Sprite placeholderSprite;

    public GameObject GetObjectShape(string shapeID) // Gets a shape using a shapeID
    {
        foreach (var possibleShape in allObjects)
        {
            if (shapeID == possibleShape.name)
            {
                return possibleShape.shape;
            }
        }
        return allObjects[0].shape;
    }

    public Sprite GetObjectSprite(string shapeID) // Gets a sprite using a shapeID
    {
        foreach (var possibleShape in allObjects)
        {
            if (shapeID == possibleShape.name)
            {
                return possibleShape.sprite;
            }
        }
        return placeholderSprite;
    }

    public void SetObjectSprite(string shapeID, Sprite newSprite) // Sets a sprite using a shapeID
    {
        //Debug.Log("SetObjectSprite for " + shapeID + " called!");
        for (int i = 0; i < allObjects.Count; i++)
        {
            if (shapeID == allObjects[i].name)
            {
                ObjectType myObject = allObjects[i];
                myObject.sprite = newSprite;
                allObjects[i] = myObject;
            }
        }
    }

    ObjectType GenerateObjectType(string n, GameObject m)
    {
        ObjectType newObject;
        newObject.name = n;
        newObject.shape = m;
        newObject.sprite = placeholderSprite;
        return newObject;
    }
}

[Serializable]
struct ObjectType
{
    public string name;
    public GameObject shape;
    public Sprite sprite;
}
