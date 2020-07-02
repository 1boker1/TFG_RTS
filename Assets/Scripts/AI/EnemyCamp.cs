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
		public List<Unit.Unit> InsideUnits;

		public Objective currentObjective;

		float Radius;

		private void Start()
		{
			Radius = GetComponent<SphereCollider>().radius+25;
		}

		void Update()
	    {
			if(Completed())
				return;

			if(currentTarget!=null && campUnits.Count>0 && campUnits[0].stateMachine.currentState.GetType()!=typeof(PatrolAreaState))
			{
				if(Vector3.Distance(transform.position, currentTarget.transform.position)> Radius)
				{
					currentTarget=null;
					OnTargetTooFar();
				}
			}
			else if (campUnits.Count!=0 && campUnits[0].Target==null && campUnits[0].stateMachine.currentState.GetType()!=typeof(PatrolAreaState))
			{
				OnTargetTooFar();
			}

			if(currentTarget==null && InsideUnits.Count!=0)
			{
				InsideUnits.RemoveAll(item => item == null);

				foreach(var _Unit in campUnits)
				{
					if(InsideUnits.Count>0)
					_Unit.OnTarget(InsideUnits[0]);
				}
			}
	    }
	
		private void OnTriggerEnter(Collider other)
		{
			Unit.Unit _Unit=other.GetComponent<Unit.Unit>();

			if(_Unit!=null && !campUnits.Contains(_Unit) && _Unit.Team==SelectionManager.Instance.m_Team)
			{
				if(currentTarget==null)
					OnNewTarget(_Unit);
				
				InsideUnits.Add(_Unit);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if(other.GetComponent<Unit.Unit>()!=null)
			{
				if(InsideUnits.Contains(other.GetComponent<Unit.Unit>()))
					InsideUnits.Remove(other.GetComponent<Unit.Unit>());
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

			transform.DetachChildren();
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
