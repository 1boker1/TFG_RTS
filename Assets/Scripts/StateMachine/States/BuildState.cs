using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.StateMachine.States
{
    public class BuildState : State
    {
        [SerializeField] private Unit.Unit unit;

        [SerializeField] private UnityEvent OnBuildingDone;

        private float timer = 0;

        private IBuildable building;
        private IBuildData builderData;

        public override void Enter()
        {
            builderData = GetComponent<IBuildData>();
            building = unit.Target.transform.GetComponent<IBuildable>();

            Utils.LookTarget(unit, unit.Target);
        }

        public override void Execute()
        {
            if (building == null) OnBuildingDone?.Invoke();
            if (!Utils.InRange(unit, unit.Target, unit.unitData.AttackRange)) { unit.ChangeState(typeof(ChaseState)); return; }

            timer += Time.deltaTime;

            if (timer > 1 / builderData.BuildSpeed)
            {
                timer = 0;

                Construct();
            }
        }

        public override void Exit() { }

        private void Construct()
        {
            building.Build(builderData.BuildAmount);

            if (building.Built) OnBuildingDone?.Invoke();
        }
    }
}
