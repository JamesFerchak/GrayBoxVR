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

        // TEMPORARY CODE, should be auto-generated using a file without using a preset gameobject
        ObjectType wallObject = GenerateObjectType("4", wallPrefab);
        allObjects.Add(wallObject);
        ObjectType pillarObject = GenerateObjectType("5", pillarPrefab);
        allObjects.Add(pillarObject);
        ObjectType shortPillarObject = GenerateObjectType("6", shortPillarPrefab);
        allObjects.Add(shortPillarObject);
        ObjectType floorObject = GenerateObjectType("7", floorPrefab);
        allObjects.Add(floorObject);

        // We now have a list, "allObjects", that includes all preloaded objects.
    }

    [SerializeField] List<ObjectType> primitiveObjects; // Pre-created
    List<ObjectType> allObjects;

    [SerializeField] static int maximumObjects;
    [SerializeField] Sprite placeholderSprite;
    [SerializeField] Camera spriteCamera;
    [SerializeField] Camera originalCamera;
    [SerializeField] GameObject spriteScreenshotPosition;

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

    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();

    public IEnumerator GenerateObjectSprite(string shapeID) // Returns a sprite of the given object
    {
        // Swap to screenshot camera
        originalCamera.enabled = false;
        spriteCamera.enabled = true;

        // Create the object to take a screenshot of
        GameObject newObject = GetObjectShape(shapeID);
        Vector3 objectPosition = spriteScreenshotPosition.transform.position;
        Quaternion objectRotation = spriteScreenshotPosition.transform.rotation;
        GameObject screenshottedObject = Instantiate(newObject, objectPosition, objectRotation);

        // Wait until the end of the frame to prevent errors
        yield return frameEnd;

        // Render the image and convert it into a sprite
        spriteCamera.Render();
        Texture2D image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        image.Apply();
        Sprite newSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(), 1.0f);

        // Clean up and set the new object sprite
        SetObjectSprite(shapeID, newSprite);
        Destroy(screenshottedObject);

        // Swap back to original camera
        originalCamera.enabled = true;
        spriteCamera.enabled = false;
    }
}

[Serializable]
struct ObjectType
{
    public string name;
    public GameObject shape;
    public Sprite sprite;
}
