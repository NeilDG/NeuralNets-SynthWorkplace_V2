using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ShadowRandomizer : MonoBehaviour
{
    [SerializeField] private Transform[] objectList;

    private const float MIN_POS_X = -60.0f;
    private const float MAX_POS_X = 60.0f;

    private const float MIN_POS_Z = -60.0f;
    private const float MAX_POS_Z = 60.0f;

    private const float MIN_SCALE_X = 5.0f;
    private const float MIN_SCALE_Z = 5.0f;

    private const float MAX_SCALE_X = 70.0f;
    private const float MAX_SCALE_Z = 70.0f;

    private const float MIN_ROT_HORIZONTAL = 0.0f;
    private const float MAX_ROT_HORIZONTAL = 85.0f;
    private const float MIN_ROT_VERTICAL = 0.0f;
    private const float MAX_ROT_VERTICAL = 10.0f;

    private const float INTERVAL = 1.0f;
    private float ticks = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.ticks += Time.deltaTime;
        if (this.ticks > INTERVAL)
        {
            this.ticks = 0.0f;
            this.StartRandomization();
        }
    }

    private void StartRandomization()
    {
        for (int i = 0; i < objectList.Length; i++)
        {
            Vector3 pos = this.objectList[i].localPosition;
            Vector3 rotAngles = this.objectList[i].localEulerAngles;
            Vector3 scale = this.objectList[i].localScale;

            pos.x = Random.Range(MIN_POS_X, MAX_POS_X);
            pos.z = Random.Range(MIN_POS_Z, MAX_POS_Z);

            scale.x = Random.Range(MIN_SCALE_X, MAX_SCALE_X);
            scale.z = Random.Range(MIN_SCALE_Z, MAX_SCALE_Z);

            rotAngles.x = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);
            rotAngles.y = Random.Range(MIN_ROT_HORIZONTAL, MAX_ROT_HORIZONTAL);
            rotAngles.z = Random.Range(MIN_ROT_VERTICAL, MAX_ROT_VERTICAL);

            this.objectList[i].localPosition = pos;
            this.objectList[i].localEulerAngles = rotAngles;
            this.objectList[i].localScale = scale;
        }
    }
}
