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
        switch (current_wrap)
        {
            case "red":
                objectToPaint.GetComponent<MeshRenderer>().material = red;
                break;
            case "blue":
                objectToPaint.GetComponent<MeshRenderer>().material = blue;
                break;
            case "yellow":
                objectToPaint.GetComponent<MeshRenderer>().material = yellow;
                break;
            case "white":
                objectToPaint.GetComponent<MeshRenderer>().material = white;
                break;
            case "black":
                objectToPaint.GetComponent<MeshRenderer>().material = black;
                break;
            case "green":
                objectToPaint.GetComponent<MeshRenderer>().material = green;
                break;
            case "brown":
                objectToPaint.GetComponent<MeshRenderer>().material = brown;
                break;
            case "orange":
                objectToPaint.GetComponent<MeshRenderer>().material = orange;
                break;
            case "purple":
                objectToPaint.GetComponent<MeshRenderer>().material = purple;
                break;
            case "pink":
                objectToPaint.GetComponent<MeshRenderer>().material = pink;
                break;
            case "gray":
                objectToPaint.GetComponent<MeshRenderer>().material = gray;
                break;
            case "cyan":
                objectToPaint.GetComponent<MeshRenderer>().material = cyan;
                break;
            case "stone":
                objectToPaint.GetComponent<MeshRenderer>().material = stone;
                break;
            case "glass":
                objectToPaint.GetComponent<MeshRenderer>().material = glass;
                break;
            case "space":
                objectToPaint.GetComponent<MeshRenderer>().material = space;
                break;
            case "smile":
                objectToPaint.GetComponent<MeshRenderer>().material = smile;
                break;
            case "brick":
                objectToPaint.GetComponent<MeshRenderer>().material = brick;
                break;
            case "sand":
                objectToPaint.GetComponent<MeshRenderer>().material = sand;
                break;
            case "water":
                objectToPaint.GetComponent<MeshRenderer>().material = water;
                break;
            case "metal":
                objectToPaint.GetComponent<MeshRenderer>().material = metal;
                break;
            case "tiles":
                objectToPaint.GetComponent<MeshRenderer>().material = tiles;
                break;
            case "wood":
                objectToPaint.GetComponent<MeshRenderer>().material = wood;
                break;
            case "dirt":
                objectToPaint.GetComponent<MeshRenderer>().material = dirt;
                break;
            case "grass":
                objectToPaint.GetComponent<MeshRenderer>().material = grass;
                break;
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
                switch (current_wrap)
                {
                    case "red":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = red;
                        break;
                    case "blue":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = blue;
                        break;
                    case "yellow":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = yellow;
                        break;
                    case "green":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = green;
                        break;
                    case "brown":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = brown;
                        break;
                    case "white":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = white;
                        break;
                    case "black":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = black;
                        break;
                    case "purple":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = purple;
                        break;
                    case "pink":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = pink;
                        break;
                    case "orange":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = orange;
                        break;
                    case "gray":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = gray;
                        break;
                    case "cyan":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = cyan;
                        break;
                    case "stone":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = stone;
                        break;
                    case "glass":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = glass;
                        break;
                    case "space":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = space;
                        break;
                    case "smile":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = smile;
                        break;
                    case "brick":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = brick;
                        break;
                    case "sand":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = sand;
                        break;
                    case "water":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = water;
                        break;
                    case "metal":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = metal;
                        break;
                    case "tiles":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = tiles;
                        break;
                    case "wood":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = wood;
                        break;
                    case "dirt":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = dirt;
                        break;
                    case "grass":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = grass;
                        break;
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


