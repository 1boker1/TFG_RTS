using Assets.Scripts.ProceduralGeneration;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(MeshGenerator))]
    public class MapGeneratorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            MeshGenerator _MeshGenerator = (MeshGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mesh"))
                _MeshGenerator.GenerateMesh();
            if (GUILayout.Button("Generate Trees"))
                _MeshGenerator.GenerateTreeMap();
            if (GUILayout.Button("Clear Trees"))
                _MeshGenerator.treeGenerator.ClearTrees(_MeshGenerator.meshRenderer.transform);
            if (GUILayout.Button("Save Values"))
                MapPreset.CreateMapPreset(_MeshGenerator);
        }
    }
}
