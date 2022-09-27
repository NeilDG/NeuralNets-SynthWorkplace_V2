using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraRecording : MonoBehaviour
{
    [SerializeField] private Camera cleanCamera;
    [SerializeField] private Camera albedoCamera;
    [SerializeField] private Camera normalCamera;
    [SerializeField] private Camera specularCamera;
    [SerializeField] private Camera smoothnessCamera;
    [SerializeField] private Camera depthCamera;
    [SerializeField] private Camera unlitCamera;

    private const string BASE_PATH = "E:/SynthWeather Dataset 9/";

    private const int SAVE_EVERY_FRAME = 20000;
    private const int MAX_IMAGES_TO_SAVE = 250;
    private const int MAX_IMAGES_TO_SAVE_DEBUG = 10;
    private const float TIME_SCALE = 100.0f;
    private const int CAPTURE_FRAME_RATE = 5;

    private const int UNLIT_SCENE_INDEX = 1;

    private int frames = 0;
    private int counter = 0;
    private string skyboxName;
    private string currentFolderDir;

    // Start is called before the first frame update
    void Start()
    {
        this.AttachCameras();

        if (RenderSettings.skybox != null)
        {
            this.skyboxName = RenderSettings.skybox.name;
        }
        else
        {
            this.skyboxName = "Skybox_Unlit";
        }
        
        this.currentFolderDir = BASE_PATH + SceneManager.GetActiveScene().name;
        Directory.CreateDirectory(this.currentFolderDir);
        Debug.Log("UNLIT FOLDER: " + this.currentFolderDir + "/unlit/");


        if (SceneManager.GetActiveScene().buildIndex == 2) //index 2 scene records scene intrinsics
        {
            Directory.CreateDirectory(this.currentFolderDir + "/albedo/");
            Directory.CreateDirectory(this.currentFolderDir + "/normal/");
            Directory.CreateDirectory(this.currentFolderDir + "/depth/");
            //Directory.CreateDirectory(this.currentFolderDir + "/specular/");
            //Directory.CreateDirectory(this.currentFolderDir + "/smoothness/");
        }
        else if (SceneManager.GetActiveScene().buildIndex == UNLIT_SCENE_INDEX) // index 1 scene records scene unlits
        {
            Directory.CreateDirectory(this.currentFolderDir + "/unlit/");
        }

        Time.timeScale = TIME_SCALE;
        Time.captureFramerate = CAPTURE_FRAME_RATE;

    }

    void AttachCameras()
    {
        this.cleanCamera = GameObject.Find("CleanCamera").GetComponent<Camera>();
        this.albedoCamera = GameObject.Find("AlbedoCamera").GetComponent<Camera>();
        this.normalCamera = GameObject.Find("NormalCamera").GetComponent<Camera>();
        this.specularCamera = GameObject.Find("SpecularCamera").GetComponent<Camera>();
        this.smoothnessCamera = GameObject.Find("SmoothnessCamera").GetComponent<Camera>();
        this.depthCamera = GameObject.Find("DepthCamera").GetComponent<Camera>();

        if (SceneManager.GetActiveScene().buildIndex == UNLIT_SCENE_INDEX)
        {
            this.unlitCamera = GameObject.Find("UnlitCamera").GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex > 0 && this.counter >= MAX_IMAGES_TO_SAVE)
        {
            Object.DestroyImmediate(this.gameObject);
            EventBroadcaster.Instance.PostEvent(EventNames.ON_RECORDING_FINISHED);
        }
        else if (sceneIndex == 0 && this.counter >= MAX_IMAGES_TO_SAVE_DEBUG)
        {
            Object.DestroyImmediate(this.gameObject);
            EventBroadcaster.Instance.PostEvent(EventNames.ON_RECORDING_FINISHED);
        }

        float multiplier = Time.captureDeltaTime * TIME_SCALE;
        this.frames += Mathf.RoundToInt(multiplier);

        if (this.frames < 80000) //skip first N frames
        {
            //Debug.Log("Skipping " + this.frames+ " for sync.");
            return;
        }

        if (this.frames % SAVE_EVERY_FRAME == 0)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                this.WriteAlbedoCam();
                this.WriteNormalCam();
                this.WriteDepthCam();
                this.WriteRGBCam();
            }
            else if (SceneManager.GetActiveScene().buildIndex == UNLIT_SCENE_INDEX)
            {
                this.WriteUnlitCam();
            }
            else
            {
                this.WriteRGBCam();
            }
            this.counter++;
        }

    }

    void WriteRGBCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.cleanCamera.targetTexture;

        this.cleanCamera.Render();

        int width = this.cleanCamera.targetTexture.width;
        int height = this.cleanCamera.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        //File.WriteAllBytes(this.currentFolderDir + "/rgb/synth_" + this.counter + ".png", Bytes);
        File.WriteAllBytes(this.currentFolderDir + "/synth_" + this.counter + ".png", Bytes);
        Debug.Log("Saved frame number: " + this.counter);
    }

    void WriteAlbedoCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.albedoCamera.targetTexture;

        this.albedoCamera.Render();

        int width = this.albedoCamera.targetTexture.width;
        int height = this.albedoCamera.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir + "/albedo/synth_" + this.counter + ".png", Bytes);
        Debug.Log("Saved frame number: " + this.counter);
    }

    void WriteUnlitCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.unlitCamera.targetTexture;

        this.unlitCamera.Render();

        int width = this.unlitCamera.targetTexture.width;
        int height = this.unlitCamera.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir + "/unlit/synth_" + this.counter + ".png", Bytes);
        Debug.Log("Saved frame number: " + this.counter);
    }

    void WriteNormalCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.normalCamera.targetTexture;

        this.normalCamera.Render();

        int width = this.normalCamera.targetTexture.width;
        int height = this.normalCamera.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir + "/normal/synth_" + this.counter + ".png", Bytes);
        Debug.Log("Saved frame number: " + this.counter);
    }

    void WriteDepthCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.depthCamera.targetTexture;

        this.depthCamera.Render();

        int width = this.depthCamera.targetTexture.width;
        int height = this.depthCamera.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir + "/depth/synth_" + this.counter + ".png", Bytes);
        Debug.Log("Saved frame number: " + this.counter);
    }
}
