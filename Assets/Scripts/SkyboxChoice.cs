using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChoice : MonoBehaviour
{
    private static SkyboxChoice _singleton;
    public static SkyboxChoice Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one sky box choice! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    public List<Material> skyboxMaterials = new List<Material>();
    public List<Material> floorMaterials = new List<Material>();
    public GameObject floor;

    public void ChangeSkybox(int skyboxIndex)
    {
        if (skyboxIndex >= 0 && skyboxIndex < skyboxMaterials.Count)
        {
            RenderSettings.skybox = skyboxMaterials[skyboxIndex];
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("Invalid index for sky.");
        }
    }

    public void ChangeFloor(int floorIndex)
    {
        if (floorIndex >= 0 && floorIndex < floorMaterials.Count)
        {
            floor.GetComponent<MeshRenderer>().material = floorMaterials[floorIndex];
        }
        else
        {
            Debug.LogWarning("Invalid index for floor.");
        }
    }

    void Start()
    {
        ChangeSkybox(0);
        ChangeFloor(0);
    }
}