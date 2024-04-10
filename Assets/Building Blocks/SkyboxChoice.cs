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

    
    public void ChangeSkybox(int skyboxIndex)
    {
        if (skyboxIndex >= 0 && skyboxIndex < skyboxMaterials.Count)
        {
            //Debug.LogWarning("trying to change skybox!!!.");
            RenderSettings.skybox = skyboxMaterials[skyboxIndex];
            
            DynamicGI.UpdateEnvironment();
        }
        else
        {
            Debug.LogWarning("invalid index.");
        }
    }

    void Start()
    {
        ChangeSkybox(2);
    }
}