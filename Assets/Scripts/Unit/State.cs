using UnityEngine;

namespace Assets.Scripts.Unit
{
    public abstract class State : MonoBehaviour
    {
        public string AnimationBool;

        protected Animator animator;

        private void SetUp()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            SetUp();
        }

        protected void SetAnimation()
        {
            if (!animator.GetBool(AnimationBool))
                animator.SetBool(AnimationBool, true);
        }

        public abstract void Enter();

        public abstract void Execute();

        public abstract void Exit();
    }
}