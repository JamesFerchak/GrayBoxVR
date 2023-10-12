using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActions : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("DEBUG: Quitting game...");
    }
}
