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
    [SerializeField] GameObject catalogUI;
    [SerializeField] GameObject saveUI;
    [SerializeField] GameObject loadUI;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject rightHandController;

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
            scalingAssistanceText.text = (1.0f / (6.0f - value)).ToString("0.00");
        });
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
    }

    public void SelectSphere()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToSphere();
        catalogCurrentSelection.sprite = sphereAsset;
    }

    public void SelectCylinder()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToCylinder();
        catalogCurrentSelection.sprite = cylinderAsset;
    }

    public void SelectPyramid()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToPyramid();
        catalogCurrentSelection.sprite = pyramidAsset;
    }

    public void SelectFloor()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToFloor();
        catalogCurrentSelection.sprite = floorAsset;
    }

    public void SelectPillar()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToPillar();
        catalogCurrentSelection.sprite = pillarAsset;
    }

    public void SelectShortPillar()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToShortPillar();
        catalogCurrentSelection.sprite = shortPillarAsset;
    }

    public void SelectWall()
    {
        rightHandController.gameObject.GetComponent<PaletteScript>().ChangeToWall();
        catalogCurrentSelection.sprite = wallAsset;
    }

    public void RelocateMainMenu()
    {
        Vector3 newMainMenuPosition = cam.transform.TransformPoint(Vector3.forward * 2);
        newMainMenuPosition.y = 35;
        mainMenuCanvas.transform.position = newMainMenuPosition;
    }

    public void OpenOptionsMenu()
    {
        optionsUI.SetActive(true);
        catalogUI.SetActive(false);
        saveUI.SetActive(false);
        loadUI.SetActive(false);

    }

    public void OpenCatalogMenu()
    {
        optionsUI.SetActive(false);
        catalogUI.SetActive(true);
        saveUI.SetActive(false);
        loadUI.SetActive(false);
    }

    public void OpenSaveMenu()
    {
        optionsUI.SetActive(false);
        catalogUI.SetActive(false);
        saveUI.SetActive(true);
        loadUI.SetActive(false);
    }

    public void OpenLoadMenu()
    {
        optionsUI.SetActive(false);
        catalogUI.SetActive(false);
        saveUI.SetActive(false);
        loadUI.SetActive(true);
    }

    public void SaveLevelWithButton()
    {
        BlockRangler.SaveLevel();
        OpenCatalogMenu(); // Switches to catalog
        rightHandController.gameObject.GetComponent<PaletteScript>().InteractWithMainMenu(); // Closes menu
    }

    public void LoadLevelWithButton()
    {
        BlockRangler.LoadLevel();
        OpenCatalogMenu();
        rightHandController.gameObject.GetComponent<PaletteScript>().InteractWithMainMenu();
    }
}
