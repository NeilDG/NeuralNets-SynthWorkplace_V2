using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


public class AutomatedBakingScript : MonoBehaviour
{
    private static int index = 0;
    private static int requiredBakes;
    private static string[] assetPaths;
    private static LightingSettings lightingSettings;

    private enum BakingMode
    {
        WITH_SHADOWS = 0,
        NO_SHADOWS = 1
    }

    private static BakingMode bakingMode = BakingMode.NO_SHADOWS; //modify as necessary
    private static int pngSceneIndex = 0;

    static string[] PopulateSkyboxes()
    {
        string[] guids = AssetDatabase.FindAssets("equirect t:material");
        string[] assetPaths = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
        }
        
        return assetPaths;
    }

    static LightingSettings SetDebugSettings()
    {
        LightingSettings defaultSettings = new LightingSettings();
        defaultSettings.directSampleCount = 4;
        defaultSettings.indirectSampleCount = 4;
        defaultSettings.environmentSampleCount = 4;
        defaultSettings.lightmapMaxSize = 1024;
        defaultSettings.lightmapResolution = 2.0f;
        defaultSettings.prioritizeView = false;
        defaultSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;

        return defaultSettings;
    }

    static LightingSettings SetProductionSettings()
    {
        LightingSettings defaultSettings = new LightingSettings();
        /*defaultSettings.directSampleCount = 1024;
        defaultSettings.indirectSampleCount = 1024;
        defaultSettings.environmentSampleCount = 1024;
        defaultSettings.lightmapMaxSize = 1024;
        defaultSettings.lightmapResolution = 8.0f;
        defaultSettings.prioritizeView = false;*/

        defaultSettings.directSampleCount = 256;
        defaultSettings.indirectSampleCount = 256;
        defaultSettings.environmentSampleCount = 256;
        defaultSettings.lightmapMaxSize = 256;
        defaultSettings.lightmapResolution = 2.0f;
        defaultSettings.prioritizeView = false;
        defaultSettings.lightmapper = LightingSettings.Lightmapper.ProgressiveGPU;
        if (bakingMode == BakingMode.WITH_SHADOWS)
        {
            defaultSettings.mixedBakeMode = MixedLightingMode.Shadowmask;
        }
        else
        {
            defaultSettings.mixedBakeMode = MixedLightingMode.Subtractive;
        }
        

        return defaultSettings;
    }

    [MenuItem("Window/Automated Baking/Save Skyboxes")]
    static void SaveSkyboxes()
    {
        assetPaths = PopulateSkyboxes();
    }

    static String FormatScenePath(BakingMode bakingMode, string sceneName)
    {
        if (bakingMode == BakingMode.WITH_SHADOWS)
        {
            return Application.dataPath + "/Scenes/Skyboxes/" + sceneName;
        }
        else
        {
            return Application.dataPath + "/Scenes/Skyboxes_NoShadows/" + sceneName;
        }
    }

    static String FormatRelativePath(BakingMode bakingMode, string sceneName)
    {
        if (bakingMode == BakingMode.WITH_SHADOWS)
        {
            return "Assets/Scenes/Skyboxes/" + sceneName;
        }
        else
        {
            return "Assets/Scenes/Skyboxes_NoShadows/" + sceneName;
        }
    }
    [MenuItem("Window/Automated Baking/Create Scene Copies")]
    static void SaveSceneCopies()
    {
        assetPaths = PopulateSkyboxes();
        requiredBakes = assetPaths.Length;
        //requiredBakes = 20;

        for (int i = index; i < requiredBakes; i++)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[i]);
            string sceneName = "Scene_" + i.ToString() + "_" + skyboxMat.name + ".unity";
            string scenePath = FormatScenePath(bakingMode, sceneName);
            RenderSettings.skybox = skyboxMat;
            bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
        }

        Debug.Log("Saved " +requiredBakes+ " scene copies.");
        
    }

    [MenuItem("Window/Automated Baking/Start Automated Baking")]
    static void StartAutomatedBaking()
    {
        assetPaths = PopulateSkyboxes();
        requiredBakes = assetPaths.Length;
        //requiredBakes = 20;

        Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[index]);
        RenderSettings.skybox = skyboxMat;

        Lightmapping.bakeStarted += OnBakeStarted;
        Lightmapping.bakeCompleted += OnBakeCompleted;

        string sceneNameMinusExt = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name;
        string sceneName = sceneNameMinusExt + ".unity";
        string scenePath = FormatScenePath(bakingMode, sceneName);
        bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);

        Lightmapping.Clear();
        Lightmapping.ClearDiskCache();
        Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);
        EditorSceneManager.OpenScene(scenePath);
        Lightmapping.lightingDataAsset = AssetDatabase.LoadAssetAtPath<LightingDataAsset>(scenePath + "/LightingData.asset");
        
        Lightmapping.BakeAsync();
        //OnBakeCompleted()
    }


    static void OnBakeStarted()
    {
        string sceneName = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name + ".unity";
        string scenePath = FormatScenePath(bakingMode, sceneName);

        //bool success = Lightmapping.TryGetLightingSettings(out lightingSettings);
        //lightingSettings = SetDebugSettings();
        lightingSettings = SetProductionSettings();
        RenderSettings.ambientIntensity = 2.0f; //boost light intensity
        Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);

        Debug.Log("Bake started for scene:" + sceneName);
        LightingSettings currentSettings = Lightmapping.GetLightingSettingsForScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Direct samples: " + currentSettings.directSampleCount);
        Debug.Log("Indirect samples: " + currentSettings.indirectSampleCount);
        Debug.Log("Environment samples: " + currentSettings.environmentSampleCount);
        Debug.Log("Baking device: " + currentSettings.lightmapper.ToString());
        Debug.Log("Skybox name: " + RenderSettings.skybox.name);
        
    }

    static void OnBakeCompleted()
    {
        string sceneNameMinusExt = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name;
        string sceneName = sceneNameMinusExt + ".unity";
        Debug.Log("Bake completed. Saved scene: " +sceneName);

        //start next bake
        index++;

        if (index < requiredBakes)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[index]);
            RenderSettings.skybox = skyboxMat;

            /*Debug.Log("Baking next skybox.");
            Debug.Log("Direct samples: " + lightingSettings.directSampleCount);
            Debug.Log("Indirect samples: " + lightingSettings.indirectSampleCount);
            Debug.Log("Environment samples: " + lightingSettings.environmentSampleCount);
            Debug.Log("Baking device: " + lightingSettings.lightmapper.ToString());
            Debug.Log("Skybox name: " + RenderSettings.skybox.name);*/

            string scenePath = FormatScenePath(bakingMode, sceneName);
            bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
            if (result)
            {
                Lightmapping.Clear();
                Lightmapping.ClearDiskCache();
                Lightmapping.SetLightingSettingsForScene(EditorSceneManager.GetActiveScene(), lightingSettings);
                EditorSceneManager.OpenScene(scenePath);

                sceneNameMinusExt = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name;
                sceneName = sceneNameMinusExt + ".unity";
                string lightingDataPath = FormatRelativePath(bakingMode, sceneNameMinusExt) + "/LightingData.asset";
                Debug.Log("Lighting data path: " + lightingDataPath);
                Lightmapping.lightingDataAsset = AssetDatabase.LoadAssetAtPath<LightingDataAsset>(lightingDataPath);
                Lightmapping.BakeAsync();
                //OnBakeCompleted();
            }
            else
            {
                Debug.LogError("An error occurred when creating a scene copy.");
            }

        }

        if (index == requiredBakes)
        {
            Debug.Log("All bake completed");
            /*index = 0;
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);*/
        }

    }

    [MenuItem("Window/Automated Baking/Maintenance/Map Lighting Data Assets")]
    static void AssignLightingDataAsset()
    {
        assetPaths = PopulateSkyboxes();
        requiredBakes = assetPaths.Length;
        //requiredBakes = 20;

        index = 0;
        AssignLightingData();
    }

    static void AssignLightingData()
    {
        string sceneNameMinusExt = "Scene_" + index.ToString() + "_" + RenderSettings.skybox.name;
        string sceneName = sceneNameMinusExt + ".unity";

        if (index < requiredBakes)
        {
            Material skyboxMat = AssetDatabase.LoadAssetAtPath<Material>(assetPaths[index]);
            RenderSettings.skybox = skyboxMat;

            string scenePath = FormatScenePath(bakingMode, sceneName);
            EditorSceneManager.OpenScene(scenePath);
            string lightingDataPath = FormatRelativePath(bakingMode, sceneNameMinusExt) + "/LightingData.asset";
            Debug.Log("Lighting data path: " + lightingDataPath);
            Lightmapping.lightingDataAsset = AssetDatabase.LoadAssetAtPath<LightingDataAsset>(lightingDataPath);

            bool result = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath, true);
            if (result)
            {
                Debug.Log("Updated lighting data asset of: " + sceneNameMinusExt);
            }
            else
            {
                Debug.LogError("An error occurred on mapping lighting data for: " + sceneNameMinusExt);
            }

            //start next scene
            index++;
            AssignLightingData();
        }

        if (index == requiredBakes)
        {
            index = 0;
            Debug.Log("Mapping completed");
        }
    }
}
