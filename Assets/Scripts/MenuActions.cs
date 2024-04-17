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
using System;
using Environment = System.Environment;

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

    // CORE MENU OBJECTS
    [SerializeField] GameObject mainMenuCanvas; // Cannot be deleted
    [SerializeField] GameObject mainMenuPanel; // Can be made visible at will
    [SerializeField] GameObject[] mainMenuTabs;
    [SerializeField] GameObject[] mainMenuButtons;
    public bool inMenuMode; // True if the menu is open

    // OPTIONS MENU OBJECTS
    [SerializeField] GameObject[] optionsTabs;
    [SerializeField] GameObject[] optionsButtons;
    [SerializeField] Slider placementAssistanceSlider;
    [SerializeField] Text placementAssistanceText;
    [SerializeField] Slider rotationAssistanceSlider;
    [SerializeField] Text rotationAssistanceText;
    [SerializeField] Slider scalingAssistanceSlider;
    [SerializeField] Text scalingAssistanceText;
    public bool controllerUIOff = false; // True if the controller ui is turned off
    public bool closeMainMenuEnabled = false;

    // SHAPES MENU OBJECTS
    [SerializeField] Image[] shapeButtonThumbnails;
    [SerializeField] GameObject[] shapeButtons;
    int lastSelectedShapeIndex = 0; // Cube

    // PROJECTS MENU OBJECTS
    [SerializeField] GameObject[] projectThumbnails = new GameObject[10];
    [SerializeField] GameObject[] loadProjectButtons = new GameObject[10];
    bool[] projectExists = new bool[10];
    string levelPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("\\", "/") + "/GrayboxVR/";

    // WRAPS MENU OBJECTS
    [SerializeField] GameObject[] wrapButtons;
    int lastSelectedWrapIndex = 0; // Red

    // WORLD MENU OBJECTS
    [SerializeField] GameObject[] skyButtons;
    int lastSelectedSkyIndex = 0; // Default
    [SerializeField] GameObject[] floorButtons;
    int lastSelectedFloorIndex = 0; // Default/White

    // OTHER OBJECTS
    [SerializeField] GameObject cam;
    [SerializeField] GameObject rightHandController;
    [SerializeField] GameObject leftHandController;
    public GameObject leftControllerUI;
    public GameObject leftControllerUIAlt;
    public GameObject rightControllerUI;
    public GameObject rightControllerUIAlt;
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

        // Prepare UI and game for start
        RefreshShapeThumbnails();
        RefreshSavedProjects();
        wrapButtons[lastSelectedWrapIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        shapeButtons[lastSelectedShapeIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        SelectSky(0);
        SelectFloor(0);
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
        newMainMenuPosition.y = cam.transform.position.y;
        mainMenuCanvas.transform.position = newMainMenuPosition;
    }

    public void InteractWithMainMenu()
    {
        if (mainMenuPanel != null) // If panel exists
        {
            if (inMenuMode) // If panel is already open
            {
                Effects.Singleton.PlaySound(cam.transform.position, 3);
                mainMenuPanel.SetActive(false); // Close panel
                inMenuMode = false;
            }
            else // If panel is closed
            {
                Effects.Singleton.PlaySound(cam.transform.position, 1);
                RefreshSavedProjects();
                RelocateMainMenu();
                mainMenuPanel.SetActive(true); // Open panel
                inMenuMode = true;
            }
        }
    }

    public void CloseMainMenu()
    {
        if (closeMainMenuEnabled)
        {
            if (mainMenuPanel != null) // If panel exists
            {
                if (inMenuMode) // If panel is already open
                {
                    Effects.Singleton.PlaySound(cam.transform.position, 3);
                    mainMenuPanel.SetActive(false); // Close panel
                    inMenuMode = false;
                }
            }
        }
    }

    public void SwitchMenuTabs(int tabID) // 0: Options, 1: Shapes, 2: Save, 3: Load, 4: Wraps
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        // Closes menu tabs except for the given tabID
        for (int i = 0; i < mainMenuTabs.Length; i++)
        {
            mainMenuTabs[i].SetActive(false);
            mainMenuButtons[i].SetActive(true);
        }
        mainMenuTabs[tabID].SetActive(true);
        mainMenuButtons[tabID].SetActive(false);
    }

    public void SwitchOptionsTabs(int tabID) // 0: Settings, 1: Controls, 2: Quit, 3: Sharing Projects
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        // Closes tabs except for the given tabID
        for (int i = 0; i < optionsTabs.Length; i++)
        {
            optionsTabs[i].SetActive(false);
            optionsButtons[i].SetActive(true);
        }
        optionsTabs[tabID].SetActive(true);
        optionsButtons[tabID].SetActive(false);
    }

    public void SelectShape(string shapeID)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        ObjectCreator.Singleton.currentObjectType = ObjectDefinitions.Singleton.GetObjectShape(shapeID);
        HologramDisplay.Singleton.SetHologramToShape(shapeID);

        int nextSelectedShapeIndex = int.Parse(shapeID);
        shapeButtons[lastSelectedShapeIndex].GetComponent<Image>().color = UnityEngine.Color.white;
        shapeButtons[nextSelectedShapeIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        lastSelectedShapeIndex = nextSelectedShapeIndex;
    }

    public void SelectColor(string color)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        ObjectPainter.Singleton.current_wrap = color;

        int nextSelectedWrapIndex = ObjectPainter.Singleton.GetButtonIndexOfWrap(color);
        wrapButtons[lastSelectedWrapIndex].GetComponent<Image>().color = UnityEngine.Color.white;
        wrapButtons[nextSelectedWrapIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        lastSelectedWrapIndex = nextSelectedWrapIndex;
    }

    public void SelectSky(int sky)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        SkyboxChoice.Singleton.ChangeSkybox(sky);

        int nextSelectedSkyIndex = sky;
        skyButtons[lastSelectedSkyIndex].GetComponent<Image>().color = UnityEngine.Color.white;
        skyButtons[nextSelectedSkyIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        lastSelectedSkyIndex = nextSelectedSkyIndex;
    }

    public void SelectFloor(int floor)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        SkyboxChoice.Singleton.ChangeFloor(floor);

        int nextSelectedFloorIndex = floor;
        floorButtons[lastSelectedFloorIndex].GetComponent<Image>().color = UnityEngine.Color.white;
        floorButtons[nextSelectedFloorIndex].GetComponent<Image>().color = UnityEngine.Color.cyan;
        lastSelectedFloorIndex = nextSelectedFloorIndex;
    }

    public void SaveLevelWithButton(string saveID)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 6);
        BlockRangler.SaveLevel("save" + saveID);
        SwitchMenuTabs(0); // Switches to Options tab
        InteractWithMainMenu(); // Closes menu
        ScreenCapture.CaptureScreenshot(levelPath + "save" + saveID + "thumbnail.png"); // Saves to project directory


        if (!projectExists[(int)saveID[0] - 65])
        {
            loadProjectButtons[(int)saveID[0] - 65].SetActive(true);
            projectExists[(int)saveID[0] - 65] = true;
        }
    }

    public void LoadLevelWithButton(string saveID)
    {
        Effects.Singleton.PlaySound(cam.transform.position, 7);
        BlockRangler.LoadLevel("save" + saveID);
        SwitchMenuTabs(0); // Switches to Options tab
        InteractWithMainMenu(); // Closes menu
    }

    public void RefreshSavedProjects()
    {
        Texture2D textureConverter = new Texture2D(64, 64);
        byte[] bytes;
        Rect dimensions;

        for (int i = 65; i < 75; i++)
        {
            char cID = (char)i; // Converts int into ASCII character
            int iID = i - 65; // Used for iterating through project list

            // Find and load projects
            if (File.Exists(levelPath + "save" + cID + ".kek"))
            {
                loadProjectButtons[iID].SetActive(true);
                projectExists[iID] = true;
            }
            else // If no project exists
            {
                loadProjectButtons[iID].SetActive(false);
                projectExists[iID] = false;
            }

            // Find and load thumbnails (not necessary)
            if (File.Exists(levelPath + "save" + cID + "thumbnail.png"))
            {
                bytes = File.ReadAllBytes(levelPath + "save" + cID + "thumbnail.png");
                textureConverter.LoadImage(bytes);
                dimensions = new Rect(0, 0, textureConverter.width, textureConverter.height);
                Sprite newThumbnail = Sprite.Create(textureConverter, dimensions, new Vector2(), 100.0f);
                newThumbnail.name = "sprite" + cID;
                projectThumbnails[iID].GetComponent<Image>().sprite = newThumbnail;
                textureConverter = new Texture2D(64, 64);
            }
        }
    }

    public void RefreshShapeThumbnails()
    {
        for (int i = 0; i < shapeButtonThumbnails.Length; i++)
        {
            string shapeID = i.ToString();
            Sprite objectSprite = ObjectDefinitions.Singleton.GetObjectSprite(shapeID);
            shapeButtonThumbnails[i].GetComponent<Image>().sprite = objectSprite;
        }
    }

    public void AltControlSchemeToggle()
    {
        LeftHandController.Singleton.switchAltControlScheme();
    }

    public void QuitGame()
    {
        Effects.Singleton.PlaySound(cam.transform.position, 2);
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void ToggleControllerUI()
    {
        if (controllerUIOff == false)
        {
            controllerUIOff = true;
            leftControllerUI.SetActive(false);
            leftControllerUIAlt.SetActive(false);
            rightControllerUI.SetActive(false);
            rightControllerUIAlt.SetActive(false);
        }
        else
        {
            controllerUIOff = false;
            LeftHandController.Singleton.checkAltControls();
        }
    }

    public void ToggleSoundEffects()
    {
        if (Effects.Singleton.soundsEnabled)
        {
            Effects.Singleton.soundsEnabled = false;
        }
        else
        {
            Effects.Singleton.soundsEnabled = true;
        }
    }

    public void ToggleVisualEffects()
    {
        if (Effects.Singleton.effectsEnabled)
        {
            Effects.Singleton.effectsEnabled = false;
        }
        else
        {
            Effects.Singleton.effectsEnabled = true;
        }
    }

    public void ToggleMenuExitOnButtonPress()
    {
        if (closeMainMenuEnabled)
        {
            closeMainMenuEnabled = false;
        }
        else
        {
            closeMainMenuEnabled = true;
        }
    }
}
