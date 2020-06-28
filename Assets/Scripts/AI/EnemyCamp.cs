using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.StateMachine.States;
using UnityEngine;

namespace Assets.Scripts
{
	[RequireComponent(typeof(SphereCollider))]
	public class EnemyCamp : MonoBehaviour
	{
	    private Unit.Unit currentTarget;
		public List<Unit.Unit> campUnits;

		public Objective currentObjective;

	    void Update()
	    {
			if(currentTarget!=null && campUnits[0].stateMachine.currentState.GetType()!=typeof(PatrolAreaState))
			{
				if(Vector3.Distance(transform.position, currentTarget.transform.position)>25f)
				{
					currentTarget=null;
					OnTargetTooFar();
				}
			}
			else if (campUnits.Count!=0 && campUnits[0].Target==null && campUnits[0].stateMachine.currentState.GetType()!=typeof(PatrolAreaState))
			{
				OnTargetTooFar();
			}
	    }
	
		private void OnTriggerEnter(Collider other)
		{
			Unit.Unit _Unit=other.GetComponent<Unit.Unit>();

			if(_Unit!=null && !campUnits.Contains(_Unit))
			{
				OnNewTarget(_Unit);
			}
		}

		public void SetUp()
		{
			foreach(Unit.Unit _Unit in campUnits)
			{
				_Unit.OnUnitDestroyed+=CampUnitKilled;
				_Unit.transform.parent=null;
				_Unit.gameObject.SetActive(true);
			}
		}

		public void CampUnitKilled(Unit.Unit unit)
		{
			if(campUnits.Contains(unit))
				campUnits.Remove(unit);

			if(campUnits.Count==0)
			{
				if(currentObjective.Completed())
					currentObjective.OnCompleted();
			}

		}

		public bool Completed()
		{
			return campUnits.Count==0;
		}

		private void OnNewTarget(Unit.Unit newTarget)
		{
			if(currentTarget==null)
			{
				currentTarget=newTarget;

				foreach(var _Unit in campUnits)
				{
					_Unit.OnTarget(currentTarget);
				}
			}
		}

		private void OnTargetTooFar()
		{
			foreach(var _Unit in campUnits)
			{
				_Unit.Target=null;
				_Unit.ChangeState(typeof(PatrolAreaState));
			}
		}
	}
}
