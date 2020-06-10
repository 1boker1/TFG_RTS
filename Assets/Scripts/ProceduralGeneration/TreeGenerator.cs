using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.ProceduralGeneration
{
    public class TreeGenerator : MonoBehaviour
    {
        public bool Generate;

        [Space(10)]
        [SerializeField] private List<GameObject> treePrefabs;

        [Range(0, 1)]
        public float TreePercentage;

        public float MaxOffset;

        [SerializeField] private List<GameObject> GeneratedTrees = new List<GameObject>();
        [SerializeField] private List<GameObject> ActiveTrees = new List<GameObject>();

        public Transform Parent;

        public void GenerateTrees(float[,] _TreeMap, float[,] FinalNoiseMap, int Seed, float VertexDistance)
        {
            int _MapWidth = _TreeMap.GetLength(0);
            int _MapHeight = _TreeMap.GetLength(1);

            for (int z = 1; z <= _MapHeight - 1; z++)
            {
                for (int x = 1; x <= _MapWidth - 1; x++)
                {
                    if (Math.Abs(FinalNoiseMap[x, z]) < 0.5 && _TreeMap[x, z] > 1 - TreePercentage)
                    {
                        Random _Offset = new Random(Seed * x * z);

                        if (_Offset.Next(0, 100) < 75)
                        {
                            float _OffsetX = UnityEngine.Random.value * MaxOffset;
                            float _OffsetZ = UnityEngine.Random.value * MaxOffset;
                            float _OffsetRotation = UnityEngine.Random.value * 360;
                            Vector3 _Position = new Vector3(x * VertexDistance * 1.25f + _OffsetX, FinalNoiseMap[x, z],
                                z * VertexDistance * 1.25f + _OffsetZ);

                            if (HasGround(_Position, out var _Hit))
                            {
                                if (GeneratedTrees.Count > 0)
                                {
                                    GeneratedTrees[0].transform.position = _Position;
                                    GeneratedTrees[0].transform.rotation = Quaternion.Euler(0, _OffsetRotation, 0);
                                    GeneratedTrees[0].SetActive(true);
                                    ActiveTrees.Add(GeneratedTrees[0]);
                                    GeneratedTrees.RemoveAt(0);
                                }
                                else
                                {
                                    int _Rotation = _Offset.Next(0, 180);
                                    int _TreeIndex = _Offset.Next(0, treePrefabs.Count);

                                    GameObject _Tree = Instantiate(treePrefabs[_TreeIndex], _Hit.point, Quaternion.Euler(0, _Rotation, 0));
                                    _Tree.transform.parent = Parent;

                                    ActiveTrees.Add(_Tree);
                                }
                            }
                        }
                    }
                }
            }
        }
        [ContextMenu("Generate TreePool")]
        public void CreateTreePool()
        {
            for (int i = 0; i < 10000; i++)
            {
                Random _Offset = new Random();

                int _Rotation = _Offset.Next(0, 180);
                int _TreeIndex = _Offset.Next(0, treePrefabs.Count);

                GameObject _Tree = Instantiate(treePrefabs[_TreeIndex], Vector3.zero, Quaternion.Euler(0, _Rotation, 0));

                _Tree.gameObject.SetActive(false);
                _Tree.transform.parent = Parent;

                GeneratedTrees.Add(_Tree);
            }
        }
        [ContextMenu("Destroy TreePool")]
        public void DestroyTreePool()
        {
            foreach (var tree in GeneratedTrees)
            {
                if (Application.isEditor)
                    DestroyImmediate(tree);
                else
                    Destroy(tree);
            }

            GeneratedTrees.Clear();
        }

        public bool HasGround(Vector3 Position, out RaycastHit Hit)
        {
            Ray _Ray = new Ray(Position + Vector3.up, Vector3.down);

            return Physics.Raycast(_Ray, out Hit, 2);
        }

        public void ClearTrees(Transform Parent)
        {
            Transform[] _Child = Parent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < _Child.Length; i++)
            {
                if (_Child[i] != Parent)
                {
                    if (Application.isEditor)
                        DestroyImmediate(_Child[i].gameObject);
                    else
                        Destroy(_Child[i].gameObject);
                }
            }
        }
    }
}