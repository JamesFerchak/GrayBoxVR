using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFacer : MonoBehaviour
{

    [SerializeField] GameObject cam;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - cam.transform.position);
    }
}
