using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
using UnityEngine.XR;

[ExecuteInEditMode]
public class CameraShadowRenderer : MonoBehaviour
{
    [SerializeField] private Material shadowMaterial;
    [SerializeField] private RenderTexture shadowMapCopyRT;
    [SerializeField] private Light light;
    
    private CommandBuffer cmd;

    void Start()
    {
        cmd = new CommandBuffer(); 
        cmd.Blit(null, this.shadowMapCopyRT, this.shadowMaterial);
        cmd.SetGlobalTexture("_MyScreenSpaceShadows", this.shadowMapCopyRT);
        //light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, cmd);
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterForwardOpaque, cmd);

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //Debug.Log("rendering shadow");
        //Graphics.Blit(source, destination, this.shadowMaterial);
    }
}