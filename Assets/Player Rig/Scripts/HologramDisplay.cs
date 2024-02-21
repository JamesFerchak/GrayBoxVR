using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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

    // TODO: Remove these objects, make them references to the original objects instead
    public GameObject holoCubePrefab; // Cube GameObject
    public GameObject holoSpherePrefab; // Sphere GameObject
    public GameObject holoCylinderPrefab; // Cylinder GameObject
    public GameObject holoPyramidPrefab; // Pyramid GameObject
    public GameObject holoFloorPrefab; // Cube GameObject
    public GameObject holoPillarPrefab; // Long pillar GameObject
    public GameObject holoShortPillarPrefab; // Short pillar GameObject
    public GameObject holoWallPrefab; // Wall GameObject
    public GameObject currentHologram;

    public bool hologramEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        // Converts prefabs into in-game objects
        holoCubePrefab = Instantiate(holoCubePrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoSpherePrefab = Instantiate(holoSpherePrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoCylinderPrefab = Instantiate(holoCylinderPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoPyramidPrefab = Instantiate(holoPyramidPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoFloorPrefab = Instantiate(holoFloorPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoPillarPrefab = Instantiate(holoPillarPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoShortPillarPrefab = Instantiate(holoShortPillarPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        holoWallPrefab = Instantiate(holoWallPrefab, new Vector3(-1000.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));

        currentHologram = holoCubePrefab;
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
        GameObject newHologram;

        switch (shapeID)
        {
            case "cube":
                newHologram = holoCubePrefab;
                break;
            case "sphere":
                newHologram = holoSpherePrefab;
                break;
            case "cylinder":
                newHologram = holoCylinderPrefab;
                break;
            case "pyramid":
                newHologram = holoPyramidPrefab;
                break;
            case "floor":
                newHologram = holoFloorPrefab;
                break;
            case "pillar":
                newHologram = holoPillarPrefab;
                break;
            case "shortpillar":
                newHologram = holoShortPillarPrefab;
                break;
            case "wall":
                newHologram = holoWallPrefab;
                break;
            default:
                newHologram = holoCubePrefab;
                break;
        }
        currentHologram.transform.position = new Vector3(-1000.0f, 0.0f, 0.0f);
        currentHologram = newHologram;
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
