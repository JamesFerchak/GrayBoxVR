using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using Image = UnityEngine.UI.Image;

public class MenuActions : MonoBehaviour
{
    private static MenuActions _singleton;
    public static MenuActions Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one MenuActions! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    [SerializeField] GameObject mainMenuCanvas; // Cannot be deleted
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject[] mainMenuTabs;
    
    [SerializeField] GameObject cam;
    [SerializeField] GameObject rightHandController;
    [SerializeField] GameObject leftHandController;

    [SerializeField] Slider placementAssistanceSlider;
    [SerializeField] Text placementAssistanceText;
    [SerializeField] Slider rotationAssistanceSlider;
    [SerializeField] Text rotationAssistanceText;
    [SerializeField] Slider scalingAssistanceSlider;
    [SerializeField] Text scalingAssistanceText;

    [SerializeField] Image catalogCurrentSelection;
    [SerializeField] Sprite squareAsset;
    [SerializeField] Sprite sphereAsset;
    [SerializeField] Sprite cylinderAsset;
    [SerializeField] Sprite pyramidAsset;
    [SerializeField] Sprite floorAsset;
    [SerializeField] Sprite pillarAsset;
    [SerializeField] Sprite shortPillarAsset;
    [SerializeField] Sprite wallAsset;
    Sprite[] spriteArray = new Sprite[10];

    [SerializeField] GameObject[] levelThumbnailsLoadMenu = new GameObject[10];
    [SerializeField] GameObject[] levelThumbnailsSaveMenu = new GameObject[10];
    [SerializeField] GameObject[] loadButtons = new GameObject[10];
    bool[] projectExists = new bool[10];

    public bool inMenuMode; // True if the menu is open

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        placementAssistanceSlider.onValueChanged.AddListener((value) =>
        {
            if (value != 0)
            {
                placementAssistanceText.text = (1.0f / (6.0f - value)).ToString("0.00");
            }
            else
            {
                placementAssistanceText.text = "0";
            }
        });

        rotationAssistanceSlider.onValueChanged.AddListener((value) =>
        {
            rotationAssistanceText.text = (15.0f * value).ToString("0");
        });

        scalingAssistanceSlider.onValueChanged.AddListener((value) =>
        {
            if (value != 0)
            {
                scalingAssistanceText.text = (1.0f / (6.0f - value)).ToString("0.00");
            }
            else
            {
                scalingAssistanceText.text = "0";
            }
        });

        AddSavesToMenuUI();
    }

    private void Update()
    {
        // Rotate the canvas so it faces towards the camera
        mainMenuCanvas.transform.rotation = Quaternion.LookRotation(mainMenuCanvas.transform.position - cam.transform.position);

        // If player gets too close, teleport the UI away
        if (Mathf.Abs(mainMenuCanvas.transform.position.x - cam.transform.position.x) < 5.0f &&
            Mathf.Abs(mainMenuCanvas.transform.position.z - cam.transform.position.z) < 5.0f)
        {
            RelocateMainMenu();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void SelectCube()
    {
        ObjectCreator.Singleton.ChangeToShape("cube");
        catalogCurrentSelection.sprite = squareAsset;
        HologramDisplay.Singleton.SetHologramToCube();
    }

    public void SelectSphere()
    {
        ObjectCreator.Singleton.ChangeToShape("sphere");
        catalogCurrentSelection.sprite = sphereAsset;
        HologramDisplay.Singleton.SetHologramToSphere();
    }

    public void SelectCylinder()
    {
        ObjectCreator.Singleton.ChangeToShape("cylinder");
        catalogCurrentSelection.sprite = cylinderAsset;
        HologramDisplay.Singleton.SetHologramToCylinder();
    }

    public void SelectPyramid()
    {
        ObjectCreator.Singleton.ChangeToShape("pyramid");
        catalogCurrentSelection.sprite = pyramidAsset;
        HologramDisplay.Singleton.SetHologramToPyramid();
    }

    public void SelectFloor()
    {
        ObjectCreator.Singleton.ChangeToShape("floor");
        catalogCurrentSelection.sprite = floorAsset;
        HologramDisplay.Singleton.SetHologramToFloor();
    }

    public void SelectPillar()
    {
        ObjectCreator.Singleton.ChangeToShape("pillar");
        catalogCurrentSelection.sprite = pillarAsset;
        HologramDisplay.Singleton.SetHologramToPillar();
    }

    public void SelectShortPillar()
    {
        ObjectCreator.Singleton.ChangeToShape("shortpillar");
        catalogCurrentSelection.sprite = shortPillarAsset;
        HologramDisplay.Singleton.SetHologramToShortPillar();
    }

    public void SelectWall()
    {
        ObjectCreator.Singleton.ChangeToShape("wall");
        catalogCurrentSelection.sprite = wallAsset;
        HologramDisplay.Singleton.SetHologramToWall();
    }

    public void RelocateMainMenu()
    {
        Vector3 newMainMenuPosition = cam.transform.TransformPoint(Vector3.forward * 2);
        newMainMenuPosition.y = 35;
        mainMenuCanvas.transform.position = newMainMenuPosition;
    }

    public void SelectColor(string color)
    {
        ObjectPainter.Singleton.current_wrap = color;
    }

    public void SwitchMenuTabs(int tabID) // 0: Options, 1: Shapes, 2: Save, 3: Load, 4: Wraps
    {
        // Closes menu tabs except for the given tabID
        for (int i = 0; i <= 5; i++)
        {
            if (i != tabID)
            {
                mainMenuTabs[i].SetActive(true);
            }
            else
            {
                mainMenuTabs[i].SetActive(false);
            }
        }
    }

    public void SaveLevelWithButton(string saveID)
    {
        BlockRangler.SaveLevel("save" + saveID);
        SwitchMenuTabs(0); // Switches to Options tab
        InteractWithMainMenu(); // Closes menu
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "/save" + saveID + "thumbnail.png"); // Saves to project directory
        
        if (!projectExists[(int)saveID[0] - 65])
        {
            loadButtons[(int)saveID[0] - 65].SetActive(true);
            projectExists[(int)saveID[0] - 65] = true;
        }
    }

    public void LoadLevelWithButton(string saveID)
    {
        BlockRangler.LoadLevel("save" + saveID);
        SwitchMenuTabs(0); // Switches to Options tab
        InteractWithMainMenu();
    }

    public void InteractWithMainMenu()
    {
        if (mainMenuPanel != null) // If panel exists
        {
            if (inMenuMode) // If panel is already open
            {
                mainMenuPanel.SetActive(false); // Close panel
                inMenuMode = false;
            }
            else // If panel is closed
            {
                MenuActions.Singleton.RelocateMainMenu();
                mainMenuPanel.SetActive(true); // Open panel
                inMenuMode = true;
            }
        }
    }

    public void AddSavesToMenuUI()
    {
        // Code to load images into load menu and save menu
        Texture2D textureConverter = new Texture2D(2, 2);
        byte[] bytes;

        string filePath = Application.persistentDataPath + "/";
        for (int i = 65; i < 75; i++)
        {
            if (File.Exists(filePath + "save" + (char)i + "thumbnail.png"))
            {
                bytes = File.ReadAllBytes(filePath + "save" + (char)i + "thumbnail.png");
                textureConverter.LoadImage(bytes);
                spriteArray[i - 65] = Sprite.Create(textureConverter, new Rect(0, 0, textureConverter.width, textureConverter.height), new Vector2(), 100.0f);
                spriteArray[i - 65].name = "sprite" + (char)i;
                levelThumbnailsLoadMenu[i - 65].GetComponent<Image>().sprite = spriteArray[i - 65];
                levelThumbnailsSaveMenu[i - 65].GetComponent<Image>().sprite = spriteArray[i - 65];
                projectExists[i - 65] = true;
                textureConverter = new Texture2D(2, 2);
            }
            else
            {
                loadButtons[i - 65].SetActive(false);
                projectExists[i - 65] = false;
            }
        }
    }
}
