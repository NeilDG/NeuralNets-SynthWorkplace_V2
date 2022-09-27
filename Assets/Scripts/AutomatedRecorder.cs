using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomatedRecorder : MonoBehaviour
{
    private int index = 0;
    private static string[] assetPaths;
    [SerializeField] private GameObject cameraRecorder;

    private enum RecordingType
    {
        RECORD_INTRINSICS = 0,
        RECORD_RGB = 1,
        DEBUG = 2
    };

    [SerializeField] private RecordingType recordingType;

    [SerializeField] private CameraRecordingV2.ShadowDatasetType shadowDatasetType;

    //private Camera cameraViewRef;
    //private Transform cameraViewParent;
    //private Camera cameraView;

    // Start is called before the first frame update
    void Start()
    {
        LoadNextScene();

        GameObject.DontDestroyOnLoad(this.gameObject);
        EventBroadcaster.Instance.AddObserver(EventNames.ON_RECORDING_FINISHED, OnRecordingFinished);
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        Time.captureFramerate = 5;
    }

    private void LoadNextScene()
    {
        int maxCount;
        if (recordingType == RecordingType.DEBUG)
        {
            maxCount = 1;
        }
        else if (recordingType == RecordingType.RECORD_RGB)
        {
            maxCount = SceneManager.sceneCountInBuildSettings;
        }
        else
        {
            maxCount = 2;
        }

        if (this.index > 0 && this.index < maxCount)
        {
            SceneManager.LoadScene(index);
        }
        else if (this.index == 0)
        {
            this.SceneManager_sceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.StartCoroutine(this.DelayRecordStart());
        //this.cameraViewRef = GameObject.Find("CleanCamera").GetComponent<Camera>();
        //this.cameraViewParent = this.cameraViewRef.transform.parent;

        //this.cameraView = GameObject.Instantiate(this.cameraViewRef);
        //Component.Destroy(this.cameraView.GetComponent<AudioListener>());
        //this.cameraView.targetTexture = null;
        //this.cameraView.transform.SetParent(this.cameraViewParent);
    }

    private IEnumerator DelayRecordStart()
    {
        yield return new WaitForSeconds(Time.captureDeltaTime * 1.0f);
        this.index++;
        GameObject.Instantiate(this.cameraRecorder).GetComponent<CameraRecordingV2>().SetShadowDatasetType(this.shadowDatasetType);
    }

    private void OnRecordingFinished()
    {
        Debug.Log("Finished recording scene");
        this.LoadNextScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
