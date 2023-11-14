using System.Collections;
using System.Collections.Generic;
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

    public GameObject holoCubePrefab; // Cube GameObject
    public GameObject holoSpherePrefab; // Sphere GameObject
    public GameObject holoCylinderPrefab; // Cylinder GameObject
    public GameObject holoPyramidPrefab; // Pyramid GameObject
    public GameObject holoFloorPrefab; // Cube GameObject
    public GameObject holoPillarPrefab; // Long pillar GameObject
    public GameObject holoShortPillarPrefab; // Short pillar GameObject
    public GameObject holoWallPrefab; // Wall GameObject
    public GameObject currentHologram;

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

    public void ShowHologram(Vector3 position, Vector3 rotation)
    {
        currentHologram.transform.position = position;
        currentHologram.transform.rotation = Quaternion.Euler(rotation);
    }

    void SetHologramToCube()
    {
        currentHologram = holoCubePrefab;
    }

    void SetHologramToSphere()
    {
        currentHologram = holoSpherePrefab;
    }

    void SetHologramToCylinder()
    {
        currentHologram = holoCylinderPrefab;
    }

    void SetHologramToPyramid()
    {
        currentHologram = holoPyramidPrefab;
    }

    void SetHologramToFloor()
    {
        currentHologram = holoFloorPrefab;
    }

    void SetHologramToPillar()
    {
        currentHologram = holoPillarPrefab;
    }

    void SetHologramToShortPillar()
    {
        currentHologram = holoShortPillarPrefab;
    }

    void SetHologramToWall()
    {
        currentHologram = holoWallPrefab;
    }
}
