using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraDisableSpriteBehavior : MonoBehaviour
{
    [SerializeField] private GameObject occluderContainer;
    private Camera camera;

    //private float originalAmbientIntensity;
    // or get it on runtime
    private void Awake()
    {
        this.camera = this.GetComponent<Camera>();
    }

    private void OnEnable()
    {
        // register the callbacks when enabling object
        Camera.onPreRender += MyPreRender;
        Camera.onPostRender += MyPostRenderer;
    }

    private void OnDisable()
    {
        // remove the callbacks when disabling object
        Camera.onPreRender -= MyPreRender;
        Camera.onPostRender -= MyPostRenderer;
    }

    // callback before ANY camera starts rendering
    private void MyPreRender(Camera cam)
    {
        // if mainCamera set to originalShadowSettings 
        // could also simply return but just to be sure
        //
        // for other camera disable shadows
        if (cam == this.camera)
        {
            this.occluderContainer.SetActive(false);
        }
    }

    // callback after ANY camera finishes rendering
    private void MyPostRenderer(Camera cam)
    {
        if (cam == this.camera)
        {
            this.occluderContainer.SetActive(true);
        }
    }
}