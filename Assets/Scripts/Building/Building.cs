using System;
using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Building
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BuildingData))]
    [RequireComponent(typeof(BuildingHealthSystem))]
    public class Building : MonoBehaviour, ISelectable
    {
        public bool Built { get; set; }
        public bool Selected { get; set; }
        public int Team { get; set; }

        [HideInInspector] public BuildingData buildingData;
        [HideInInspector] public BuildingConstructor constructor;

        private void Awake()
        {
            buildingData = GetComponent<BuildingData>();
            constructor = new BuildingConstructor(this);

            Team = buildingData.team;
            Built = buildingData.built;
        }

        public void Select(int? team)
        {
            Selected = true;

            constructor.constructionLine.enabled = true;

            if (!SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Add(this);
        }

        public void Deselect()
        {
            Selected = false;

            constructor.constructionLine.enabled = false;


            if (SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Remove(this);
        }

        public IEnumerator Highlight()
        {
            for (int i = 0; i < 6; i++)
            {
                constructor.constructionLine.enabled = !constructor.constructionLine.enabled;

                yield return new WaitForSeconds(.2f);
            }

            constructor.constructionLine.enabled = false;
        }

        public Type GetAction(Unit.Unit unit, int? team)
        {
            unit.Target = Built ? null : this;

            return GetActionWithoutTarget(unit, team);
        }

        public Type GetActionWithoutTarget(Unit.Unit unit, int? team)
        {
            if (team != Team) return typeof(AttackState);

            return Built ? typeof(IdleState) : typeof(BuildState);
        }
    }
}