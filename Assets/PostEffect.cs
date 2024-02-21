using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{

    Camera AttachedCamera;
    public Shader Post_Outline;

    private void Awake()
    {
        AttachedCamera = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
    }
}
