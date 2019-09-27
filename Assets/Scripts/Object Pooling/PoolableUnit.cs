using UnityEngine;

namespace Assets.Scripts.Object_Pooling
{
    public class PoolableUnit : MonoBehaviour, IPoolable
    {
        public GameObject Key { get; set; }

        private Unit.Unit unit;

        public void Initialise(GameObject key)
        {
            Key = key;
            transform.position = Vector3.zero;

            gameObject.SetActive(false);

            if (PoolSystem.Pools.ContainsKey(Key)) PoolSystem.Pools[Key].EnQueue(this);
        }

        public void DeQueue(Vector3 position)
        {
            transform.position = position;

            gameObject.SetActive(true);
        }

        public void EnQueue()
        {
            gameObject.SetActive(false);

            PoolSystem.Pools[Key].EnQueue(this);
        }
    }
}