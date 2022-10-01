using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

/// <summary>
/// For shadow scene dataset recording
/// </summary>
public class CameraRecordingV2 : MonoBehaviour
{
    [SerializeField] private Camera cameraWithShadows;
    [SerializeField] private Camera cameraNoShadows;

    private const string BASE_PATH = "E:/SynthWeather Dataset 10_2/";

    public static int SAVE_EVERY_FRAME = 5;
    public static int REFRESH_SCENE_PER_FRAME = 1000;
    //public static int REFRESH_SCENE_PER_FRAME = SAVE_EVERY_FRAME * 20;

    private const int MAX_IMAGES_TO_SAVE = 250000;
    private const int MAX_IMAGES_TO_SAVE_DEBUG = 10;
    private const int CAPTURE_FRAME_RATE = 30;

    private long frames = 0;
    private int counter = 0;
    private const int STARTING_IMG_INDEX = 0;

    private string currentFolderDir_WithShadows;
    private string currentFolderDir_NoShadows;

    void AttachCameras()
    {
        this.cameraWithShadows = GameObject.Find("CameraWithShadows").GetComponent<Camera>();
        this.cameraNoShadows = GameObject.Find("CameraNoShadows").GetComponent<Camera>();
    }

    void CreateSubFolder(string sceneName)
    {
        /*if (this.shadowDatasetType == ShadowDatasetType.WITH_SHADOWS)
        {
            this.currentFolderDir = BASE_PATH + "/rgb/" + sceneName + "/";
            Directory.CreateDirectory(this.currentFolderDir);
        }
        else
        {
            this.currentFolderDir = BASE_PATH + "/rgb_noshadows/" + sceneName + "/";
            Directory.CreateDirectory(this.currentFolderDir);
        }*/

        this.currentFolderDir_WithShadows = BASE_PATH + "/rgb/" + sceneName + "/";
        Directory.CreateDirectory(this.currentFolderDir_WithShadows);

        this.currentFolderDir_NoShadows = BASE_PATH + "/rgb_noshadows/" + sceneName + "/";
        Directory.CreateDirectory(this.currentFolderDir_NoShadows);
    }

    /*public void SetShadowDatasetType(ShadowDatasetType datasetType)
    {
        this.shadowDatasetType = datasetType;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(this.counter);

        this.AttachCameras();
        Time.captureFramerate = CAPTURE_FRAME_RATE;

        string sceneName = SceneManager.GetActiveScene().name;
        this.CreateSubFolder(sceneName);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        string sceneName = SceneManager.GetActiveScene().name;
        if (this.counter >= MAX_IMAGES_TO_SAVE)
        {
            Debug.Log("Done saving images for skybox: " + sceneName);

            Object.DestroyImmediate(this.gameObject);
            EventBroadcaster.Instance.PostEvent(EventNames.ON_RECORDING_FINISHED);

            /*var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);*/
        }

        this.frames++;

;       if (this.frames < 100) //skip first N frames
        {
            Debug.Log("Skipping " + this.frames+ " for sync.");
            return;
        }

        if (this.frames % SAVE_EVERY_FRAME == 0)
        {
            if (this.counter >= STARTING_IMG_INDEX) //skip writing
            {
                this.WriteRGBCam();
                this.WriteRGBCam_NoShadows();
                //Debug.Log("Saving frame: " + this.frames);
            }
            else
            {
                if(this.counter % 1000 == 0)
                    Debug.Log("Skipping img save counter: " + this.counter + ".");
            }
            this.counter++;
        }

        if (this.frames % (REFRESH_SCENE_PER_FRAME * 10) == 0)
        {
            Debug.Log("Clearing image dictionary.");
            DatasetLoader.GetInstance().ClearImageDictionary();
        }
    }

    void WriteRGBCam()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.cameraWithShadows.targetTexture;

        this.cameraWithShadows.Render();

        int width = this.cameraWithShadows.targetTexture.width;
        int height = this.cameraWithShadows.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        //RenderTexture.active = currentRT
        RenderTexture.active = null;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir_WithShadows + "/synth_" + this.counter + ".png", Bytes);
        //Debug.Log("Saved frame number: " + this.counter);
    }

    void WriteRGBCam_NoShadows()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = this.cameraNoShadows.targetTexture;

        this.cameraNoShadows.Render();

        int width = this.cameraNoShadows.targetTexture.width;
        int height = this.cameraNoShadows.targetTexture.height;

        Texture2D Image = new Texture2D(width, height);
        Image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Image.Apply();
        //RenderTexture.active = currentRT
        RenderTexture.active = null;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir_NoShadows + "/synth_" + this.counter + ".png", Bytes);
        //Debug.Log("Saved frame number: " + this.counter);
    }
}
