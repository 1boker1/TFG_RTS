using UnityEngine;

namespace Assets.Scripts.Unit
{
    public abstract class State : MonoBehaviour
    {
        public abstract void Enter();

        public abstract void Execute();

        public abstract void Exit();
    }
}