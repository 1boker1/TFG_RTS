using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Building
{
    public class SpawnerBuilding : MonoBehaviour, ISpawnData
    {
        public GameObject destinationMarker;
        public GameObject defaultSpawnPoint;

        private float timer = 0;

        public List<UnitData> queue = new List<UnitData>();

        private Building building;

        private GameObject unit;
        private GameObject target = null;

        public UnitData[] unitsToSpawn = new UnitData[6];

        [SerializeField] private int queueCapacity = 6;

        [SerializeField] private List<GameObject> availableEntities;
        public List<GameObject> AvailableEntities { get => availableEntities; set => availableEntities = value; }

        public float SpawnPercentage { get; private set; }

        private void Start()
        {
            building = GetComponent<Building>();
            destinationMarker.transform.position = new Vector3(999, 999, 999);
        }

        private void Update()
        {
            if (!building.Built) return;

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
            destinationMarker.transform.position = position;
            destinationMarker.SetActive(active);
            destinationMarker.GetComponent<PathRenderer>().SetPositions();
        }

        private void GoToSpawnPoint(Unit.Unit unit)
        {
            if (!target) unit.MoveToPosition(destinationMarker.transform.position);
            else unit.OnTarget(target.GetComponent<ISelectable>());
        }

        private void SpawnQueue()
        {
            if (queue.Count <= 0) { timer = 0; return; }

            timer += Time.deltaTime;

            float timeToSpawn = queue[0].GetComponent<UnitData>().timeToSpawn;

            if (timer >= timeToSpawn)
            {
                Spawn(queue[0]);

                queue.Remove(queue[0]);

                timer = 0;
            }

            SpawnPercentage = timer / timeToSpawn;
        }

        public void Spawn(UnitData unit)
        {
            unit = Instantiate(unit);

            unit.team = building.Team;
            unit.transform.position = defaultSpawnPoint.transform.position;
            unit.transform.LookAt(unit.transform.position + new Vector3(-10, 0, 0));

            GoToSpawnPoint(unit.GetComponent<Unit.Unit>());
        }

        public void AddToQueue(UnitData unit)
        {
            if (unit == null) return;

            if (queue.Count < queueCapacity)
            {
                if (Utils.HaveEnoughResources(unit.WoodCost, unit.FoodCost, unit.GoldCost, unit.RockCost)) queue.Add(unit);
            }
            else
            {
                Debug.Log("Unit Queue is full");
            }
        }
    }
}
