using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessChanger : MonoBehaviour
{
    [SerializeField] private PostProcessVolume noisyVolume;
    [SerializeField] private PostProcessVolume cleanVolume;

    private float ticks = 0.0f;
    private float INTERVAL = 0.25f;

    private float HUE_SHIFT_INTERVAL = 25.0f;
    private const float MAX_HUE = 180.0f;
    private const float MIN_HUE = -180.0f;
    private float currentHue = 0.0f;

    bool isPositive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.ticks += Time.deltaTime;
        if(this.ticks > INTERVAL)
        {
            this.ticks = 0.0f;
            this.HUE_SHIFT_INTERVAL = Random.Range(1.0f, 25.0f);
            if(this.currentHue < MAX_HUE && this.isPositive)
            {
                this.currentHue += HUE_SHIFT_INTERVAL;

                if(this.currentHue > MAX_HUE)
                {
                    this.isPositive = false;
                }
            }
            else if(this.currentHue > MIN_HUE && !this.isPositive)
            {
                this.currentHue -= HUE_SHIFT_INTERVAL;

                if(this.currentHue < MIN_HUE)
                {
                    this.isPositive = true;
                }
            }

            float contrast = Random.Range(0.0f, 100.0f);
            ColorGrading colorGrading;
            this.noisyVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
            colorGrading.hueShift.value = currentHue;
            colorGrading.contrast.value = contrast;

            this.cleanVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
            colorGrading.hueShift.value = currentHue;
            colorGrading.contrast.value = contrast;

            //Debug.Log("Hue is set to: " + this.currentHue + " Contrast is: " + contrast);
        }
    }
}
