using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraRigRandomizer : MonoBehaviour
{
    private Transform cameraRig;

    [SerializeField] private PostProcessVolume postProcessVolume;
    private PostProcessProfile postProcessProfile;

    private Vector3 minPos;
    private Vector3 maxPos;

    private Vector3 minRot;
    private Vector3 maxRot;

    private const float INTERVAL = 1.0f;
    private float ticks = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.postProcessProfile = this.postProcessVolume.profile;
        this.cameraRig = this.gameObject.transform;
        this.minPos = new Vector3(-340.0f, 40.0f, -265.0f);
        this.maxPos = new Vector3(350.0f, 80.0f, 105.0f);

        //this.minRot = Vector3.zero;
        this.minRot = new Vector3(55.0f, 55.0f, 55.0f);
        this.maxRot = new Vector3(90.0f, 90.0f, 0.0f);

        //this.RandomizeCamera();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*this.ticks += Time.captureDeltaTime;
        if (this.ticks >= INTERVAL)
        {
            this.ticks = 0.0f;
            this.RandomizeCamera();
        }*/

        //this.RandomizeCamera();
        //this.RandomizePostProcess();
    }

    private void RandomizeCamera()
    {
        Vector3 localPos = this.cameraRig.localPosition;
        localPos.x = Random.Range(this.minPos.x, this.maxPos.x);
        localPos.y = Random.Range(this.minPos.y, this.maxPos.y);
        localPos.z = Random.Range(this.minPos.z, this.maxPos.z);

        this.cameraRig.localPosition = localPos;

        Vector3 localRot = this.cameraRig.localEulerAngles;
        localRot.x = Random.Range(this.minRot.x, this.maxRot.x);
        localRot.y = Random.Range(this.minRot.y, this.maxRot.y);

        this.cameraRig.localEulerAngles = localRot;
    }

    private void RandomizePostProcess()
    {
        ColorGrading colorGrading = this.postProcessProfile.GetSetting<ColorGrading>();
        //colorGrading.postExposure.value = Random.Range(-0.75f, 0.75f);
        colorGrading.saturation.value = Random.Range(0.0f, 100.0f);
        colorGrading.contrast.value = Random.Range(0.0f, 100.0f);

        this.postProcessVolume.profile = this.postProcessProfile;
    }


}
