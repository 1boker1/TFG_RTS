using UnityEngine;

namespace Assets.Scripts.Object_Pooling
{
    public interface IPoolable
    {
        Transform transform { get; }
        GameObject Key { get; set; }

        void Initialise(GameObject key);
        void DeQueue(Vector3 position);
        void EnQueue();
    }
}