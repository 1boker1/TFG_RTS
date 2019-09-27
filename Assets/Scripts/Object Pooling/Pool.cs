using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Object_Pooling
{
    public class Pool : MonoBehaviour
    {
        public GameObject prefab;

        public int amount;

        public Queue<IPoolable> pooledObjects = new Queue<IPoolable>();

        public void SpawnPrefabs()
        {
            if (prefab.GetComponent<IPoolable>() == null) return;

            for (int i = 0; i < amount; i++)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<IPoolable>().Initialise(prefab);
            }
        }

        public void DeQueue(Vector3 position)
        {
            pooledObjects.Dequeue().DeQueue(position);
        }

        public void EnQueue(IPoolable pooledObject)
        {
            pooledObjects.Enqueue(pooledObject);
        }
    }
}