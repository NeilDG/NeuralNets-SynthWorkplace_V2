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
    private Dictionary<int, Texture2D> loadedImages;
    private const int LOADED_IMG_LIMIT = 1000;

    private DatasetLoader()
    {
        Random.InitState(1);
        this.placesDatasetFiles = Directory.GetFiles("E:/Places Dataset/", "*.jpg");
        this.loadedImages = new Dictionary<int, Texture2D>();

        for (int i = 0; i < LOADED_IMG_LIMIT; i++)
        {
            int key = Random.Range(0, this.placesDatasetFiles.Length);
            byte[] imgBytes = File.ReadAllBytes(this.placesDatasetFiles[key]);
            this.loadedImages[key] = new Texture2D(256, 256, TextureFormat.ARGB4444, false);
            this.loadedImages[key].LoadImage(imgBytes);
            this.loadedImages[key].Apply();
        }
        
    }

    public string[] GetPlacesDatasetFiles()
    {
        return this.placesDatasetFiles;
    }

    public Texture2D GetRandomImage()
    {
        int key = Random.Range(0, this.placesDatasetFiles.Length);

        if (this.loadedImages.ContainsKey(key))
        {
            return this.loadedImages[key];
        }
        else
        {
            //if capacity is full, unload 1 random image in dictionary
            this.UnloadRandomImage();

            byte[] imgBytes = File.ReadAllBytes(this.placesDatasetFiles[key]);
            this.loadedImages[key] = new Texture2D(256, 256, TextureFormat.ARGB4444, false);
            this.loadedImages[key].LoadImage(imgBytes);
            this.loadedImages[key].Apply();

            return this.loadedImages[key];
        }
    }

    private void UnloadRandomImage()
    {
        if (this.loadedImages.Count >= LOADED_IMG_LIMIT)
        {
            int randomKey = Random.Range(0, this.placesDatasetFiles.Length);
            if (this.loadedImages.ContainsKey(randomKey))
            {
                GameObject.Destroy(this.loadedImages[randomKey]);
                this.loadedImages.Remove(randomKey);

                Debug.Log("Successfully unloaded: " + randomKey);
                Resources.UnloadUnusedAssets();
            }
        }
    }

    public void ClearImageDictionary()
    {
        for (int key = 0; key < this.placesDatasetFiles.Length; key++)
        {
            if (this.loadedImages.ContainsKey(key))
            {
                GameObject.Destroy(this.loadedImages[key]);
                this.loadedImages[key].hideFlags = HideFlags.HideAndDontSave;
            }
        }

        this.loadedImages.Clear();
    }
}
