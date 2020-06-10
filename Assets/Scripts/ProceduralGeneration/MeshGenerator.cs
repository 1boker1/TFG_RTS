using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.AI;

namespace Assets.Scripts.ProceduralGeneration
{
    public class MeshGenerator : MonoBehaviour
    {
        public MapPreset Preset;
        public bool UsePreset;

        public int Seed;
        public Vector2 Offset;

        [Space(20)]
        [Range(0, 250)] public int MapWidth;
        [Range(0, 250)] public int MapHeight;

        public float VertexDistance;
        public float HeightMultiplier;

        [Range(0.1f, 1)] public float GreenPercentage = 0.5f;
        [Range(0.1f, 1)] public float HeightShift = 0.5f;

        public float NoiseScale;

        [Range(0, 10)] public int Octaves;
        [Range(0, 1)] public float Persistance;
        public float Lacunarity;

        [Range(0.01f, 1f)] public float RoundAmount;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public NavMeshSurface navMesh;

        public TreeGenerator treeGenerator;

        private Mesh mesh;

        private Vector3[] vertices;
        private int[] triangles;
        private float[,] FinalNoiseMap;

        private float MaxHeight;
        private float MinHeight;

        //Debug
        private float StartTime;

        private void Start()
        {
            if (Time.realtimeSinceStartup < 5)
            {
                Debug.Log(Time.realtimeSinceStartup);
                GenerateMesh();
            }
        }

        private void OnValidate()
        {
            Seed = Mathf.Max(Seed, 0);
        }

        public void GenerateMesh()
        {
            StartTime = Time.realtimeSinceStartup;

            if (Preset != null && UsePreset)
            {
                MeshGenerator meshGenerator = this;
                Preset.LoadPreset(ref meshGenerator);
            }

            mesh = new Mesh();
            meshFilter.mesh = mesh;

            if (Seed == 0)
            {
                System.Random _RandomNumber = new System.Random();
                Seed = _RandomNumber.Next();
            }

            CreateMesh();
            UpdateMesh();
        }

        public void CreateMesh()
        {
            vertices = new Vector3[(MapWidth + 1) * (MapHeight + 1)];
            FinalNoiseMap = new float[MapWidth + 1, MapHeight + 1];

            float[,] _NoiseMap = Noise.GenerateNoiseMap(MapWidth + 1, MapHeight + 1, Seed, NoiseScale, Octaves, Persistance, Lacunarity, Offset);

            ShiftNoiseMapBy(HeightShift, _NoiseMap);

            int index = 0;

            MaxHeight = float.MinValue;
            MinHeight = float.MaxValue;

            for (int z = 0; z <= MapHeight; z++)
            {
                for (int x = 0; x <= MapWidth; x++)
                {
                    float _Threshold = (GreenPercentage - 0.5f);

                    if (_NoiseMap[x, z] < _Threshold && _NoiseMap[x, z] > -_Threshold)
                        _NoiseMap[x, z] = 0;
                    else
                        _NoiseMap[x, z] *= 1 - GreenPercentage;

                    FinalNoiseMap[x, z] = Round(_NoiseMap[x, z] * HeightMultiplier);

                    UnityEngine.Random.InitState(Seed * x * z);

                    float _OffsetY = (UnityEngine.Random.value - 0.5f) * 0.5f;

                    FinalNoiseMap[x, z] += _OffsetY;

                    if (FinalNoiseMap[x, z] < MinHeight)
                        MinHeight = FinalNoiseMap[x, z];
                    else if (FinalNoiseMap[x, z] > MaxHeight)
                        MaxHeight = FinalNoiseMap[x, z];

                    vertices[index] = new Vector3(x * VertexDistance, FinalNoiseMap[x, z], z * VertexDistance);
                    index++;
                }
            }

            triangles = new int[MapWidth * MapHeight * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < MapHeight; z++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    triangles[tris + 0] = vert + 0;             //p0
                    triangles[tris + 1] = vert + MapWidth + 1;  //p1
                    triangles[tris + 2] = vert + 1;             //p2
                    triangles[tris + 3] = vert + 1;             //p2
                    triangles[tris + 4] = vert + MapWidth + 1;  //p1
                    triangles[tris + 5] = vert + MapWidth + 2;  //p3

                    vert++;

                    tris += 6;
                }

                vert++;
            }
        }

        private void ShiftNoiseMapBy(float ShiftValue, float[,] NoiseMap)
        {
            for (int z = 0; z < NoiseMap.GetLength(1); z++)
            {
                for (int x = 0; x < NoiseMap.GetLength(0); x++)
                {
                    NoiseMap[x, z] -= ShiftValue;
                }
            }
        }

        public void UpdateMesh()
        {
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            mesh.MarkDynamic();

            if (MaxHeight < 1) MaxHeight = 1;
            if (MinHeight > -1) MinHeight = -1;

            meshRenderer.sharedMaterial.SetFloat("MaxY", MaxHeight);
            meshRenderer.sharedMaterial.SetFloat("MinY", MinHeight);

            MeshCollider collider = meshRenderer.GetComponent<MeshCollider>();

            if (collider == null)
                collider = meshRenderer.gameObject.AddComponent<MeshCollider>();

            collider.sharedMesh = mesh;

            Debug.Log("Mesh: " + ((Time.realtimeSinceStartup - StartTime) * 1000f) + "ms");
        }

        public void BuildNavigationMesh()
        {
            navMesh.size = new Vector3(MapWidth * VertexDistance, 1, MapHeight * VertexDistance);
            navMesh.center = new Vector3(MapWidth * VertexDistance * 0.5f, 0, MapHeight * VertexDistance * 0.5f);
            navMesh.BuildNavMesh();

            Debug.Log("NavMesh: " + ((Time.realtimeSinceStartup - StartTime) * 1000f) + "ms");
        }

        public void GenerateTreeMap()
        {
            float[,] _TreeMap = Noise.GenerateNoiseMap(MapWidth/(5/4) + 1, MapHeight/(5/4) + 1, Seed * Seed + 1, NoiseScale * .5f, Octaves, Persistance, Lacunarity, Offset);

            treeGenerator.GenerateTrees(_TreeMap, FinalNoiseMap, Seed, VertexDistance);

            Debug.Log("Trees: " + ((Time.realtimeSinceStartup - StartTime) * 1000f) + "ms");
        }

        public Vector3 GetValidMapPosition(Vector3 Size, int MaxIterations)
        {
            List<List<Vector3>> Regions = GetRegions();
            List<Vector3> _InvalidPositions = new List<Vector3>();

            Vector3 _Position = Vector3.zero;
            int iterations = 0;

            while (true)
            {
                if (iterations > MaxIterations) //To not break Unity
                    return Vector3.zero;

                iterations++;

                int _RandomRegionIndex = UnityEngine.Random.Range(0, Regions.Count);
                int _RandomPointIndex = UnityEngine.Random.Range(0, Regions[_RandomRegionIndex].Count);

                Vector3 _RandomRegionPoint = Regions[_RandomRegionIndex][_RandomPointIndex];

                if (!_InvalidPositions.Contains(_RandomRegionPoint))
                {
                    _InvalidPositions.Add(_RandomRegionPoint);

                    if (_RandomRegionPoint.x > 25 && _RandomRegionPoint.x < MapWidth - 25 &&
                    _RandomRegionPoint.z > 25 && _RandomRegionPoint.z < MapHeight - 25)
                    {
                        _Position = (_RandomRegionPoint * VertexDistance).With(y: 2);

                        Collider[] colliders = Physics.OverlapBox(_Position, Size);

                        if (colliders.Length == 0)
                            return _Position;
                    }
                }
            }
        }

        public void FlatMapInRadius(Vector3 Point, float Radius)
        {
            Vector3[] _Vertices = meshFilter.mesh.vertices;

            for (int i = 0; i < _Vertices.Length; i++)
            {
                if (Vector3.Distance(_Vertices[i], Point.With(y: 0)) <= Radius)
                {
                    float _OffsetY = (UnityEngine.Random.value - 0.5f) * 0.5f;
                    _Vertices[i] = new Vector3(_Vertices[i].x, _OffsetY, _Vertices[i].z);
                }
            }

            meshFilter.mesh.SetVertices(_Vertices);
            meshFilter.mesh.RecalculateNormals();
        }

        public List<List<Vector3>> GetRegions()
        {
            List<List<Vector3>> regions = new List<List<Vector3>>();
            int[,] mapFlags = new int[MapWidth, MapHeight];

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (mapFlags[x, y] == 0 && Mathf.Abs(FinalNoiseMap[x, y]) < 1)
                    {
                        List<Vector3> newRegion = GetRegionTiles(x, y);
                        if (newRegion.Count > 100)
                        {
                            regions.Add(newRegion);

                            foreach (Vector3 tile in newRegion)
                            {
                                mapFlags[(int)tile.x, (int)tile.z] = 1;
                            }
                        }
                    }
                }
            }

            return regions;
        }

        List<Vector3> GetRegionTiles(int startX, int startY)
        {
            List<Vector3> tiles = new List<Vector3>();
            int[,] mapFlags = new int[MapWidth, MapHeight];
            int tileType = (int)FinalNoiseMap[startX, startY];

            Queue<Vector3> queue = new Queue<Vector3>();
            queue.Enqueue(new Vector3(startX, 0, startY));
            mapFlags[startX, startY] = 1;

            while (queue.Count > 0)
            {
                Vector3 tile = queue.Dequeue();
                tiles.Add(tile);

                for (int x = (int)tile.x - 1; x <= tile.x + 1; x++)
                {
                    for (int y = (int)tile.z - 1; y <= tile.z + 1; y++)
                    {
                        if (IsInMapRange(x, y) && (y == (int)tile.z || x == (int)tile.x))
                        {
                            if (mapFlags[x, y] == 0 && Mathf.Abs(FinalNoiseMap[x, y]) < 1)
                            {
                                mapFlags[x, y] = 1;
                                queue.Enqueue(new Vector3(x, 0, y));
                            }
                        }
                    }
                }
            }

            return tiles;
        }

        private bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;
        }

        public float Round(float Value)
        {
            return Mathf.Round(Value * RoundAmount) / RoundAmount;
        }
    }
}
