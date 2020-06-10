using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ProceduralGeneration
{
    public class RandomMapValues : ScriptableObject
    {
        public int MapWidth;
        public int MapHeight;
        public float VertexDistance;
        public float HeightMultiplier;

        public float GreenPercentage;
        public float HeightShift;

        public float NoiseScale;

        public int Octaves;
        public float Persistance;
        public float Lacunarity;

        public float RoundAmount;

        public int Seed;
        public Vector2 Offset;

        public float TreePercentage;
#if UNITY_EDITOR
        public static void CreateRandomMapValues(MeshGenerator MeshGenerator)
        {
            var _Values = new RandomMapValues
            {
                MapWidth = MeshGenerator.MapWidth,
                MapHeight = MeshGenerator.MapHeight,
                VertexDistance = MeshGenerator.VertexDistance,
                HeightMultiplier = MeshGenerator.HeightMultiplier,
                NoiseScale = MeshGenerator.NoiseScale,
                Octaves = MeshGenerator.Octaves,
                Persistance = MeshGenerator.Persistance,
                Lacunarity = MeshGenerator.Lacunarity,
                RoundAmount = MeshGenerator.RoundAmount,
                Seed = MeshGenerator.Seed,
                Offset = MeshGenerator.Offset,
                TreePercentage = MeshGenerator.treeGenerator.TreePercentage,
                GreenPercentage = MeshGenerator.GreenPercentage,
                HeightShift = MeshGenerator.HeightShift
            };

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects" + "/MapValues" + ".asset");

            AssetDatabase.CreateAsset(_Values, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _Values;
        }

        public void SetValues(ref MeshGenerator MeshGenerator)
        {
            MeshGenerator.MapWidth = MapWidth;
            MeshGenerator.MapHeight = MapHeight;
            MeshGenerator.VertexDistance = VertexDistance;
            MeshGenerator.HeightMultiplier = HeightMultiplier;
            MeshGenerator.NoiseScale = NoiseScale;
            MeshGenerator.Octaves = Octaves;
            MeshGenerator.Persistance = Persistance;
            MeshGenerator.Lacunarity = Lacunarity;
            MeshGenerator.RoundAmount = RoundAmount;
            MeshGenerator.Seed = Seed;
            MeshGenerator.Offset = Offset;
            MeshGenerator.treeGenerator.TreePercentage = TreePercentage;
            MeshGenerator.GreenPercentage = GreenPercentage;
            MeshGenerator.HeightShift = HeightShift;
        }
#endif
    }
}