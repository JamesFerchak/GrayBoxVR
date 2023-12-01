using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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

    [SerializeField] GameObject mainMenuCanvas;
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

        // Code to load images into load menu and save menu
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
        Application.Quit();
        Debug.Log("DEBUG: Quitting game...");
    }

    public void SelectSquare()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToSquare();
        catalogCurrentSelection.sprite = squareAsset;
        HologramDisplay.Singleton.SetHologramToCube();
    }

    public void SelectSphere()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToSphere();
        catalogCurrentSelection.sprite = sphereAsset;
        HologramDisplay.Singleton.SetHologramToSphere();
    }

    public void SelectCylinder()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToCylinder();
        catalogCurrentSelection.sprite = cylinderAsset;
        HologramDisplay.Singleton.SetHologramToCylinder();
    }

    public void SelectPyramid()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToPyramid();
        catalogCurrentSelection.sprite = pyramidAsset;
        HologramDisplay.Singleton.SetHologramToPyramid();
    }

    public void SelectFloor()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToFloor();
        catalogCurrentSelection.sprite = floorAsset;
        HologramDisplay.Singleton.SetHologramToFloor();
    }

    public void SelectPillar()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToPillar();
        catalogCurrentSelection.sprite = pillarAsset;
        HologramDisplay.Singleton.SetHologramToPillar();
    }

    public void SelectShortPillar()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToShortPillar();
        catalogCurrentSelection.sprite = shortPillarAsset;
        HologramDisplay.Singleton.SetHologramToShortPillar();
    }

    public void SelectWall()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToWall();
        catalogCurrentSelection.sprite = wallAsset;
        HologramDisplay.Singleton.SetHologramToWall();
    }

    public void RelocateMainMenu()
    {
        Vector3 newMainMenuPosition = cam.transform.TransformPoint(Vector3.forward * 2);
        newMainMenuPosition.y = 35;
        mainMenuCanvas.transform.position = newMainMenuPosition;
    }

    public void SelectRed()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "red";
    }
    public void SelectBlue()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "blue";
    }
    public void SelectYellow()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "yellow";
    }
    public void SelectWhite()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "white";
    }
    public void SelectBlack()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "black";
    }
    public void SelectOrange()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "orange";
    }
    public void SelectBrown()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "brown";
    }
    public void SelectGreen()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "green";
    }
    public void SelectPurple()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "purple";
    }
    public void SelectPink()
    {
        leftHandController.gameObject.GetComponent<PaletteScript>().current_wrap = "pink";
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
        leftHandController.gameObject.GetComponent<PaletteScript>().InteractWithMainMenu(); // Closes menu
        ScreenCapture.CaptureScreenshot(Application.persistentDataPath + "save" + saveID + "thumbnail.png"); // Saves to project directory
    }

    public void LoadLevelWithButton(string saveID)
    {
        BlockRangler.LoadLevel("save" + saveID);
        OpenShapesMenu();
        leftHandController.gameObject.GetComponent<PaletteScript>().InteractWithMainMenu();
    }
}
