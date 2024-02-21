using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void AutoPaintObject()
    {
        switch (current_wrap)
        {
            case "red":
                gameObject.GetComponent<MeshRenderer>().material = red;
                break;
            case "blue":
                gameObject.GetComponent<MeshRenderer>().material = blue;
                break;
            case "yellow":
                gameObject.GetComponent<MeshRenderer>().material = yellow;
                break;
            case "white":
                gameObject.GetComponent<MeshRenderer>().material = white;
                break;
            case "black":
                gameObject.GetComponent<MeshRenderer>().material = black;
                break;
            case "green":
                gameObject.GetComponent<MeshRenderer>().material = green;
                break;
            case "brown":
                gameObject.GetComponent<MeshRenderer>().material = brown;
                break;
            case "orange":
                gameObject.GetComponent<MeshRenderer>().material = orange;
                break;
            case "purple":
                gameObject.GetComponent<MeshRenderer>().material = purple;
                break;
            case "pink":
                gameObject.GetComponent<MeshRenderer>().material = pink;
                break;
            case "gray":
                gameObject.GetComponent<MeshRenderer>().material = gray;
                break;
            case "cyan":
                gameObject.GetComponent<MeshRenderer>().material = cyan;
                break;
            case "stone":
                gameObject.GetComponent<MeshRenderer>().material = stone;
                break;
            case "glass":
                gameObject.GetComponent<MeshRenderer>().material = glass;
                break;
            case "space":
                gameObject.GetComponent<MeshRenderer>().material = space;
                break;
            case "smile":
                gameObject.GetComponent<MeshRenderer>().material = smile;
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
                    case "white":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = white;
                        break;
                    case "black":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = black;
                        break;
                    case "green":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = green;
                        break;
                    case "brown":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = brown;
                        break;
                    case "orange":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = orange;
                        break;
                    case "purple":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = purple;
                        break;
                    case "pink":
                        hit.transform.gameObject.GetComponent<MeshRenderer>().material = pink;
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
                }
            }

        }
    }
}


