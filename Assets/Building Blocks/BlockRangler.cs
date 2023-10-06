using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class BlockRangler : MonoBehaviour
{

	public static string settingsPath;

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

    private static List<GameObject> Blocks = new List<GameObject>();

    public void AddToBlockList(GameObject blockToAdd)
    {
        Blocks.Add(blockToAdd);
        Debug.Log($"{blockToAdd.name} added to list. he is at {blockToAdd.transform.position}");
    }

	private void Awake()
	{
        Singleton = this;
		settingsPath = Application.persistentDataPath + "/PlayerSettings.kek";
	}

	// Start is called before the first frame update
	void Start()
    {
        GameObject instance = Instantiate(Resources.Load("Blocks/Cube", typeof(GameObject))) as GameObject;
        Debug.LogWarning("WORK ON SAVEPLAYERLEVEL!!!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public static void SavePlayerLevel()
	{
		/*BinaryFormatter myFormatter = new BinaryFormatter();
		FileStream myStream = new FileStream(settingsPath, FileMode.Create);

		PlayerSettingsData myData = new PlayerSettingsData(ourPlayerBehavior);

		myFormatter.Serialize(myStream, myData);
		myStream.Close();*/
	}
}
