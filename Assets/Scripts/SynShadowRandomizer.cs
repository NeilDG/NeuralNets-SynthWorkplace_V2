using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynShadowRandomizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Light directionalLight;

    [SerializeField] private float MIN_POS_X = -7.0f;
    [SerializeField] private float MAX_POS_X = 7.0f;

    [SerializeField] private float MIN_POS_Y = -5.0f;
    [SerializeField] private float MAX_POS_Y = 5.0f;

    private const float INTERVAL = 1.0f;
    private float ticks = 0.0f;
    private int frames = 0;

    private Color[] rwLightColors;

    // Start is called before the first frame update
    void Start()
    {
        this.InitializeLightColors();
    }

    // Update is called once per frame
    void Update()
    {
        this.RandomizeLightDirection();
        this.RandomizeShadowMatteTransform();
        if (this.frames % CameraRecordingV2.REFRESH_SCENE_PER_FRAME == 0)
        {
            this.frames = 0;
            this.RandomizeShadowMatte();
        }
    }

    private void RandomizeShadowMatte()
    {
        this.spriteRenderer.sprite = DatasetLoader.GetInstance().GetRandomSprite();
    }

    private void RandomizeShadowMatteTransform()
    {
        const float MIN_ANGLE = 0.0f;
        const float MAX_ANGLE = 180.0f;

        this.spriteRenderer.gameObject.SetActive(true);
        Vector3 pos = this.spriteRenderer.transform.localPosition;
        Vector3 rotAngles = this.spriteRenderer.transform.localEulerAngles;
        Vector3 scale = this.spriteRenderer.transform.localScale;

        pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
        pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);

        rotAngles.z = Random.Range(MIN_ANGLE, MAX_ANGLE);

        this.spriteRenderer.transform.localPosition = pos;
        this.spriteRenderer.transform.localEulerAngles = rotAngles;
        this.spriteRenderer.transform.localScale = scale;

        const float SHADOW_MIN_STRENGTH = 0.4f;
        const float SHADOW_MAX_STRENGTH = 0.95f;
        this.spriteRenderer.color = new Color(0, 0, 0, Random.Range(SHADOW_MIN_STRENGTH, SHADOW_MAX_STRENGTH));
    }

    private void InitializeLightColors()
    {
        this.rwLightColors = new Color[6];
        //from http://planetpixelemporium.com/tutorialpages/light.html
        this.rwLightColors[0].r = 255.0f / 255.0f;
        this.rwLightColors[0].g = 197.0f / 255.0f;
        this.rwLightColors[0].b = 143.0f / 255.0f;

        this.rwLightColors[1].r = 255.0f / 255.0f;
        this.rwLightColors[1].g = 214.0f / 255.0f;
        this.rwLightColors[1].b = 170.0f / 255.0f;

        this.rwLightColors[2].r = 255.0f / 255.0f;
        this.rwLightColors[2].g = 241.0f / 255.0f;
        this.rwLightColors[2].b = 224.0f / 255.0f;

        this.rwLightColors[3].r = 255.0f / 255.0f;
        this.rwLightColors[3].g = 250.0f / 255.0f;
        this.rwLightColors[3].b = 244.0f / 255.0f;

        this.rwLightColors[4].r = 255.0f / 255.0f;
        this.rwLightColors[4].g = 255.0f / 255.0f;
        this.rwLightColors[4].b = 251.0f / 255.0f;

        this.rwLightColors[5].r = 255.0f / 255.0f;
        this.rwLightColors[5].g = 255.0f / 255.0f;
        this.rwLightColors[5].b = 255.0f / 255.0f;

        /*this.rwLightColors[6].r = 201.0f/255.0f;
        this.rwLightColors[6].g = 226.0f/255.0f;
        this.rwLightColors[6].b = 255.0f/255.0f;

        this.rwLightColors[7].r = 64.0f/255.0f;
        this.rwLightColors[7].g = 156.0f/255.0f;
        this.rwLightColors[7].b = 255.0f/255.0f;*/

    }

    private void RandomizeLightDirection()
    {
        const float MIN_ANGLE = 15.0f;
        const float MAX_ANGLE = 170.0f;
        const float AMBIENT_INTENSITY = 0.1f;

        Transform lightTransform = this.directionalLight.transform;
        Vector3 rotAngles = lightTransform.localEulerAngles;

        rotAngles.x = Random.Range(MIN_ANGLE, MAX_ANGLE);
        rotAngles.y = Random.Range(MIN_ANGLE, MAX_ANGLE);

        //lightTransform.localEulerAngles = rotAngles;

        int randLight = Random.Range(0, this.rwLightColors.Length);
        this.directionalLight.color = this.rwLightColors[randLight];
        RenderSettings.ambientLight = this.directionalLight.color * AMBIENT_INTENSITY;
        this.directionalLight.shadowNormalBias = Random.Range(0.4f, 2.5f);
    }
}
