using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShadowsOnlyEnabler : MonoBehaviour
{
    [SerializeField] private GameObject[] excludeList;

    // Start is called before the first frame update
    void Start()
    {
        this.RenderOnlyShadows();
    }

    private bool IsExcluded(GameObject gameObject)
    {
        for (int i = 0; i < this.excludeList.Length; i++)
        {
            if (this.excludeList[i].GetInstanceID() == gameObject.GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }

    private void RenderOnlyShadows()
    {
        MeshRenderer[] meshRenderList = this.GetComponentsInChildren<MeshRenderer>();
        Debug.Log("Length:" + meshRenderList.Length);

        for (int i = 0; i < meshRenderList.Length; i++)
        {
            if (this.IsExcluded(meshRenderList[i].gameObject) == false)
            {
                meshRenderList[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }
}
