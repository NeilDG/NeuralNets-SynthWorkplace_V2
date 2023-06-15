using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ShadowSpriteRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject squareRef;
    [SerializeField] private GameObject triangleRef;
    [SerializeField] private GameObject circleRef;
    [SerializeField] private List<GameObject> primitives = new List<GameObject>();
    [SerializeField] private PostProcessProfile cameraProfile;

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
        this.rwLightColors = new Color[7];
        //from http://planetpixelemporium.com/tutorialpages/light.html
        this.rwLightColors[0].r = 255.0f / 255.0f;
        this.rwLightColors[0].g = 147.0f / 255.0f;
        this.rwLightColors[0].b = 41.0f / 255.0f;

        this.rwLightColors[1].r = 255.0f / 255.0f;
        this.rwLightColors[1].g = 197.0f / 255.0f;
        this.rwLightColors[1].b = 143.0f / 255.0f;

        this.rwLightColors[2].r = 255.0f / 255.0f;
        this.rwLightColors[2].g = 214.0f / 255.0f;
        this.rwLightColors[2].b = 170.0f / 255.0f;

        this.rwLightColors[3].r = 255.0f / 255.0f;
        this.rwLightColors[3].g = 241.0f / 255.0f;
        this.rwLightColors[3].b = 224.0f / 255.0f;

        this.rwLightColors[4].r = 255.0f / 255.0f;
        this.rwLightColors[4].g = 250.0f / 255.0f;
        this.rwLightColors[4].b = 244.0f / 255.0f;

        this.rwLightColors[5].r = 255.0f / 255.0f;
        this.rwLightColors[5].g = 255.0f / 255.0f;
        this.rwLightColors[5].b = 251.0f / 255.0f;

        this.rwLightColors[6].r = 255.0f / 255.0f;
        this.rwLightColors[6].g = 255.0f / 255.0f;
        this.rwLightColors[6].b = 255.0f / 255.0f;

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

        this.squareRef.gameObject.SetActive(false);
        this.triangleRef.gameObject.SetActive(false);
        this.circleRef.gameObject.SetActive(false);

        this.InitializePrimitiveGroup(PrimitiveGroup.CUBE, NUM_PRIMITIVES[(int)PrimitiveGroup.CUBE]);
        this.InitializePrimitiveGroup(PrimitiveGroup.PRISM, NUM_PRIMITIVES[(int)PrimitiveGroup.PRISM]);
        this.InitializePrimitiveGroup(PrimitiveGroup.SPHERE, NUM_PRIMITIVES[(int)PrimitiveGroup.SPHERE]);
    }

    private void InitializePrimitiveGroup(PrimitiveGroup primitiveGroup, int numPrimitives)
    {

        GameObject primitive = null;
        if (primitiveGroup == PrimitiveGroup.CUBE)
        {
            primitive = this.squareRef;
        }
        else if (primitiveGroup == PrimitiveGroup.PRISM)
        {
            primitive = this.triangleRef;
        }
        else if (primitiveGroup == PrimitiveGroup.SPHERE)
        {
            primitive = this.circleRef;
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
        int randLight = Random.Range(0, this.rwLightColors.Length);
        float shadowStrength = MathF.Round(Random.Range(ShadowParameters.SHADOW_MIN_STRENGTH, ShadowParameters.SHADOW_MAX_STRENGTH), 4);
        for (int i = 0; i < this.primitives.Count; i++)
        {
            SpriteRenderer sprite = this.primitives[i].GetComponent<SpriteRenderer>();
            //set shadow strength through occluder
            Color occluderColor = sprite.color;
            occluderColor.a = shadowStrength;
            sprite.color = occluderColor;

        }

        //set lighting via color profile
        Color gradingColor = new Color(this.rwLightColors[randLight].r, this.rwLightColors[randLight].g,
            this.rwLightColors[randLight].b);
        ColorGrading grading = this.cameraProfile.GetSetting<ColorGrading>();
        grading.colorFilter.value = gradingColor;

        Debug.Log("<b> Shadow parameters: " + shadowStrength + "</b>" + " Color grading color: " +grading.colorFilter.value);
    }
}
