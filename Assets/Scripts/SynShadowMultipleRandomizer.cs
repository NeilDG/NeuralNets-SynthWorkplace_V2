using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SynShadowMultipleRandomizer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Light directionalLight;
    [SerializeField] private List<SpriteRenderer> spriteInstances = new List<SpriteRenderer>();

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
        this.InitializeSprites();
    }

    // Update is called once per frame
    void Update()
    {
        this.RandomizeLightDirection();
        this.RandomizeShadowMatteTransform();
        if (this.frames % ShadowParameters.REFRESH_3D_MODEL_PER_FRAME == 0)
        {
            this.frames = 0;
            this.RandomizeShadowMatte();
        }
        this.frames++;
    }

    private void InitializeSprites()
    {
        int numSprites = ShadowParameters.NUM_SPLICED_SPRITES;
        this.spriteRenderer.gameObject.SetActive(false);

        for (int i = 0; i < numSprites; i++)
        {
            GameObject spriteInstance = GameObject.Instantiate(this.spriteRenderer.gameObject);

            Vector3 pos = this.spriteRenderer.transform.localPosition;
            Vector3 rotAngles = this.spriteRenderer.transform.localEulerAngles;
            Vector3 scale = this.spriteRenderer.transform.localScale;

            spriteInstance.transform.localPosition = pos;
            spriteInstance.transform.localEulerAngles = rotAngles;
            spriteInstance.transform.localScale = scale;

            spriteInstance.transform.SetParent(this.spriteRenderer.transform.parent);
            spriteInstance.gameObject.SetActive(true);

            this.spriteInstances.Add(spriteInstance.GetComponent<SpriteRenderer>());
        }
    }

    private void RandomizeShadowMatte()
    {
        for (int i = 0; i < this.spriteInstances.Count; i++)
        {
            this.spriteInstances[i].sprite = DatasetLoader.GetInstance().GetRandomSprite();
        }
    }

    private void RandomizeShadowMatteTransform()
    {
        const float MIN_ANGLE = 0.0f;
        const float MAX_ANGLE = 180.0f;

        for (int i = 0; i < this.spriteInstances.Count; i++)
        {
            this.spriteInstances[i].gameObject.SetActive(true);
            Vector3 pos = this.spriteRenderer.transform.localPosition;
            Vector3 rotAngles = this.spriteRenderer.transform.localEulerAngles;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);
            rotAngles.z = Random.Range(MIN_ANGLE, MAX_ANGLE);

            this.spriteInstances[i].transform.localPosition = pos;
            this.spriteInstances[i].transform.localEulerAngles = rotAngles;
            this.spriteInstances[i].transform.localScale = Vector3.one * 0.1f;
            this.spriteInstances[i].color = new Color(0, 0, 0, Random.Range(ShadowParameters.SHADOW_MIN_STRENGTH, ShadowParameters.SHADOW_MAX_STRENGTH));
        }

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

    private void RandomizeLightDirection()
    {
        if (this.directionalLight == null)
        {
            return;
        }

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
