using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockRangler : MonoBehaviour
{
    private static BlockRangler _singleton;
    public static BlockRangler Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one Block Rangler! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

	private void Awake()
	{
        Singleton = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        GameObject instance = Instantiate(Resources.Load("Blocks/Cube", typeof(GameObject))) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
