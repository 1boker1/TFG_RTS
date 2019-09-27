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

        private Building building;

        public UnityEvent OnBuilt;

        private void Start()
        {
            building = GetComponent<Building>();

            Built = building.buildingData.built;
            MaxHealthPoints = building.buildingData.MaxHealthPoints;

            HealthPoints = Built ? MaxHealthPoints : 1;

            if (Built) FinishBuild();
            else building.constructor.ConstructionAnimation();
        }

        public void OutOufHealth()
        {
            Destroy(building.gameObject);
        }

        public void ModifyHealth(int value, ISelectable attacker)
        {
            if (HealthPoints <= 0) return;

            HealthPoints -= value;

            if (HealthPoints < 0)
            {
                HealthPoints = 0;

                OutOufHealth();
            }
        }

        public void Build(int amount)
        {
            if (HealthPoints < MaxHealthPoints)
            {
                building.constructor.ConstructionAnimation();

                HealthPoints += amount;
                HealthPoints = Mathf.Clamp(HealthPoints, 0, MaxHealthPoints);
            }

            if (HealthPoints == MaxHealthPoints) FinishBuild();
        }

        private void FinishBuild()
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

            Built = true;

            if (!building.Selected) building.constructor.constructionLine.enabled = false;
            if (GetComponent<NavMeshObstacle>() != null) GetComponent<NavMeshObstacle>().carving = true;

            OnBuilt?.Invoke();
        }
    }
}