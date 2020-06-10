using System.Collections.Generic;
using System.ComponentModel.Design;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Scripts.Building
{
    public class BuildingHealthSystem : MonoBehaviour, IHealth, IBuildable
    {
        public bool Built { get; set; }

        public int HealthPoints { get; set; }
        public int MaxHealthPoints { get; set; }

        public UnityEvent OnBuilt;

        private List<Material> materials = new List<Material>();

        private float colliderHeight;

        private void Start()
        {
            MeshRenderer _MeshRenderer = GetComponent<MeshRenderer>();

            _MeshRenderer.GetMaterials(materials);

            var min = _MeshRenderer.bounds.min.y;
            var max = _MeshRenderer.bounds.max.y / transform.localScale.y;

            foreach (var mat in materials)
            {
                mat.SetFloat("_MinY", min);
                mat.SetFloat("_MaxY", max + 0.1f);
            }

            BuildingData _BuildingData = GetComponent<BuildingData>();

            Built = _BuildingData.built;
            MaxHealthPoints = _BuildingData.MaxHealthPoints;

            HealthPoints = Built ? MaxHealthPoints : 1;

            if (Built) FinishBuild();
            else ConstructionAnimation();
        }

        public void OutOufHealth()
        {
            Destroy(gameObject);
        }

        public void ModifyHealth(int value, ISelectable attacker)
        {
            if (HealthPoints <= 0) return;

            HealthPoints -= value;

            if (HealthPoints <= 0)
            {
                HealthPoints = 0;

                OutOufHealth();
            }
        }

        public void Build(int amount)
        {
            if (HealthPoints < MaxHealthPoints)
            {
                ConstructionAnimation();

                HealthPoints += amount;
                HealthPoints = Mathf.Clamp(HealthPoints, 0, MaxHealthPoints);
            }

            if (HealthPoints == MaxHealthPoints && !Built) FinishBuild();
        }

        private void FinishBuild()
        {
            Built = true;
            HealthPoints = MaxHealthPoints;

            ConstructionAnimation();

            if (GetComponent<NavMeshObstacle>() != null)
                GetComponent<NavMeshObstacle>().carving = true;

            OnBuilt?.Invoke();
        }

        public void ConstructionAnimation()
        {
            float fillPercentage = (float)HealthPoints / (float)MaxHealthPoints;

            foreach (var mat in materials)
                mat.SetFloat("_Percentage", fillPercentage);
        }
    }
}