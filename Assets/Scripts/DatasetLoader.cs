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

    public class Texture2DTracker
    {
        public int key = 0;
        public Texture2D loadedImg = null;
        public bool beingUsed = false;

        public Texture2DTracker(int key)
        {
            this.key = 0;
        }
    }

    private Dictionary<int, Texture2DTracker> loadedImages;

    private const int LOADED_IMG_LIMIT = 3000;
    private int currentKey = 0;

    private DatasetLoader()
    {
        Random.InitState(1);
        this.placesDatasetFiles = Directory.GetFiles("E:/Places Dataset/", "*.jpg");
        this.loadedImages = new Dictionary<int, Texture2DTracker>();

        for (int i = 0; i < LOADED_IMG_LIMIT; i++)
        {
            this.GetRandomImage();
        }
        
    }
    public Texture2DTracker GetRandomImage()
    {
        int key = this.currentKey; //iterate through each places image
        this.currentKey++;
        if (this.currentKey >= this.placesDatasetFiles.Length)
        {
            this.currentKey = 0;
        }

        if (this.loadedImages.ContainsKey(key))
        {
            return this.loadedImages[key];
        }
        else
        {
            //if capacity is full, unload 1 random image in dictionary
            this.UnloadRandomImage();

            byte[] imgBytes = File.ReadAllBytes(this.placesDatasetFiles[key]);
            this.loadedImages[key] = new Texture2DTracker(key);
            this.loadedImages[key].loadedImg = new Texture2D(256, 256, TextureFormat.ARGB4444, false);
            this.loadedImages[key].loadedImg.LoadImage(imgBytes);
            this.loadedImages[key].loadedImg.Apply();

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
                if (this.loadedImages[randomKey].beingUsed == false)
                {
                    GameObject.Destroy(this.loadedImages[randomKey].loadedImg);
                    this.loadedImages.Remove(randomKey);

                    Debug.Log("Successfully unloaded: " + randomKey);
                    Resources.UnloadUnusedAssets();
                }
                
            }
            else if (this.loadedImages.ContainsKey(randomKey) && this.loadedImages[randomKey].beingUsed)
            {
                Debug.Log("Image: " + randomKey + " is still being used. Cannot unload.");
            }
        }
    }

    public void TagImageForClearing(Texture2DTracker textureTracker)
    {
        if (this.loadedImages.ContainsKey(textureTracker.key))
        {
            this.loadedImages[textureTracker.key].beingUsed = false;
        }
    }

    public void ClearImageDictionary()
    {
        for (int key = 0; key < this.placesDatasetFiles.Length; key++)
        {
            if (this.loadedImages.ContainsKey(key) && this.loadedImages[key].beingUsed == false)
            {

                GameObject.Destroy(this.loadedImages[key].loadedImg);
                this.loadedImages.Remove(key);

                Debug.Log("Successfully unloaded: " + key);
            }

            else if (this.loadedImages.ContainsKey(key) && this.loadedImages[key].beingUsed)
            {
                Debug.Log("Image: " + key + " is still being used. Cannot unload.");
            }
        }

        Resources.UnloadUnusedAssets();
    }
}
