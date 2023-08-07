using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private string[] synShadowFiles;
    private string[] synShadowSplicedFiles;

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

    public class Sprite2DTracker
    {
        public int key = 0;
        public Sprite loadedSprite = null;
        public bool beingUsed = false;

        public Sprite2DTracker(int key)
        {
            this.key = 0;
        }
    }

    private Dictionary<int, Texture2DTracker> loadedImages;
    private Dictionary<int, Sprite2DTracker> loadedSprites;

    private const int LOADED_IMG_LIMIT = 3000;
    private int currentKey = 0;

    private DatasetLoader()
    {
        // this.placesDatasetFiles = Directory.GetFiles("X:/Places Dataset/", "*.jpg");

        List<string> istdList = new List<string>();
        int repeats = 500;
        for (int i = 0; i < repeats; i++)
        {
            string[] istdBaseList = Directory.GetFiles("X:/ISTD_Dataset/train/train_C/", "*.png");
            istdList.AddRange(istdBaseList);
        }
        this.placesDatasetFiles = istdList.ToArray();

        // List<string> srdList = new List<string>();
        // int repeats = 500;
        // for (int i = 0; i < repeats; i++)
        // {
        //     string[] baseList = Directory.GetFiles("X:/SRD_Train/shadow_free/", "*.jpg");
        //     srdList.AddRange(baseList);
        // }
        // this.placesDatasetFiles = srdList.ToArray();

        // List<string> usrList = new List<string>();
        // int repeats = 500;
        // for (int i = 0; i < repeats; i++)
        // {
        //     string[] baseList = Directory.GetFiles("E:/USR Shadow Dataset/shadow_free/", "*.jpg");
        //     usrList.AddRange(baseList);
        // }
        // this.placesDatasetFiles = usrList.ToArray();

        this.currentKey = CameraRecordingV2.counter % this.placesDatasetFiles.Length;
        Debug.Log("Set image ID to:" + this.currentKey);
        this.loadedImages = new Dictionary<int, Texture2DTracker>();
        this.loadedSprites = new Dictionary<int, Sprite2DTracker>();

        for (int i = 0; i < LOADED_IMG_LIMIT; i++)
        {
            this.GetRandomImage();
        }

        this.synShadowFiles = Directory.GetFiles("E:/SynShadow/matte/", "*.png");
        this.synShadowSplicedFiles = Directory.GetFiles("E:/SynShadow/matte_spliced/", "*.png");

    }
    public Texture2DTracker GetRandomImage()
    {
        // int key = this.currentKey; //iterate through each places image
        // this.currentKey++;
        // if (this.currentKey >= this.placesDatasetFiles.Length)
        // {
        //     this.currentKey = 0;
        // }

        int key = Random.Range(0, this.placesDatasetFiles.Length); //randomly select image from places

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

    public Sprite GetRandomSprite()
    {
        int key = Random.Range(0, this.synShadowFiles.Length); //randomly select image from synshadows
        //int key = Random.Range(0, 10); //randomly select image from synshadows

        //if capacity is full, unload 1 random image in dictionary
        Resources.UnloadUnusedAssets();

        string fileName = "SynShadow/" + this.synShadowFiles[key].Split("/").Last().Split(".")[0];
        Debug.Log("Resource file to load: " +fileName);
        Sprite loadedSprite = Resources.Load<Sprite>(fileName);
        return loadedSprite;
        
    }

    public Sprite GetRandomSplicedSprite()
    {
        int key = Random.Range(0, this.synShadowSplicedFiles.Length); //randomly select image from synshadows
        //int key = Random.Range(0, 10); //randomly select image from synshadows
        //if capacity is full, unload 1 random image in dictionary
        Resources.UnloadUnusedAssets();

        string fileName = "SynShadow_Spliced/" + this.synShadowSplicedFiles[key].Split("/").Last().Split(".")[0];
        Debug.Log("Resource file to load: " + fileName);
        Sprite loadedSprite = Resources.Load<Sprite>(fileName);

        // Debug.Log("To load: " + this.synShadowSplicedFiles[key]);
        // Sprite loadedSprite = Img2SpriteUtil.LoadNewSprite(this.synShadowSplicedFiles[key], 25.0f, SpriteMeshType.Tight);
        return loadedSprite;
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
