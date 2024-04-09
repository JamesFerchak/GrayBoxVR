using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    private static Effects _singleton;
    public static Effects Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null) _singleton = value;
            else
            {
                Debug.LogWarning($"There is more than one effects! Killing self!!!");
                Destroy(value.gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioClip menuOpen;
    public AudioClip menuClick;
    public AudioClip closeMenu;
    public AudioClip placeBlock;
    public AudioClip deleteBlock;
    public AudioClip saveBuild;
    public AudioClip loadBuild;
    public AudioClip paintBlock;
    public AudioClip group;
    public AudioClip ungroup;
    public AudioClip undo;
    public AudioClip redo;
    public void playSound(Vector3 location, int effect_number)
    {
        switch (effect_number)
        {
            // AudioSource.PlayClipAtPoint(clickNoise, cam.transform.position);
            case 1:
                AudioSource.PlayClipAtPoint(menuOpen, location);
                break;
            case 2:
                AudioSource.PlayClipAtPoint(menuClick, location);
                break;
            case 3:
                AudioSource.PlayClipAtPoint(closeMenu, location);
                break;
            case 4:
                AudioSource.PlayClipAtPoint(placeBlock, location);
                break;
            case 5:
                AudioSource.PlayClipAtPoint(deleteBlock, location);
                break;
            case 6:
                AudioSource.PlayClipAtPoint(saveBuild, location);
                break;
            case 7:
                AudioSource.PlayClipAtPoint(loadBuild, location);
                break;
            case 8:
                AudioSource.PlayClipAtPoint(paintBlock, location);
                break;
            case 9:
                AudioSource.PlayClipAtPoint(menuOpen, location);
                break;
            case 10:
                AudioSource.PlayClipAtPoint(menuOpen, location);
                break;
            case 11:
                AudioSource.PlayClipAtPoint(menuOpen, location);
                break;
        }
    }



}
