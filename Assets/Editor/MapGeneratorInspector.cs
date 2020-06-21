using Assets.Scripts.ProceduralGeneration;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(MeshGenerator))]
    public class MapGeneratorInspector : UnityEditor.Editor
    {
		string nameFBX="File Name";
        public override void OnInspectorGUI()
        {
            MeshGenerator _MeshGenerator = (MeshGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate Mesh"))
                _MeshGenerator.GenerateMesh();
            if (GUILayout.Button("Generate Trees"))
                _MeshGenerator.GenerateTreeMap();
            if (GUILayout.Button("Clear Trees"))
                _MeshGenerator.treeGenerator.ClearTrees();
            
			GUILayout.Space(10);
			GUILayout.Label("Export And Save");
			nameFBX=GUILayout.TextField(nameFBX, 20);

			if (GUILayout.Button("Save Map As FBX"))
                _MeshGenerator.SaveAsFBX(nameFBX);
			if (GUILayout.Button("Save Preset"))
                MapPreset.CreateMapPreset(_MeshGenerator, nameFBX);

        }
    }
}
