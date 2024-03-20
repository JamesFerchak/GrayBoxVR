using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using System.IO;
using Image = UnityEngine.UI.Image;
using UnityEngine.ProBuilder.Shapes;
using Sprite = UnityEngine.Sprite;

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
    [SerializeField] GameObject[] mainMenuButtons;

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
    [SerializeField] Image[] shapeButtonThumbnails; 

    Sprite[] levelSpriteArray = new Sprite[10];
    [SerializeField] GameObject[] levelThumbnailsLoadMenu = new GameObject[10];
    [SerializeField] GameObject[] levelThumbnailsSaveMenu = new GameObject[10];
    [SerializeField] GameObject[] loadButtons = new GameObject[10];
    bool[] projectExists = new bool[10];

    public bool inMenuMode; // True if the menu is open

    public AudioClip clickNoise;

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

        RefreshShapeThumbnails();
        RefreshSaveThumbnails();
        catalogCurrentSelection.sprite = ObjectDefinitions.Singleton.GetObjectSprite("0");
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

    public void RelocateMainMenu()
    {
        Vector3 newMainMenuPosition = cam.transform.TransformPoint(Vector3.forward * 2);
        newMainMenuPosition.y = 35;
        mainMenuCanvas.transform.position = newMainMenuPosition;
    }

    public void InteractWithMainMenu()
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
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

    public void SwitchMenuTabs(int tabID) // 0: Options, 1: Shapes, 2: Save, 3: Load, 4: Wraps
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
        // Closes menu tabs except for the given tabID
        for (int i = 0; i < mainMenuTabs.Length; i++)
        {
            mainMenuTabs[i].SetActive(false);
            mainMenuButtons[i].SetActive(true);
        }
        mainMenuTabs[tabID].SetActive(true);
        mainMenuButtons[tabID].SetActive(false);
    }

    public void SelectShape(string shapeID)
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
        ObjectCreator.Singleton.currentObjectType = ObjectDefinitions.Singleton.GetObjectShape(shapeID);
        catalogCurrentSelection.sprite = ObjectDefinitions.Singleton.GetObjectSprite(shapeID);
        HologramDisplay.Singleton.SetHologramToShape(shapeID);
    }

    public void SelectColor(string color)
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
        ObjectPainter.Singleton.current_wrap = color;
    }

    public void SaveLevelWithButton(string saveID)
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
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
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
        BlockRangler.LoadLevel("save" + saveID);
        SwitchMenuTabs(0); // Switches to Options tab
        InteractWithMainMenu();
    }

    public void RefreshSaveThumbnails()
    {
        Texture2D textureConverter = new Texture2D(2, 2);
        byte[] bytes;

        string filePath = Application.persistentDataPath + "/";
        for (int i = 65; i < 75; i++)
        {
            if (File.Exists(filePath + "save" + (char)i + "thumbnail.png"))
            {
                bytes = File.ReadAllBytes(filePath + "save" + (char)i + "thumbnail.png");
                textureConverter.LoadImage(bytes);
                levelSpriteArray[i - 65] = Sprite.Create(textureConverter, new Rect(0, 0, textureConverter.width, textureConverter.height), new Vector2(), 100.0f);
                levelSpriteArray[i - 65].name = "sprite" + (char)i;
                levelThumbnailsLoadMenu[i - 65].GetComponent<Image>().sprite = levelSpriteArray[i - 65];
                levelThumbnailsSaveMenu[i - 65].GetComponent<Image>().sprite = levelSpriteArray[i - 65];
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

    public void RefreshShapeThumbnails()
    {
        StartCoroutine(RefreshShapeThumbnailsCoroutine());
    }

    private IEnumerator RefreshShapeThumbnailsCoroutine()
    {
        for (int i = 0; i < shapeButtonThumbnails.Length; i++)
        {
            string shapeID = i.ToString();
            yield return StartCoroutine(ObjectDefinitions.Singleton.GenerateObjectSprite(shapeID)); // Wait for coroutine to finish
            shapeButtonThumbnails[i].GetComponent<Image>().sprite = ObjectDefinitions.Singleton.GetObjectSprite(shapeID);
        }
    }

    public void AltControlSchemeToggle()
    {
        LeftHandController.Singleton.switchAltControlScheme();
    }


    public void QuitGame()
    {
        AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
