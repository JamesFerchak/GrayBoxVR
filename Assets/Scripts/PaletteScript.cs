using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject cubePrefab; // Cube GameObject
    public GameObject[] editArray; // Array of placed GameObjects
    int editsUsed = 0; // Amount of edits in the level so far

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if button is pressed
        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f); // Needs to record location of block placement instead
        Quaternion rotation = cubePrefab.transform.rotation; // May need to record rotation of camera instead
        editArray[editsUsed] = Instantiate(cubePrefab.gameObject, position, rotation); // Places cube in level
        editsUsed++; // Increments editsUsed
    }
}
