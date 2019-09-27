using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
    public class AttackState : State
    {
        private Unit.Unit unit;

        [SerializeField] public UnityEvent OnOutOfRangeTarget;
        [SerializeField] public UnityEvent OnNullTarget;

        private float timer = 0;

        public override void Enter()
        {
            if (unit == null) unit = GetComponent<Unit.Unit>();
            Utils.LookTarget(unit, unit.Target);
        }

        public override void Execute()
        {
            if (unit.Target.Equals(null)) OnNullTarget?.Invoke();
            else if (unit.Target != null && !Utils.InRange(unit, unit.Target, unit.unitData.AttackRange)) OnOutOfRangeTarget?.Invoke();
            else PrepareAttack();
        }

        public override void Exit() { }

        private void PrepareAttack()
        {
            timer += Time.deltaTime;

            if (timer > unit.unitData.AttackSpeed)
            {
                timer = 0;

                AttackTarget();
            }
        }

        private void AttackTarget()
        {
            Utils.LookTarget(unit, unit.Target);
            unit.Target.transform.GetComponent<IHealth>().ModifyHealth((int)unit.unitData.AttackDamage, unit);

            if (unit.Target.Equals(null)) OnNullTarget?.Invoke();
        }
    }
}
