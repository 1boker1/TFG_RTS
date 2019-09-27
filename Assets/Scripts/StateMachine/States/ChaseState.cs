using Assets.Scripts.Managers;
using Assets.Scripts.Unit;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
    public class ChaseState : State
    {
        private Unit.Unit unit;
        private NavMeshAgent navMeshAgent;
        private NavMeshObstacle navMeshObstacle;

        [SerializeField] private UnityEvent OnReachedTarget;
        [SerializeField] private UnityEvent OnNullTarget;

        private Vector3 destination = Vector3.positiveInfinity;

        private Vector3 lastTargetPosition = Vector3.zero;
        private bool StaticTarget { get; set; }

        public override void Enter()
        {
            if (unit == null) unit = GetComponent<Unit.Unit>();
            if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshObstacle == null) navMeshObstacle = GetComponent<NavMeshObstacle>();

            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
            StaticTarget = !unit.Target.transform.GetComponent<NavMeshAgent>();
            destination = Utils.ClosestPoint(unit, unit.Target);
            lastTargetPosition = unit.Target.transform.position;
            navMeshAgent.SetDestination(destination);
        }

        public override void Execute()
        {
            if (unit.Target.Equals(null)) { OnNullTarget?.Invoke(); return; }

            if (ObstacleAvoidance.ObstacleAvoidance.DestinationBlocked(unit.transform, destination, unit.Target.transform)) ObstacleAvoidance.ObstacleAvoidance.FindClearDestination(unit, ref destination, navMeshAgent);
            else
                CheckForNewTargetPosition();

            if (Utils.InRange(unit, unit.Target, navMeshAgent.stoppingDistance)) OnReachedTarget?.Invoke();
        }

        public override void Exit()
        {
            navMeshAgent.SetDestination(navMeshAgent.transform.position);

            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        }

        private void CheckForNewTargetPosition()
        {
            if (StaticTarget) return;

            var currentTargetPosition = unit.Target.transform.position;

            if (lastTargetPosition == currentTargetPosition) return;

            destination = Utils.ClosestPoint(unit, unit.Target);

            navMeshAgent.SetDestination(destination);
        }
    }
}