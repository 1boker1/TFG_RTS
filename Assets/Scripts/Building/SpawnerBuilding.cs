using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Assets.Scripts.Building
{
    public class SpawnerBuilding : MonoBehaviour
    {
        public GameObject destinationMarker;

        private float timer = 0;

        public List<UnitData> queue = new List<UnitData>();

        public Building building;

        private GameObject unit;
        private GameObject target = null;

        public UnitData[] unitsToSpawn = new UnitData[6];

        [SerializeField] private int queueCapacity = 6;

        public float SpawnPercentage { get; private set; }

        public delegate void OnUnitSpawn();
        public OnUnitSpawn OnUnitSpawned;

        private void Start()
        {
            building = GetComponent<Building>();
        }

        private void Update()
        {
            if (!building.GetBuildingHealthSystem().Built) return;

            if (Input.GetMouseButtonDown(1) && building.Selected) CalculateSpawnPoint();

            destinationMarker.SetActive(building.Selected);

            SpawnQueue();
        }

        private void CalculateSpawnPoint()
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit)) return;

            if (hit.transform.CompareTag("Floor"))
            {
                target = null;
                SetSpawnPoint(hit.point, true);
            }
            else if (hit.transform.gameObject == gameObject)
            {
                target = null;
                SetSpawnPoint(Vector3.zero, false);
            }
            else if (hit.transform.GetComponent<ISelectable>() != null && !(hit.transform.GetComponent<ISelectable>() is Unit.Unit))
            {
                target = hit.transform.gameObject;
                SetSpawnPoint(hit.transform.position, true);
            }
        }

        private void SetSpawnPoint(Vector3 position, bool active)
        {
            destinationMarker.transform.position = position.With(y: transform.position.y);
            destinationMarker.SetActive(active);
            destinationMarker.GetComponent<PathRenderer>().SetPositions(transform.position);
        }

        private void GoToSpawnPoint(Unit.Unit UnitToCommand)
        {
            UnitToCommand.GetComponent<NavMeshAgent>().Warp(UnitToCommand.transform.position);

            if (!target) UnitToCommand.MoveToPosition(destinationMarker.transform.position.With(y:transform.position.y));
            else UnitToCommand.OnTarget(target.GetComponent<ISelectable>());
        }

        private void SpawnQueue()
        {
            if (queue.Count <= 0) { timer = 0; return; }

            timer += Time.deltaTime;

            float timeToSpawn = queue[0].GetComponent<UnitData>().timeToSpawn;

            if (timer >= timeToSpawn)
            {
                if (PopulationManager.Instance.CanSpawn())
                {
                    Spawn(queue[0]);

                    queue.Remove(queue[0]);

                    timer = 0;

                    OnUnitSpawned?.Invoke();
                }
                else
                {
                    timer = timeToSpawn;
                }
            }

            SpawnPercentage = timer / timeToSpawn;
        }

        public void Spawn(UnitData UnitToSpawn)
        {
            UnitToSpawn = Instantiate(UnitToSpawn);

            UnitToSpawn.team = building.Team;
            UnitToSpawn.transform.position = GetComponent<Collider>().bounds.ClosestPoint(destinationMarker.transform.position);
            UnitToSpawn.transform.LookAt(UnitToSpawn.transform.position + new Vector3(-10, 0, 0));
            UnitToSpawn.GetComponent<Unit.Unit>().Team = building.Team;

            GoToSpawnPoint(UnitToSpawn.GetComponent<Unit.Unit>());
        }

        public void AddToQueue(UnitData UnitToAdd)
        {
            if (UnitToAdd == null || queue.Count >= queueCapacity)
                return;

			if (UnitToAdd.WoodCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Wood)) &&
                UnitToAdd.GoldCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Gold)) &&
                UnitToAdd.RockCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Rock)) &&
                UnitToAdd.FoodCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Food)))
            {
				ResourceManager.Instance.AddResource(typeof(Wood), -UnitToAdd.WoodCost.amount);
				ResourceManager.Instance.AddResource(typeof(Gold), -UnitToAdd.GoldCost.amount);
				ResourceManager.Instance.AddResource(typeof(Rock), -UnitToAdd.RockCost.amount);
				ResourceManager.Instance.AddResource(typeof(Food), -UnitToAdd.FoodCost.amount);
                queue.Add(UnitToAdd);
			}
        }

        public void RemoveFromQueue(int i)
        {
            if (queue.Count <= i)
                return;

            if (i == 0)
                timer = 0;

            queue.RemoveAt(i);
        }
    }
}
