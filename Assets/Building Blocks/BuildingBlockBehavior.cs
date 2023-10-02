using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlockBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BlockRangler.Singleton.AddToBlockList(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
