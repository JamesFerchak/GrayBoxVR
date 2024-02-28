using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public Rigidbody rigidbody;
    public CapsuleCollider collider;

    // Update is called once per frame
    void Update()
    {
        if (!TourMode.Singleton.getTourModeToggle())
        {
            //Edit Mode
            rigidbody.useGravity = false;
            rigidbody.detectCollisions = false;
            rigidbody.velocity = Vector3.zero;
            collider.enabled = false;
        }
        else
        {
            //Tour Mode
            rigidbody.useGravity = true;
            rigidbody.detectCollisions = true;
            collider.enabled = true;
        }
    }
}
