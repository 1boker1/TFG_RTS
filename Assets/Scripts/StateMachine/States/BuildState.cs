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

        [SerializeField] private GameObject tool;

        public override void Enter()
        {
            builderData = GetComponent<IBuildData>();
            building = unit.Target.transform.GetComponent<IBuildable>();

            Utils.LookTarget(unit, unit.Target);

            if (tool != null)
                tool.SetActive(true);
        }

        public override void Execute()
        {
            if (building == null)
                OnBuildingDone?.Invoke();

            if (!Utils.InRange(unit, unit.Target, unit.unitData.UnitRange)) { unit.ChangeState(typeof(ChaseState)); return; }

            timer += Time.deltaTime;

            if (timer > 1 / builderData.BuildSpeed)
            {
                timer = 0;

                Construct();
            }
        }

        public override void Exit()
        {
            animator.SetBool(AnimationBool, false);

            if (tool != null)
                tool.SetActive(false);
        }

        private void Construct()
        {
            SetAnimation();

            building.Build(builderData.BuildAmount);

            if (building.Built)
                OnBuildingDone?.Invoke();
        }
    }
}
