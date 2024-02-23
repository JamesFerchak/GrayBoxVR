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
    }

    [SerializeField] List<ObjectType> primitiveObjects; // Pre-created
    List<ObjectType> allObjects;

    void Start()
    {
        allObjects = primitiveObjects;

        // // To add objects during runtime (assuming x, y, z initialized)
        // ObjectType additionalObject = GenerateObjectType(x, y, z)
        // allObjects.Add(additionalObject);
    }

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
        return allObjects[0].sprite;
    }

    ObjectType GenerateObjectType(string n, GameObject m, Sprite s)
    {
        ObjectType newObject;
        newObject.name = n;
        newObject.shape = m;
        newObject.sprite = s;
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
