using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ObjectPainter : MonoBehaviour
{
    public static ObjectPainter _singleton;
    public static ObjectPainter Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one ObjectPainter! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public string current_wrap;
    public Material red;
    public Material blue;
    public Material white;
    public Material black;
    public Material brown;
    public Material purple;
    public Material green;
    public Material yellow;
    public Material orange;
    public Material pink;
    public Material gray;
    public Material stone;
    public Material glass;
    public Material space;
    public Material smile;
    public Material cyan;
    public Material brick; 
    public Material water; 
    public Material sand; 
    public Material tiles; 
    public Material metal; 
    public Material wood;
    public Material dirt;
    public Material grass;

    public AudioClip paintNoise;

    public void AutoPaintObject(GameObject objectToPaint)
    {
        Material paintingMaterial = red; // Default

        switch (current_wrap)
        {
            case "red":
                paintingMaterial = red;
                break;
            case "blue":
                paintingMaterial = blue;
                break;
            case "yellow":
                paintingMaterial = yellow;
                break;
            case "white":
                paintingMaterial = white;
                break;
            case "black":
                paintingMaterial = black;
                break;
            case "green":
                paintingMaterial = green;
                break;
            case "brown":
                paintingMaterial = brown;
                break;
            case "orange":
                paintingMaterial = orange;
                break;
            case "purple":
                paintingMaterial = purple;
                break;
            case "pink":
                paintingMaterial = pink;
                break;
            case "gray":
                paintingMaterial = gray;
                break;
            case "cyan":
                paintingMaterial = cyan;
                break;
            case "stone":
                paintingMaterial = stone;
                break;
            case "glass":
                paintingMaterial = glass;
                break;
            case "space":
                paintingMaterial = space;
                break;
            case "smile":
                paintingMaterial = smile;
                break;
            case "brick":
                paintingMaterial = brick;
                break;
            case "sand":
                paintingMaterial = sand;
                break;
            case "water":
                paintingMaterial = water;
                break;
            case "metal":
                paintingMaterial = metal;
                break;
            case "tiles":
                paintingMaterial = tiles;
                break;
            case "wood":
                paintingMaterial = wood;
                break;
            case "dirt":
                paintingMaterial = dirt;
                break;
            case "grass":
                paintingMaterial = grass;
                break;
        }

        objectToPaint.GetComponent<MeshRenderer>().material = paintingMaterial;

        if (objectToPaint.transform.childCount > 0) // If the object has a child
        {
            GameObject child = objectToPaint.transform.GetChild(0).gameObject;
            child.GetComponent<MeshRenderer>().material = paintingMaterial;
        }
    }

    public void PaintObject()
    {
        Transform LeftHandObject = LeftHandController.Singleton.GetLeftHandObject().transform;
        Ray ray = new Ray(LeftHandObject.position, LeftHandObject.forward); //casts ray

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BlockRangler.ActionHistory.PushMaterialAction(hit.transform.gameObject);
            AudioSource.PlayClipAtPoint(paintNoise, hit.transform.position);
            if (hit.transform.gameObject.GetComponent<BuildingBlockBehavior>() != null)
            {
                Material paintingMaterial = red; // Default

                switch (current_wrap)
                {
                    case "red":
                        paintingMaterial = red;
                        break;
                    case "blue":
                        paintingMaterial = blue;
                        break;
                    case "yellow":
                        paintingMaterial = yellow;
                        break;
                    case "green":
                        paintingMaterial = green;
                        break;
                    case "brown":
                        paintingMaterial = brown;
                        break;
                    case "white":
                        paintingMaterial = white;
                        break;
                    case "black":
                        paintingMaterial = black;
                        break;
                    case "purple":
                        paintingMaterial = purple;
                        break;
                    case "pink":
                        paintingMaterial = pink;
                        break;
                    case "orange":
                        paintingMaterial = orange;
                        break;
                    case "gray":
                        paintingMaterial = gray;
                        break;
                    case "cyan":
                        paintingMaterial = cyan;
                        break;
                    case "stone":
                        paintingMaterial = stone;
                        break;
                    case "glass":
                        paintingMaterial = glass;
                        break;
                    case "space":
                        paintingMaterial = space;
                        break;
                    case "smile":
                        paintingMaterial = smile;
                        break;
                    case "brick":
                        paintingMaterial = brick;
                        break;
                    case "sand":
                        paintingMaterial = sand;
                        break;
                    case "water":
                        paintingMaterial = water;
                        break;
                    case "metal":
                        paintingMaterial = metal;
                        break;
                    case "tiles":
                        paintingMaterial = tiles;
                        break;
                    case "wood":
                        paintingMaterial = wood;
                        break;
                    case "dirt":
                        paintingMaterial = dirt;
                        break;
                    case "grass":
                        paintingMaterial = grass;
                        break;
                }

                GameObject objectToPaint = hit.transform.gameObject;

                objectToPaint.GetComponent<MeshRenderer>().material = paintingMaterial;

                if (objectToPaint.transform.childCount > 0) // If the object has a child
                {
                    GameObject child = objectToPaint.transform.GetChild(0).gameObject; // Paint the first child
                    child.GetComponent<MeshRenderer>().material = paintingMaterial;
                }
            }

        }
    }

    public int GetButtonIndexOfWrap(string selectedColor)
    {
        int selectedColorIndex = 0;
        switch (selectedColor)
        {
            case "red":
                selectedColorIndex = 0;
                break;
            case "blue":
                selectedColorIndex = 1;
                break;
            case "yellow":
                selectedColorIndex = 2;
                break;
            case "green":
                selectedColorIndex = 3;
                break;
            case "brown":
                selectedColorIndex = 4;
                break;
            case "white":
                selectedColorIndex = 5;
                break;
            case "black":
                selectedColorIndex = 6;
                break;
            case "purple":
                selectedColorIndex = 7;
                break;
            case "pink":
                selectedColorIndex = 8;
                break;
            case "orange":
                selectedColorIndex = 9;
                break;
            case "gray":
                selectedColorIndex = 10;
                break;
            case "cyan":
                selectedColorIndex = 11;
                break;
            case "stone":
                selectedColorIndex = 12;
                break;
            case "glass":
                selectedColorIndex = 13;
                break;
            case "space":
                selectedColorIndex = 14;
                break;
            case "smile":
                selectedColorIndex = 15;
                break;
            case "brick":
                selectedColorIndex = 16;
                break;
            case "sand":
                selectedColorIndex = 17;
                break;
            case "water":
                selectedColorIndex = 18;
                break;
            case "metal":
                selectedColorIndex = 19;
                break;
            case "tiles":
                selectedColorIndex = 20;
                break;
            case "wood":
                selectedColorIndex = 21;
                break;
            case "dirt":
                selectedColorIndex = 22;
                break;
            case "grass":
                selectedColorIndex = 23;
                break;
        }
        return selectedColorIndex;
    }
}


