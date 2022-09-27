using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageBoxRenderer : MonoBehaviour
{
    [SerializeField] private MeshRenderer baseRenderer;

    private const float MIN_SCALE_Y = 10.0f;
    private const float MAX_SCALE_Y = 50.0f;

    private const float MIN_INTERVAL = 2.0f;
    private const float MAX_INTERVAL = 3.0f;
    private float ticks = 0.0f;

    // Start is called before the first frame update
    [ExecuteAlways]
    void Start()
    {
        this.RandomizeScale();
        this.RandomizeMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        this.ticks += Time.deltaTime;
        if (this.ticks > Random.Range(MIN_INTERVAL, MAX_INTERVAL))
        {
            this.ticks = 0.0f;
            this.RandomizeScale();
            this.RandomizeMaterial();
        }
    }

    private void RandomizeScale()
    {
        Transform objectTransform = this.baseRenderer.transform;
        Vector3 localScale = objectTransform.localScale;
        localScale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);

        Vector3 localPos = objectTransform.localPosition;
        localPos.y = ((localScale.y - MIN_SCALE_Y) / 2.0f) + 5.0f;

        objectTransform.localScale = localScale;
        objectTransform.localPosition = localPos;
    }

    private void RandomizeMaterial()
    {
        Color baseColor = this.baseRenderer.material.color;
        baseColor.r = Random.Range(1.0f, 1.0f);
        baseColor.g = Random.Range(1.0f, 1.0f);
        baseColor.b = Random.Range(1.0f, 1.0f);
        this.baseRenderer.material.color = baseColor;

        this.baseRenderer.material.SetFloat("_Metallic", Random.Range(0.0f, 0.0f));
        this.baseRenderer.material.SetFloat("_Glossiness", Random.Range(0.0f, 0.0f));

        this.baseRenderer.material.mainTexture = DatasetLoader.GetInstance().GetRandomImage();

    }
}
