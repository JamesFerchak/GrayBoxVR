using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{

    Camera AttachedCamera;
    public Shader Post_Outline;
    public Shader DrawSimple;
    Camera TempCam;
    //public RenderTexture TempRT

    private void Awake()
    {
        AttachedCamera = GetComponent<Camera>();
        TempCam = new GameObject().AddComponent<Camera>();
        TempCam.enabled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //set up a temporary camera
        TempCam.CopyFrom(AttachedCamera);
        TempCam.clearFlags = CameraClearFlags.Color;
        TempCam.backgroundColor = Color.black;

        //cull any layer that isn't the outline
        TempCam.cullingMask = 1 << LayerMask.NameToLayer("Outline");

        //make the temporary rendertexture
        RenderTexture TempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);

        //put it to video memory
        TempRT.Create();

        //set the camera's target texture when rendering
        TempCam.targetTexture = TempRT;

        //render all objects this camera can render, but with our custom shader.
        TempCam.RenderWithShader(DrawSimple, "");

        //copy the temporary RT to the final image
        Graphics.Blit(TempRT, destination);

        //release the temporary RT
        TempRT.Release();
    }
}
