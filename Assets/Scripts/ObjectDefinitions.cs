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

        // TEMPORARY CODE, should be auto-generated using a file without using a sprite or preset gameobject
        ObjectType floorObject = GenerateObjectType("floor", floorPrefab);
        allObjects.Add(floorObject);
        ObjectType pillarObject = GenerateObjectType("pillar", pillarPrefab);
        allObjects.Add(pillarObject);
        ObjectType shortPillarObject = GenerateObjectType("shortpillar", shortPillarPrefab);
        allObjects.Add(shortPillarObject);
        ObjectType wallObject = GenerateObjectType("wall", wallPrefab);
        allObjects.Add(wallObject);

        // We now have a list, "allObjects", that includes all preloaded objects.
    }

    [SerializeField] List<ObjectType> primitiveObjects; // Pre-created
    List<ObjectType> allObjects;

    [SerializeField] Sprite placeholderSprite;
    [SerializeField] Camera spriteCamera;
    [SerializeField] GameObject spritePosition;

    // TEMPORARY OBJECTS AND SPRITES FOR RECTANGLES
    public GameObject floorPrefab;
    public GameObject pillarPrefab;
    public GameObject shortPillarPrefab;
    public GameObject wallPrefab;

    void Start()
    {
        
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

    ObjectType GenerateObjectType(string n, GameObject m)
    {
        ObjectType newObject;
        newObject.name = n;
        newObject.shape = m;
        newObject.sprite = placeholderSprite;
        return newObject;
    }

    public Sprite GenerateObjectSprite(GameObject newObject) // VERY UNFINISHED
    {
        Vector3 objectPosition = spritePosition.transform.position;
        Quaternion objectRotation = spritePosition.transform.rotation;
        GameObject createdObject = Instantiate(newObject, objectPosition, objectRotation);
        spriteCamera.Render();
        Texture2D image = new Texture2D(spriteCamera.targetTexture.width, spriteCamera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, spriteCamera.targetTexture.width, spriteCamera.targetTexture.height), 0, 0);
        image.Apply();
        Sprite newSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(), 100.0f);
        return newSprite;
    }
}

[Serializable]
struct ObjectType
{
    public string name;
    public GameObject shape;
    public Sprite sprite;
}
