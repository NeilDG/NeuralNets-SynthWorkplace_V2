using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ShadowRandomizer : MonoBehaviour
{
    [SerializeField] private Transform[] objectList;
    [SerializeField] private Light directionalLight;

    [SerializeField] private float MIN_POS_X = -7.0f;
    [SerializeField] private float MAX_POS_X = 7.0f;

    [SerializeField] private float MIN_POS_Y = -5.0f;
    [SerializeField] private float MAX_POS_Y = 5.0f;

    [SerializeField] private float MIN_SCALE_X = 5.0f;
    [SerializeField] private float MIN_SCALE_Y = 5.0f;

    [SerializeField] private float MAX_SCALE_X = 70.0f;
    [SerializeField] private float MAX_SCALE_Y = 70.0f;

    private const float MIN_ROT_HORIZONTAL = -85.0f;
    private const float MAX_ROT_HORIZONTAL = 85.0f;
    private const float MIN_ROT_VERTICAL = -30.0f;
    private const float MAX_ROT_VERTICAL = 30.0f;

    private const float INTERVAL = 1.0f;
    private float ticks = 0.0f;

    private Color[] rwLightColors;

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f, float mean = 0.5f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState(1);
    }

    // Update is called once per frame
    void Update()
    {
        this.InitializeLightColors();
        this.StartRandomization();
        this.RandomizeLightDirection();
    }

    private void InitializeLightColors()
    {
        this.rwLightColors = new Color[6];
        //from http://planetpixelemporium.com/tutorialpages/light.html
        this.rwLightColors[0].r = 255.0f/255.0f;
        this.rwLightColors[0].g = 197.0f/255.0f;
        this.rwLightColors[0].b = 143.0f/255.0f;

        this.rwLightColors[1].r = 255.0f/255.0f;
        this.rwLightColors[1].g = 214.0f/255.0f;
        this.rwLightColors[1].b = 170.0f/255.0f;

        this.rwLightColors[2].r = 255.0f/255.0f;
        this.rwLightColors[2].g = 241.0f/255.0f;
        this.rwLightColors[2].b = 224.0f/255.0f;

        this.rwLightColors[3].r = 255.0f/255.0f;
        this.rwLightColors[3].g = 250.0f/255.0f;
        this.rwLightColors[3].b = 244.0f/255.0f;

        this.rwLightColors[4].r = 255.0f/255.0f;
        this.rwLightColors[4].g = 255.0f/255.0f;
        this.rwLightColors[4].b = 251.0f/255.0f;

        this.rwLightColors[5].r = 255.0f/255.0f;
        this.rwLightColors[5].g = 255.0f/255.0f;
        this.rwLightColors[5].b = 255.0f/255.0f;

        /*this.rwLightColors[6].r = 201.0f/255.0f;
        this.rwLightColors[6].g = 226.0f/255.0f;
        this.rwLightColors[6].b = 255.0f/255.0f;

        this.rwLightColors[7].r = 64.0f/255.0f;
        this.rwLightColors[7].g = 156.0f/255.0f;
        this.rwLightColors[7].b = 255.0f/255.0f;*/

    }
    private void StartRandomization()
    {
        for (int i = 0; i < objectList.Length; i++)
        {
            float chance = Random.Range(0.0f, 1.0f);

            if (chance < 0.5f)
            {
                this.objectList[i].gameObject.SetActive(false);
            }
            else
            {
                this.objectList[i].gameObject.SetActive(true);
                Vector3 pos = this.objectList[i].localPosition;
                Vector3 rotAngles = this.objectList[i].localEulerAngles;
                Vector3 scale = this.objectList[i].localScale;

                pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
                pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);

                scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X) / 2.0f;
                scale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y) / 2.0f;

                rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
                rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
                rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

                this.objectList[i].localPosition = pos;
                this.objectList[i].localEulerAngles = rotAngles;
                this.objectList[i].localScale = scale;
            }
        }
    }

    private void RandomizeLightDirection()
    {
        const float MIN_ANGLE = 15.0f;
        const float MAX_ANGLE = 170.0f;

        Transform lightTransform = this.directionalLight.transform;
        Vector3 rotAngles = lightTransform.localEulerAngles;

        rotAngles.x = Random.Range(MIN_ANGLE, MAX_ANGLE);
        rotAngles.y = Random.Range(MIN_ANGLE, MAX_ANGLE);

        //lightTransform.localEulerAngles = rotAngles;

        int randLight = Random.Range(0, this.rwLightColors.Length);
        //this.directionalLight.color = this.rwLightColors[randLight];
        this.directionalLight.shadowStrength = Random.Range(0.25f, 0.9f);
        this.directionalLight.shadowNormalBias = Random.Range(0.4f, 2.5f);

    }
}
