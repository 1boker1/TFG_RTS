using System;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
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

        [SerializeField] private GameObject miningTool;
        [SerializeField] private GameObject choppingTool;
        [SerializeField] private GameObject farmingTool;

        private GameObject currentTool;

        public string miningAnimation;
        public string choppingAnimation;
        public string farmingAnimation;

        private string currentAnimation;


        public override void Enter()
        {
            Utils.LookTarget(unit, unit.Target);
            resource = unit.Target.transform.GetComponent<IHealth>();
        }

        private void SetGatheringTool()
        {
            Resource _Resource = unit.Target.transform.GetComponent<Resource>();

            if (resource == null)
                return;

            switch (_Resource.resourceType)
            {
                case Collections.ResourceType.Wood:
                    currentTool = choppingTool;
                    currentAnimation = choppingAnimation;
                    break;
                case Collections.ResourceType.Food:
                    currentTool = farmingTool;
                    currentAnimation = farmingAnimation;
                    break;
                case Collections.ResourceType.Gold:
                    currentTool = miningTool;
                    currentAnimation = miningAnimation;
                    break;
                case Collections.ResourceType.Rock:
                    currentTool = miningTool;
                    currentAnimation = miningAnimation;
                    break;
            }
        }

        public override void Execute()
        {
            if (resource == null || resource.Equals(null) || resource.HealthPoints <= 0) { OnResourceGathered?.Invoke(); return; }
            if (!Utils.InRange(unit, unit.Target, unit.unitData.UnitRange)) { unit.ChangeState(typeof(ChaseState)); return; }

            GatherResource();
        }

        public override void Exit()
        {
            animator.SetBool(currentAnimation, false);
            if (currentTool)
                currentTool.SetActive(false);
        }

        private void GatherResource()
        {
            SetAnimation();

            timer += Time.deltaTime;

            if (!(timer > unit.unitData.GatherSpeed))
                return;

            timer = 0;
            Utils.LookTarget(unit, unit.Target);
            resource.ModifyHealth(unit.unitData.GatherAmount, unit);
        }

        protected new void SetAnimation()
        {
            if (currentTool != null && currentTool.activeSelf)
                return;

            SetGatheringTool();

            animator.SetBool(currentAnimation, true);
            currentTool.SetActive(true);
        }
    }
}