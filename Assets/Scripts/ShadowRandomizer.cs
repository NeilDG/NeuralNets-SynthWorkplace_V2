using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShadowRandomizer : MonoBehaviour
{
    [SerializeField] private Transform[] cubeList;
    [SerializeField] private Transform[] prismList;
    [SerializeField] private Transform[] sphereList;

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

    private enum PrimitiveGroup
    {
        CUBE = 0, PRISM = 1, SPHERE = 2
    }

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
        int[] NUM_PRIMITIVES = new int[]{0, 0, 20}; //cubes, prisms, spheres

        for (int i = 0; i < cubeList.Length; i++)
        {
            this.cubeList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < cubeList.Length; i++)
        {
            this.prismList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < cubeList.Length; i++)
        {
            this.sphereList[i].gameObject.SetActive(false);
        }

        this.RandomizePrimitiveGroup(PrimitiveGroup.CUBE, NUM_PRIMITIVES[(int) PrimitiveGroup.CUBE]);
        this.RandomizePrimitiveGroup(PrimitiveGroup.PRISM, NUM_PRIMITIVES[(int)PrimitiveGroup.PRISM]);
        this.RandomizePrimitiveGroup(PrimitiveGroup.SPHERE, NUM_PRIMITIVES[(int)PrimitiveGroup.SPHERE]);
    }

    private void RandomizePrimitiveGroup(PrimitiveGroup primitiveGroup, int numPrimitives)
    {
        Transform[] primitiveList;
        if (primitiveGroup == PrimitiveGroup.SPHERE)
        {
            primitiveList = this.sphereList;
        }
        else if (primitiveGroup == PrimitiveGroup.PRISM)
        {
            primitiveList = this.prismList;
        }
        else
        {
            primitiveList = this.cubeList;
        }

        for (int i = 0; i < numPrimitives; i++)
        {
            primitiveList[i].gameObject.SetActive(true);
            Vector3 pos = primitiveList[i].localPosition;
            Vector3 rotAngles = primitiveList[i].localEulerAngles;
            Vector3 scale = primitiveList[i].localScale;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);

            scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);

            rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
            rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
            rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

            primitiveList[i].localPosition = pos;
            primitiveList[i].localEulerAngles = rotAngles;
            primitiveList[i].localScale = scale;

        }
    }

    private void RandomizeLightDirection()
    {
        const float MIN_ANGLE = 15.0f;
        const float MAX_ANGLE = 170.0f;

        // const float SHADOW_MIN_STRENGTH = 0.4f;
        // const float SHADOW_MAX_STRENGTH = 0.95f;
        // const float AMBIENT_INTENSITY = 0.25f;

        const float SHADOW_MIN_STRENGTH = 0.1f;
        const float SHADOW_MAX_STRENGTH = 0.95f;
        const float AMBIENT_INTENSITY = 0.1f;

        Debug.Log("<b> Shadow parameters. Min Str: " + SHADOW_MIN_STRENGTH + " Max Str: " + SHADOW_MAX_STRENGTH + " Ambient Intensity: " +AMBIENT_INTENSITY + "</b>");
        Transform lightTransform = this.directionalLight.transform;
        Vector3 rotAngles = lightTransform.localEulerAngles;

        rotAngles.x = Random.Range(MIN_ANGLE, MAX_ANGLE);
        rotAngles.y = Random.Range(MIN_ANGLE, MAX_ANGLE);

        //lightTransform.localEulerAngles = rotAngles;

        int randLight = Random.Range(0, this.rwLightColors.Length);
        this.directionalLight.color = this.rwLightColors[randLight];
        RenderSettings.ambientLight = this.directionalLight.color * AMBIENT_INTENSITY;
        this.directionalLight.shadowStrength = Random.Range(SHADOW_MIN_STRENGTH, SHADOW_MAX_STRENGTH);
        this.directionalLight.shadowNormalBias = Random.Range(0.4f, 2.5f);
    }
}
