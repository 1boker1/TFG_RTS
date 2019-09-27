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
        private NavMeshObstacle navMeshObstacle;

        [SerializeField] private UnityEvent OnDestinationReached;

        public override void Enter()
        {
            Initialise();

            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(unit.MoveDestination);
        }

        public override void Execute()
        {
            if (Utils.InRange(transform.position, unit.MoveDestination, navMeshAgent.stoppingDistance)) OnDestinationReached?.Invoke();
        }

        public override void Exit()
        {
            navMeshAgent.SetDestination(navMeshAgent.transform.position);
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        }

        private void Initialise()
        {
            if (unit == null) unit = GetComponent<Unit.Unit>();
            if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshObstacle == null) navMeshObstacle = GetComponent<NavMeshObstacle>();
        }
    }
}