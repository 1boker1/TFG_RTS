using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
    public class GatherState : State
    {
        private IHealth resource;

        [SerializeField] private Unit.Unit unit;

        [SerializeField] private UnityEvent OnResourceGathered;

        private float timer = 0;

        public float range = 1.5f;

        public override void Enter()
        {
            Utils.LookTarget(unit, unit.Target);
            resource = unit.Target.transform.GetComponent<IHealth>();
        }

        public override void Execute()
        {
            if (resource.Equals(null)) { OnResourceGathered?.Invoke(); return; }
            if (!Utils.InRange(unit, unit.Target, unit.unitData.AttackRange)) { unit.ChangeState(typeof(ChaseState)); return; }

            GatherResource();
        }

        public override void Exit() { }

        private void GatherResource()
        {
            timer += Time.deltaTime;

            if (!(timer > unit.unitData.GatherSpeed)) return;

            timer = 0;

            resource.ModifyHealth(unit.unitData.GatherAmount, unit);
        }
    }
}