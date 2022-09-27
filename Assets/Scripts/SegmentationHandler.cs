using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentationHandler : MonoBehaviour
{
    [SerializeField] private Shader segmentShader;

    private Dictionary<string, Color32> segmentDict = new Dictionary<string, Color32>();
    private Dictionary<int, Color32> layerDict = new Dictionary<int, Color32>();

    private const int BUILDING_LAYER = 9;
    private const int ROAD_LAYER = 10;
    private const int DEFAULT_LAYER = 0;

    void Start()
    {
        // Fill the Dictionary with Tag names and corresponding colors
        segmentDict.Add("Building", new Color32(255, 0, 0, 255));
        segmentDict.Add("Road", new Color32(0, 255, 0, 255));
        segmentDict.Add("Untagged", new Color32(0, 0, 255, 255));

        // Fill the Dictionary with Tag names and corresponding colors
        layerDict.Add(BUILDING_LAYER, new Color32(255, 0, 0, 255));
        layerDict.Add(ROAD_LAYER, new Color32(0, 255, 0, 255));
        layerDict.Add(DEFAULT_LAYER, new Color32(0, 0, 0, 255));

        // Find all GameObjects with Mesh Renderer and add a color variable to be
        // used by the shader in it's MaterialPropertyBlock
        var renderers = FindObjectsOfType<MeshRenderer>();
        var mpb = new MaterialPropertyBlock();
        foreach (var r in renderers)
        {

            if (segmentDict.TryGetValue(r.transform.tag, out Color32 outColor))
            {
                mpb.SetColor("_SegmentColor", outColor);
                r.SetPropertyBlock(mpb);
            }

            if (layerDict.TryGetValue(r.gameObject.layer, out Color32 layerColor))
            {
                mpb.SetColor("_SegmentColor", layerColor);
                r.SetPropertyBlock(mpb);
            }
        }

        // Finally set the Segment shader as replacement shader
        this.GetComponent<Camera>().SetReplacementShader(segmentShader, "RenderType");
    }
}
