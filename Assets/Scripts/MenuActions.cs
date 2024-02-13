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
    [SerializeField] GameObject optionsUI;
    [SerializeField] GameObject shapesUI;
    [SerializeField] GameObject wrapsUI;
    [SerializeField] GameObject saveUI;
    [SerializeField] GameObject loadUI;
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

    public void SelectSquare()
    {
        ObjectCreator.Singleton.ChangeToSquare();
        catalogCurrentSelection.sprite = squareAsset;
        HologramDisplay.Singleton.SetHologramToCube();
    }

    public void SelectSphere()
    {
        ObjectCreator.Singleton.ChangeToSphere();
        catalogCurrentSelection.sprite = sphereAsset;
        HologramDisplay.Singleton.SetHologramToSphere();
    }

    public void SelectCylinder()
    {
        ObjectCreator.Singleton.ChangeToCylinder();
        catalogCurrentSelection.sprite = cylinderAsset;
        HologramDisplay.Singleton.SetHologramToCylinder();
    }

    public void SelectPyramid()
    {
        ObjectCreator.Singleton.ChangeToPyramid();
        catalogCurrentSelection.sprite = pyramidAsset;
        HologramDisplay.Singleton.SetHologramToPyramid();
    }

    public void SelectFloor()
    {
        ObjectCreator.Singleton.ChangeToFloor();
        catalogCurrentSelection.sprite = floorAsset;
        HologramDisplay.Singleton.SetHologramToFloor();
    }

    public void SelectPillar()
    {
        ObjectCreator.Singleton.ChangeToPillar();
        catalogCurrentSelection.sprite = pillarAsset;
        HologramDisplay.Singleton.SetHologramToPillar();
    }

    public void SelectShortPillar()
    {
        ObjectCreator.Singleton.ChangeToShortPillar();
        catalogCurrentSelection.sprite = shortPillarAsset;
        HologramDisplay.Singleton.SetHologramToShortPillar();
    }

    public void SelectWall()
    {
        ObjectCreator.Singleton.ChangeToWall();
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

    public void OpenOptionsMenu()
    {
        optionsUI.SetActive(true);
        shapesUI.SetActive(false);
        saveUI.SetActive(false);
        loadUI.SetActive(false);
        wrapsUI.SetActive(false);
    }

    public void OpenShapesMenu()
    {
        optionsUI.SetActive(false);
        shapesUI.SetActive(true);
        saveUI.SetActive(false);
        loadUI.SetActive(false);
        wrapsUI.SetActive(false);
    }

    public void OpenWrapsMenu()
    {
        optionsUI.SetActive(false);
        shapesUI.SetActive(false);
        wrapsUI.SetActive(true);
        saveUI.SetActive(false);
        loadUI.SetActive(false);
    }

    public void OpenSaveMenu()
    {
        optionsUI.SetActive(false);
        shapesUI.SetActive(false);
        saveUI.SetActive(true);
        loadUI.SetActive(false);
        wrapsUI.SetActive(false);
    }

    public void OpenLoadMenu()
    {
        optionsUI.SetActive(false);
        shapesUI.SetActive(false);
        saveUI.SetActive(false);
        loadUI.SetActive(true);
        wrapsUI.SetActive(false);
    }

    public void SaveLevelWithButton(string saveID)
    {
        BlockRangler.SaveLevel("save" + saveID);
        OpenShapesMenu(); // Switches to catalog
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
        OpenShapesMenu();
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
