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

        Vector3 rotation = new Vector3(0, gameObject.transform.rotation.y * 90, 0); // Places block flat, only keeping y rotation of controller

        Instantiate(cubePrefab.gameObject, position, Quaternion.Euler(rotation)); // Places cube in level
    }

    public void EraseObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Destroy the hit object
            Destroy(hit.transform.gameObject);
        }
    }

    public void EnlargeObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Enlarge the hit object
            hit.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    public void DownsizeObject()
    {
        Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Downsize the hit object
            hit.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
}
