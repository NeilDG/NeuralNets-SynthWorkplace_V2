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
    [SerializeField] private Camera cleanCamera;

    public enum ShadowDatasetType
    {
        WITH_SHADOWS,
        NO_SHADOWS,
    }

    [SerializeField] private ShadowDatasetType shadowDatasetType;

    private const string BASE_PATH = "E:/SynthWeather Dataset 10/";

    public static int SAVE_EVERY_FRAME = 20000;
    private const int MAX_IMAGES_TO_SAVE = 250000;
    private const int MAX_IMAGES_TO_SAVE_DEBUG = 10;
    private const int CAPTURE_FRAME_RATE = 5;

    private int frames = 0;
    private int counter = 0;
    private const int STARTING_IMG_INDEX = 14298;

    private string currentFolderDir;
    private string[] skyboxList;

    static string[] PopulateSkyboxes()
    {
        string[] guids = AssetDatabase.FindAssets("equirect t:material");
        string[] assetPaths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }

        return assetPaths;
    }

    void AttachCameras()
    {
        this.cleanCamera = GameObject.Find("CleanCamera").GetComponent<Camera>();
    }

    void CreateSubFolder(string sceneName)
    {
        if (this.shadowDatasetType == ShadowDatasetType.WITH_SHADOWS)
        {
            this.currentFolderDir = BASE_PATH + "/rgb/" + sceneName + "/";
            Directory.CreateDirectory(this.currentFolderDir);
        }
        else
        {
            this.currentFolderDir = BASE_PATH + "/rgb_noshadows/" + sceneName + "/";
            Directory.CreateDirectory(this.currentFolderDir);
        }
    }

    public void SetShadowDatasetType(ShadowDatasetType datasetType)
    {
        this.shadowDatasetType = datasetType;
    }

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(1);

        this.AttachCameras();
        Time.captureFramerate = CAPTURE_FRAME_RATE;

        this.skyboxList = PopulateSkyboxes();

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

        float multiplier = Time.captureDeltaTime * Time.timeScale;
        this.frames += Mathf.RoundToInt(multiplier);

;       if (this.frames < 8000) //skip first N frames
        {
            Debug.Log("Skipping " + this.frames+ " for sync.");
            return;
        }

        if (this.frames % SAVE_EVERY_FRAME == 0)
        {
            if (this.counter >= STARTING_IMG_INDEX) //skip writing
            {
                this.WriteRGBCam();
            }
            else
            {
                if(this.counter % 1000 == 0)
                    Debug.Log("Skipping img save counter: " + this.counter + ".");
            }
            this.counter++;
        }

        /*if (this.frames % (SAVE_EVERY_FRAME * 10) == 0)
        {
            Debug.Log("Clearing image dictionary.");
            DatasetLoader.GetInstance().ClearImageDictionary();
        }*/
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
        //RenderTexture.active = currentRT
        RenderTexture.active = null;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(this.currentFolderDir + "/synth_" + this.counter + ".png", Bytes);
        //Debug.Log("Saved frame number: " + this.counter);
    }
}
