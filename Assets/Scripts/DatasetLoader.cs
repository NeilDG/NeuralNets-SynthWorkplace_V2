using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class DatasetLoader
{
    private static DatasetLoader sharedInstance = null;

    public static DatasetLoader GetInstance()
    {
        if (sharedInstance == null)
        {
            sharedInstance = new DatasetLoader();
        }

        return sharedInstance;
    }

    private string[] placesDatasetFiles;
    private Texture2D[] loadedImages;

    private DatasetLoader()
    {
        this.placesDatasetFiles = Directory.GetFiles("E:/Places Dataset/", "*.jpg");
        this.loadedImages = new Texture2D[this.placesDatasetFiles.Length];

        for (int i = 0; i < 250; i++)
        {
            byte[] imgBytes = File.ReadAllBytes(placesDatasetFiles[i]);
            this.loadedImages[i] = new Texture2D(512, 512, TextureFormat.ARGB4444, false);
            this.loadedImages[i].LoadImage(imgBytes);
            this.loadedImages[i].Apply();
        }
        
    }

    public string[] GetPlacesDatasetFiles()
    {
        return this.placesDatasetFiles;
    }

    public Texture2D GetRandomImage()
    {
        return this.loadedImages[Random.Range(0, this.loadedImages.Length)];
    }
}
