using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UIElements;

public class HologramDisplay : MonoBehaviour
{
    private static HologramDisplay _singleton;
    public static HologramDisplay Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one HologramDisplay! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    public GameObject currentHologram;
    public bool hologramEnabled = true;
    public Material hologramMaterial;

    // Start is called before the first frame update
    void Start()
    {
        GameObject cubeHologramMesh = ObjectDefinitions.Singleton.GetObjectShape("cube");
        currentHologram = Instantiate(cubeHologramMesh, Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 0.0f));
        currentHologram.gameObject.GetComponent<MeshRenderer>().material = hologramMaterial;
        currentHologram.tag = "Untagged";
        currentHologram.transform.GetComponent<Collider>().enabled = false;
        Destroy(currentHologram.GetComponent<BuildingBlockBehavior>());
    }

    public void ShowHologram(Vector3 position, Quaternion rotation)
    {
        if (hologramEnabled)
        {
            currentHologram.transform.position = position;
            currentHologram.transform.rotation = rotation;
        }
    }

    public void SetHologramToShape(string shapeID)
    {
        GameObject newHologramMesh = ObjectDefinitions.Singleton.GetObjectShape(shapeID);
        GameObject newHologram = Instantiate(newHologramMesh, Vector3.zero, Quaternion.Euler(0.0f, 0.0f, 0.0f));

        if (newHologram.transform.childCount > 0) // If the object has a child, give that the hologram effect
        {
            GameObject child = newHologram.transform.GetChild(0).gameObject;
            child.GetComponent<MeshRenderer>().material = hologramMaterial;
        }
        else // Otherwise, the original object is the hologram
        {
            newHologram.GetComponent<MeshRenderer>().material = hologramMaterial;
        }
        Destroy(newHologram.GetComponent<BuildingBlockBehavior>());

        GameObject oldHologram = currentHologram;
        currentHologram = newHologram;
        currentHologram.tag = "Untagged";
        if (currentHologram.transform.GetComponent<Collider>())
        {
            currentHologram.transform.GetComponent<Collider>().enabled = false;
        }
        if (currentHologram.transform.GetComponent<MeshCollider>())
        {
            currentHologram.transform.GetComponent<MeshCollider>().enabled = false;
        }
        oldHologram.transform.position = new Vector3(-1000.0f, 0.0f, 0.0f);
        Destroy(oldHologram);
    }

    public void ToggleHologram()
    {
        currentHologram.transform.position = new Vector3(-1000.0f, 0.0f, 0.0f);
        hologramEnabled = !hologramEnabled;
    }

    public bool GetHologramState()
    {
        return hologramEnabled;
    }
}
