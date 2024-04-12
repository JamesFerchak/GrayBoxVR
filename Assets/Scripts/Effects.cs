using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    void Awake()
    {
        Singleton = this;
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

    public ParticleSystem poofEffect;

    public bool soundsEnabled = true;
    public bool effectsEnabled = true;

    public void PlaySound(Vector3 location, int effect_number)
    {
        if (!soundsEnabled)
        {
            return;
        }
        switch (effect_number)
        {
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
                AudioSource.PlayClipAtPoint(group, location);
                break;
            case 10:
                AudioSource.PlayClipAtPoint(ungroup, location);
                break;
            case 11:
                AudioSource.PlayClipAtPoint(undo, location);
                break;
            case 12:
                AudioSource.PlayClipAtPoint(redo, location);
                break;
        }
    }

    public void PlayEffect(Vector3 location)
    {
        if (!effectsEnabled)
        {
            return;
        }
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, location);
        Instantiate(poofEffect, location, rotation);
    }
}
