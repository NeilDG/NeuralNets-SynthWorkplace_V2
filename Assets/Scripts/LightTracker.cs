using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LightTracker : MonoBehaviour
{
    [SerializeField] private Light dirLight;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private int startIndex = 0;
    private int index = 0;
    private String filePath;
    private StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        this.filePath = "D:/Users/delgallegon/Documents/GithubProjects/NeuralNets-SynthWorkplace/Recordings/Lights_11/lights_{0}.txt";
    }

    // Update is called once per frame
    void Update()
    {
        if (this.index >= this.startIndex)
        {
            float x = Mathf.Abs(this.cameraTransform.localPosition.x - this.dirLight.transform.localPosition.x);
            //float y = Mathf.Abs(this.cameraTransform.localPosition.y - this.dirLight.transform.localPosition.y);
            float z = Mathf.Abs(this.cameraTransform.localPosition.z - this.dirLight.transform.localPosition.z);

            //Debug.LogFormat("Distance to global light: (X:{0}, Z:{1})", x, z);

            String textFilePath = String.Format(this.filePath, this.index - this.startIndex);

            //Save coords per file
            StreamWriter writer = new StreamWriter(textFilePath, true);
            writer.WriteLine("{0},{1}", x, z);
            writer.Close();
        }

        this.index++;

    }
}
