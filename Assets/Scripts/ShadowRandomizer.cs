using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShadowRandomizer : MonoBehaviour
{
    // [SerializeField] private Transform[] cubeList;
    // [SerializeField] private Transform[] prismList;
    // [SerializeField] private Transform[] sphereList;
    // [SerializeField] private Transform[] capsuleList;

    [SerializeField] private GameObject cubeRef;
    [SerializeField] private GameObject prismRef;
    [SerializeField] private GameObject sphereRef;
    [SerializeField] private GameObject capsuleRef;
    [SerializeField] private List<GameObject> primitives = new List<GameObject>();

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
        CUBE = 0, PRISM = 1, SPHERE = 2, CAPSULE = 3
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
        this.InitializeLightColors();
        this.InitializePrimitives();
    }

    // Update is called once per frame
    void Update()
    {
        this.RandomizeAllPrimitives();
        this.RandomizeLightDirection();
    }

    void OnDestroy()
    {
        this.primitives.Clear();
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
    private void InitializePrimitives()
    {
        int[] NUM_PRIMITIVES = ShadowParameters.PRIMITIVE_SETS;
        
        this.cubeRef.gameObject.SetActive(false);
        this.prismRef.gameObject.SetActive(false);
        this.sphereRef.gameObject.SetActive(false);
        this.capsuleRef.gameObject.SetActive(false);

        this.InitializePrimitiveGroup(PrimitiveGroup.CUBE, NUM_PRIMITIVES[(int) PrimitiveGroup.CUBE]);
        this.InitializePrimitiveGroup(PrimitiveGroup.PRISM, NUM_PRIMITIVES[(int)PrimitiveGroup.PRISM]);
        this.InitializePrimitiveGroup(PrimitiveGroup.SPHERE, NUM_PRIMITIVES[(int)PrimitiveGroup.SPHERE]);
        // this.InitializePrimitiveGroup(PrimitiveGroup.CAPSULE, NUM_PRIMITIVES[(int)PrimitiveGroup.CAPSULE]);
    }

    private void InitializePrimitiveGroup(PrimitiveGroup primitiveGroup, int numPrimitives)
    {

        GameObject primitive = null;
        if (primitiveGroup == PrimitiveGroup.CUBE)
        {
            primitive = this.cubeRef;
        }
        else if (primitiveGroup == PrimitiveGroup.PRISM)
        {
            primitive = this.prismRef;
        }
        else if (primitiveGroup == PrimitiveGroup.SPHERE)
        {
            primitive = this.sphereRef;
        }
        else if (primitiveGroup == PrimitiveGroup.CAPSULE)
        {
            primitive = this.capsuleRef;
        }
        else
        {
            Debug.LogError("Unidentified primitive! Skipping");
            return;
        }

       
        for (int i = 0; i < numPrimitives; i++)
        {
            GameObject primitiveInstance = GameObject.Instantiate(primitive);
            primitiveInstance.transform.SetParent(this.transform);
            primitiveInstance.gameObject.SetActive(true);

            Vector3 pos = primitive.transform.localPosition;
            Vector3 rotAngles = primitive.transform.localEulerAngles;
            Vector3 scale = primitive.transform.localScale;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);
            
            scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);
            
            rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
            rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
            rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

            primitiveInstance.transform.localPosition = pos;
            primitiveInstance.transform.localEulerAngles = rotAngles;
            primitiveInstance.transform.localScale = scale;

            this.primitives.Add(primitiveInstance);
        }
    }

    private void RandomizeAllPrimitives()
    {
        for (int i = 0; i < this.primitives.Count; i++)
        {
            GameObject primitiveInstance = this.primitives[i];
            Vector3 pos = primitiveInstance.transform.localPosition;
            Vector3 rotAngles = primitiveInstance.transform.localEulerAngles;
            Vector3 scale = primitiveInstance.transform.localScale;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);

            scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);

            rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
            rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
            rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

            primitiveInstance.transform.localPosition = pos;
            primitiveInstance.transform.localEulerAngles = rotAngles;
            primitiveInstance.transform.localScale = scale;
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

        // lightTransform.localEulerAngles = rotAngles;

        int randLight = Random.Range(0, this.rwLightColors.Length);
        this.directionalLight.color = this.rwLightColors[randLight];
        RenderSettings.ambientLight = Color.white * MathF.Round(Random.Range(ShadowParameters.MIN_AMBIENT_INTENSITY, ShadowParameters.MAX_AMBIENT_INTENSITY), 4);
        this.directionalLight.shadowStrength = MathF.Round(Random.Range(ShadowParameters.SHADOW_MIN_STRENGTH, ShadowParameters.SHADOW_MAX_STRENGTH), 4);
        this.directionalLight.shadowNormalBias = Random.Range(0.4f, 2.5f);

        Debug.Log("<b> Shadow parameters: " + this.directionalLight.shadowStrength + " Ambient Intensity: " + RenderSettings.ambientLight + "</b>");
    }
}
