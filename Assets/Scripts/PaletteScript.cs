using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteScript : MonoBehaviour
{
    public GameObject cubePrefab; // Cube GameObject

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaceObject()
    {
        Vector3 position = gameObject.transform.position; // Needs to record location of block placement instead
        Quaternion rotation = gameObject.transform.rotation; // May need to record rotation of camera instead
        Instantiate(cubePrefab.gameObject, position, rotation); // Places cube in level
    }
}
