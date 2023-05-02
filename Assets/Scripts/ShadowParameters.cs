using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowParameters
{
    // public const int REFRESH_SCENE_PER_FRAME = 3; //for SynthInoueScene
    public const int REFRESH_SCENE_PER_FRAME = 3000;

    public const int MAX_IMAGES_TO_SAVE = 10000; //for DEBUG
    // public const int MAX_IMAGES_TO_SAVE = 150000;

    public const string BASE_PATH = "X:/SynthWeather Dataset 10/v66_places/";
    // public const string BASE_PATH = "X:/SynthWeather Dataset 10_2/";

    public static readonly int[] PRIMITIVE_SETS = {0, 150, 0, 0}; //cubes, prisms, spheres, capsules,

    public const float SHADOW_MIN_STRENGTH = 0.4f;
    public const float SHADOW_MAX_STRENGTH = 0.95f;
    // public const float SHADOW_MIN_STRENGTH = 0.1f;
    // public const float SHADOW_MAX_STRENGTH = 0.95f;

    public const float MIN_AMBIENT_INTENSITY = 0.25f;
    public const float MAX_AMBIENT_INTENSITY = 0.25f;
    // public const float MIN_AMBIENT_INTENSITY = 0.1f;
    // public const float MAX_AMBIENT_INTENSITY = 0.1f;
}

