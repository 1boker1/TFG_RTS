using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
    public class MoveState : State
    {
        private Unit.Unit unit;

        private NavMeshAgent navMeshAgent;

        [SerializeField] private UnityEvent OnDestinationReached;

        public override void Enter()
        {
            Initialise();

            navMeshAgent.SetDestination(unit.MoveDestination);

            SetAnimation();
        }

        public override void Execute()
        {
            if (Utils.InRange(transform.position, unit.MoveDestination, navMeshAgent.stoppingDistance))
                OnDestinationReached?.Invoke();
        }

        public override void Exit()
        {
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
            animator.SetBool(AnimationBool, false);
        }

        private void Initialise()
        {
            if (unit == null) unit = GetComponent<Unit.Unit>();
            if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
        }
    }
}