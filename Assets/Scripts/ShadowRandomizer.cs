using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ShadowRandomizer : MonoBehaviour
{
    [SerializeField] private Transform[] objectList;
    [SerializeField] private Light directionalLight;

    private const float MIN_POS_X = -7.0f;
    private const float MAX_POS_X = 7.0f;

    private const float MIN_POS_Y = -5.0f;
    private const float MAX_POS_Y = 5.0f;

    private const float MIN_SCALE_X = 5.0f;
    private const float MIN_SCALE_Y = 5.0f;

    private const float MAX_SCALE_X = 70.0f;
    private const float MAX_SCALE_Y = 70.0f;

    private const float MIN_ROT_HORIZONTAL = -85.0f;
    private const float MAX_ROT_HORIZONTAL = 85.0f;
    private const float MIN_ROT_VERTICAL = -30.0f;
    private const float MAX_ROT_VERTICAL = 30.0f;

    private const float INTERVAL = 1.0f;
    private float ticks = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState(1);
    }

    // Update is called once per frame
    void Update()
    {
        this.StartRandomization();
        this.RandomizeLightDirection();
    }

    private void StartRandomization()
    {
        for (int i = 0; i < objectList.Length; i++)
        {
            Vector3 pos = this.objectList[i].localPosition;
            Vector3 rotAngles = this.objectList[i].localEulerAngles;
            Vector3 scale = this.objectList[i].localScale;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.y = Random.Range(MIN_POS_Y, MAX_POS_Y);

            scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scale.y = Random.Range(MIN_SCALE_Y, MAX_SCALE_Y);

            rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
            rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
            rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

            this.objectList[i].localPosition = pos;
            this.objectList[i].localEulerAngles = rotAngles;
            this.objectList[i].localScale = scale;
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

        lightTransform.localEulerAngles = rotAngles;

    }
}
