using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowParameters
{
    // public const int REFRESH_SCENE_PER_FRAME = 1; //for SynthInoueScene
    public const int REFRESH_SCENE_PER_FRAME = 2000;
    public const int REFRESH_3D_MODEL_PER_FRAME = 5;

    public const int MAX_IMAGES_TO_SAVE = 50000; //for DEBUG
    // public const int MAX_IMAGES_TO_SAVE = 200000;

    public const string BASE_PATH = "X:/SynthWeather Dataset 10/v91_istd/";
    // public const string BASE_PATH = "X:/SynthWeather Dataset 10_2/";

    public static readonly int[] PRIMITIVE_SETS = {0, 10, 10, 0}; //cubes, prisms, spheres, capsules
    public static readonly int NUM_SPLICED_SPRITES = 10;

    // public const float SHADOW_MIN_STRENGTH = 0.4f;
    // public const float SHADOW_MAX_STRENGTH = 0.95f;
    // public const float SHADOW_MIN_STRENGTH = 0.1f;
    // public const float SHADOW_MAX_STRENGTH = 0.95f;
    public const float SHADOW_MIN_STRENGTH = 0.1f;
    public const float SHADOW_MAX_STRENGTH = 0.5f;

    //NOTE: Ambient intensity has no effect on sprites.
    public const float MIN_AMBIENT_INTENSITY = 0.45f;
    public const float MAX_AMBIENT_INTENSITY = 0.45f;
    // public const float MIN_AMBIENT_INTENSITY = 0.1f;
    // public const float MAX_AMBIENT_INTENSITY = 0.1f;
}

