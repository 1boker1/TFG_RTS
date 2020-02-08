using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[ExecuteInEditMode]
public class SliceObject : MonoBehaviour
{
    public GameObject graphic;
    public Material[] materials;

    private void Start()
    {
        materials = GetMaterials(graphic);
    }

    private void Update()
    {
        foreach (var material in materials)
        {
            material.SetVector("_SliceCenter", transform.position);
            material.SetVector("_SliceNormal", transform.forward);
        }
    }

    public Material[] GetMaterials(GameObject graphic)
    {
        var renderers = graphic.GetComponentsInChildren<MeshRenderer>();

        return renderers.SelectMany(rend => rend.materials).ToArray();
    }
}