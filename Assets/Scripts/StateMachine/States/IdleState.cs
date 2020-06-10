using Assets.Scripts.Unit;

namespace Assets.Scripts.StateMachine.States
{
    public class IdleState : State
    {
        public override void Enter()
        {
            if (animator != null)
                animator.SetBool(AnimationBool, true);
        }

        public override void Execute()
        {

        }

        public override void Exit()
        {
            if (animator != null)
                animator.SetBool(AnimationBool, false);
        }
    }
}