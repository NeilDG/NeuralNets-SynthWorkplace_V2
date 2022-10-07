using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageBoxRenderer : MonoBehaviour
{
    [SerializeField] private MeshRenderer baseRenderer;
    [SerializeField] private DatasetLoader.Texture2DTracker textureTracker;

    [SerializeField] private float MIN_SCALE_Y = 5.0f;
    [SerializeField] private float MAX_SCALE_Y = 25.0f;

    [SerializeField] private float MIN_SCALE_X = 5.0f;
    [SerializeField] private float MAX_SCALE_X = 80.0f;

    //private float ticks = 0.0f;
    private int frames = 0;
    private int matCount = 0;

    private Vector3 baseScale;

    // Start is called before the first frame update
    [ExecuteAlways]
    void Start()
    {
        this.baseScale = this.transform.localScale;

        this.RandomizeScale();
        this.RandomizeMaterial();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*float multiplier = Time.captureDeltaTime * Time.timeScale;
        this.frames += Mathf.RoundToInt(multiplier);
        if (this.frames % CameraRecordingV2.REFRESH_SCENE_PER_FRAME == 0)
        {
            this.frames = Mathf.RoundToInt(multiplier);
            this.RandomizeScale();
            //this.RandomizeMaterial();
        }
        else
        {
            //Debug.Log("Image box frame count: " +frames);
        }*/

        this.frames++;
        this.RandomizeScale();

        if (this.frames % CameraRecordingV2.REFRESH_SCENE_PER_FRAME == 0)
        {
            this.frames = 0;
            this.RandomizeMaterial();
        }
    }

    private void RandomizeScale()
    {
        Transform objectTransform = this.baseRenderer.transform;
        Vector3 localScale = new Vector3(baseScale.x, baseScale.y, baseScale.z);
        localScale.x += Random.Range(MIN_SCALE_X, MAX_SCALE_X);
        localScale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);
        localScale.z += Random.Range(MIN_SCALE_X, MAX_SCALE_X);

        Vector3 localPos = objectTransform.localPosition;
        localPos.y = ((localScale.y - MIN_SCALE_Y) / 2.0f) + 5.0f;

        objectTransform.localScale = localScale;
        objectTransform.localPosition = localPos;
    }

    private void RandomizeMaterial()
    {
        Material baseMaterial = new Material(this.baseRenderer.material);
        baseMaterial.name = "Material_Instance_" + this.matCount;
        this.matCount++;

        //destroy previous material
        GameObject.Destroy(this.baseRenderer.material.mainTexture);
        GameObject.Destroy(this.baseRenderer.material);
        if (this.textureTracker != null)
        {
            DatasetLoader.GetInstance().TagImageForClearing(this.textureTracker);
            this.textureTracker = null;
        }
        
        /*for (int i = 0; i < this.baseRenderer.materials.Length; i++)
        {
            GameObject.Destroy(this.baseRenderer.materials[i]);
        }*/

        Color baseColor = baseMaterial.color;
        baseColor.r = Random.Range(1.0f, 1.0f);
        baseColor.g = Random.Range(1.0f, 1.0f);
        baseColor.b = Random.Range(1.0f, 1.0f);
        baseMaterial.color = baseColor;

        baseMaterial.SetFloat("_Metallic", Random.Range(0.0f, 0.0f));
        baseMaterial.SetFloat("_Glossiness", Random.Range(0.0f, 0.0f));

        Texture mainTexture = new Texture2D(256, 256);
        this.textureTracker = DatasetLoader.GetInstance().GetRandomImage();
        mainTexture = this.textureTracker.loadedImg;
        this.textureTracker.beingUsed = true;

        baseMaterial.mainTexture = mainTexture;
        this.baseRenderer.material = baseMaterial;

    }
}
