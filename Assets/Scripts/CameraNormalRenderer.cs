using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraNormalRenderer : MonoBehaviour
{
    public Material mat;

    private Camera cam;

    void Start()
    {
        this.cam = this.GetComponent<Camera>();
        this.cam.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
    }

    //method which is automatically called by unity after the camera is done rendering
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //get viewspace to worldspace matrix and pass it to shader
        Matrix4x4 viewToWorld = cam.cameraToWorldMatrix;
        this.mat.SetMatrix("_viewToWorld", viewToWorld);
        //draws the pixels from the source texture to the destination texture
        Graphics.Blit(source, destination, mat);
    }
}
