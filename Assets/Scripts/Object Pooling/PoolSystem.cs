using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Object_Pooling
{
    public static class PoolSystem
    {
        public static List<Pool> PoolsList = new List<Pool>();
        public static Dictionary<GameObject, Pool> Pools;

        [ContextMenu("Initialize Pools")]
        public static void InitializePools()
        {
            foreach (var pool in PoolsList)
            {
                Pools.Add(pool.prefab, pool);

                pool.SpawnPrefabs();
            }
        }
    }
}