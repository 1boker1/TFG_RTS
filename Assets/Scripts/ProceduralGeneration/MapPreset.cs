using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.ProceduralGeneration
{
	public class MapPreset:ScriptableObject
	{
		public float HeightMultiplier;

		public float GreenPercentage;
		public float HeightShift;

		public float NoiseScale;

		public int Octaves;
		public float Persistance;
		public float Lacunarity;

		public float RoundAmount;

		public float TreePercentage;
#if UNITY_EDITOR
		public static void CreateMapPreset(MeshGenerator MeshGenerator,string Name)
		{
			var _Values = new MapPreset
			{
				HeightMultiplier=MeshGenerator.HeightMultiplier,
				NoiseScale=MeshGenerator.NoiseScale,
				Octaves=MeshGenerator.Octaves,
				Persistance=MeshGenerator.Persistance,
				Lacunarity=MeshGenerator.Lacunarity,
				RoundAmount=MeshGenerator.RoundAmount,
				GreenPercentage=MeshGenerator.GreenPercentage,
				HeightShift=MeshGenerator.HeightShift,

				TreePercentage=MeshGenerator.treeGenerator.TreePercentage
			};

			if(Name=="")
				Name="MapValues";

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObjects"+"/"+Name+".asset");

			AssetDatabase.CreateAsset(_Values,assetPathAndName);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject=_Values;
		}
#endif
		public void LoadPreset(ref MeshGenerator MeshGenerator)
		{
			MeshGenerator.HeightMultiplier=HeightMultiplier;
			MeshGenerator.NoiseScale=NoiseScale;
			MeshGenerator.Octaves=Octaves;
			MeshGenerator.Persistance=Persistance;
			MeshGenerator.Lacunarity=Lacunarity;
			MeshGenerator.RoundAmount=RoundAmount;
			MeshGenerator.treeGenerator.TreePercentage=TreePercentage;
			MeshGenerator.GreenPercentage=GreenPercentage;
			MeshGenerator.HeightShift=HeightShift;
		}
	}
}