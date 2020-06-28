using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
	public class PatrolAreaState : State
	{
		private Unit.Unit unit;
        private NavMeshAgent navMeshAgent;

		public float areaToPatrol=20f;
		public float patrolSpeed=5f;
		private float initialSpeed;

		private Vector3 initialPoint;
		private Vector3 currentPoint;

        [SerializeField] private UnityEvent OnEnemyFound;

		private void Start()
		{
			Initialise();
			initialPoint=transform.position;
		}

		public override void Enter()
		{
			Initialise();

			SetAnimation();
			currentPoint=GetNewPatrolPoint();
			navMeshAgent.SetDestination(currentPoint);
			initialSpeed=navMeshAgent.speed;
			navMeshAgent.speed=patrolSpeed;
		}

		public override void Execute()
		{
			if (Utils.InRange(transform.position, currentPoint, navMeshAgent.stoppingDistance))
			{
				currentPoint=GetNewPatrolPoint();
				if(currentPoint==Vector3.positiveInfinity)
					currentPoint=GetNewPatrolPoint();

				navMeshAgent.SetDestination(currentPoint);
			}
		}

		public override void Exit()
		{
			navMeshAgent.SetDestination(transform.position);
            animator.SetBool(AnimationBool, false);
			navMeshAgent.speed=initialSpeed;
		}

		public Vector3 GetNewPatrolPoint()
		{
			Vector2 _Point=Random.insideUnitCircle*areaToPatrol;

			NavMesh.SamplePosition(initialPoint+new Vector3(_Point.x,0,_Point.y), out NavMeshHit hit, 20, NavMesh.AllAreas);

			return hit.position;
		}

		private void Initialise()
        {
            if (unit == null) unit = GetComponent<Unit.Unit>();
            if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
			if(initialPoint==Vector3.zero)	initialPoint = transform.position;
        }
	}
}