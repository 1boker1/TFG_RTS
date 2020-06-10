using System;
using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Building
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(BuildingData))]
    [RequireComponent(typeof(BuildingHealthSystem))]
    public class Building : MonoBehaviour, ISelectable
    {
        public bool Selected { get; set; }
        public int Team { get; set; }

        private BuildingData buildingData;
        public BuildingData GetBuildingData() => buildingData;

        private BuildingHealthSystem healthSystem;
        public BuildingHealthSystem GetBuildingHealthSystem() => healthSystem;

        private void Awake()
        {
            buildingData = GetComponent<BuildingData>();
            healthSystem = GetComponent<BuildingHealthSystem>();

            Team = buildingData.team;

            transform.position = transform.position.With(y: 0.01f);
        }

        public void Select(int? team)
        {
            Selected = true;

            SelectionLine.Instance.LineSetup(gameObject);

            if (!SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Add(this);
        }

        public void Deselect()
        {
            Selected = false;

            if (SelectionLine.Instance != null)
                SelectionLine.Instance.EnableLine(false);

            if (SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Remove(this);
        }

        public IEnumerator Highlight()
        {
            SelectionLine.Instance.HighlightObject(this);
            yield break;
        }

        public Type GetAction(Unit.Unit unit, int? team)
        {
            if (team != Team)
                unit.Target = this;
            else
                unit.Target = healthSystem.Built ? null : this;

            return GetActionWithoutTarget(unit, team);
        }

        public Type GetActionWithoutTarget(Unit.Unit unit, int? team)
        {
            if (team != Team)
                return typeof(AttackState);

            return healthSystem.Built ? typeof(IdleState) : typeof(BuildState);
        }

        private void OnDestroy()
        {
            Deselect();
        }
    }
}