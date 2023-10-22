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
    [SerializeField] GameObject menuUI;
    [SerializeField] GameObject catalogUI;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject rightHandController;

    [SerializeField] Image catalogCurrentSelection;
    [SerializeField] Sprite squareAsset;
    [SerializeField] Sprite sphereAsset;

    private void Awake()
    {
        Singleton = this;
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

    public void SwapMenu()
    {
        if (menuUI.active)
        {
            menuUI.SetActive(false);
            catalogUI.SetActive(true);
        }
        else
        {
            menuUI.SetActive(true);
            catalogUI.SetActive(false);
        }
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

    public void RelocateMainMenu()
    {
        Vector3 newMainMenuPosition = cam.transform.TransformPoint(Vector3.forward * 2);
        newMainMenuPosition.y = 35;
        mainMenuCanvas.transform.position = newMainMenuPosition; 
    }
}
