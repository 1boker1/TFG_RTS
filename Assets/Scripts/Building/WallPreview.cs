using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Building
{
    public class WallPreview : MonoBehaviour
    {
        public static WallPreview instance;

        [SerializeField] private GameObject PillarPrefab;
        [SerializeField] private GameObject WallPrefab;

        [SerializeField] private GameObject DummyPillar;
        [SerializeField] private GameObject DummyWall;

        [SerializeField] private float WallOffsetMultiplier = 0.95f;

        public List<WallPiecePreview> Pillars = new List<WallPiecePreview>();
        public List<WallPiecePreview> Walls = new List<WallPiecePreview>();

        private WallPiecePreview firstPillar;
        private WallPiecePreview secondPillar;
        private WallPiecePreview currentPillar;

        public static GameObject builtPillar;

        private float wallSize;
        private float wallAlphaX;

        public bool RayOnMountains = false;

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(instance.gameObject);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            SelectionManager.isPlacingBuild = true;

            SetUpBuilding();
        }

        private void OnDisable()
        {
            SelectionManager.isPlacingBuild = false;

            Cancel();
        }

        private void Update()
        {
            PositionatePillar();

            if (firstPillar != null && currentPillar != null)
                CreateWallsPreview();

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                OnPillarCreation();

            if (Input.GetMouseButtonDown(1))
                Cancel();
        }

        private void PositionatePillar()
        {
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 999f, LayerMask.GetMask("Floor")))
            //{
            //    RayOnMountains = hit.point.y > 0.5f;
			//
            //    currentPillar.transform.position = hit.point.With(y: 0.01f);
            //}

			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 999f, LayerMask.GetMask("Floor")))
            {
                currentPillar.IsOnGround = Mathf.Abs(hit.point.y) < 0.5f;
                currentPillar.transform.position = hit.point.With(y: 0.01f);
				currentPillar.SetBuildState();
            }
        }

        private void CreateWallsPreview()
        {
            float pillarOffset = 0;
            float distance = 0;

            pillarOffset = firstPillar.GetComponent<CapsuleCollider>().radius;

            Vector3 _Direction = (currentPillar.transform.position - firstPillar.transform.position).normalized;
            Vector3 _FirstPoint = firstPillar.transform.position + _Direction * pillarOffset;
            Vector3 _SecondPoint = currentPillar.transform.position - _Direction * pillarOffset;

            distance = Vector3.Distance(_FirstPoint, _SecondPoint);

            int AmountOfWalls = Mathf.CeilToInt(distance / wallSize);
            wallAlphaX = distance / (AmountOfWalls * wallSize);

            //Debug.Log("Distance: " + distance + " WallSize: " + wallSize + " WallAmount: " + AmountOfWalls + " WallPercentage: " + wallAlphaX);

            var WallsCopy = new WallPiecePreview[Walls.Count];

            Walls.CopyTo(WallsCopy);

            if (Walls.Count != 0 && AmountOfWalls < Walls.Count)
            {
                for (int i = AmountOfWalls; i < WallsCopy.Length; i++)
                {
                    if (i >= Walls.Count)
                        break;

                    Destroy(Walls[i].gameObject);
                    Walls.RemoveAt(i);
                }
            }
            else
            {
                for (int i = Walls.Count; i < AmountOfWalls; i++)
                {
                    GameObject wall = Instantiate(DummyWall, transform.position, Quaternion.identity);
                    Walls.Add(wall.GetComponent<WallPiecePreview>());
                }
            }

            PositionateWalls(_FirstPoint, _Direction, wallAlphaX);
        }

        private void PositionateWalls(Vector3 StartingPoint, Vector3 Direction, float sizePercentage)
        {

            for (int i = 0; i < Walls.Count; i++)
            {
                Vector3 _Position = StartingPoint + (Direction * (i + 0.5f) * (wallSize * sizePercentage));

                Walls[i].GetComponent<MeshRenderer>().material.SetFloat("_PercentageX", sizePercentage);
                Walls[i].transform.position = _Position;
                Walls[i].transform.right = Direction;
            }
        }

        private void OnPillarCreation()
        {
            if (!currentPillar.CanBeBuilt)
                return;

            if (firstPillar == null)
                firstPillar = currentPillar;
            else
                secondPillar = currentPillar;

            if (secondPillar == null)
            {
                SetUpBuilding();
            }
            else
            {
                BuildWall();
            }
        }

        private void BuildWall()
        {
            var PillarsCopy = new WallPiecePreview[Pillars.Count];
            Pillars.CopyTo(PillarsCopy);

            GameObject firstObject = null;

            for (int i = 0; i < PillarsCopy.Length; i++)
            {
                if (PillarsCopy[i].CanBeBuilt)
                {
                    if (firstObject == null)
                        firstObject = Instantiate(PillarPrefab, PillarsCopy[i].transform.position, PillarsCopy[i].transform.rotation);
                    else
                        builtPillar = Instantiate(PillarPrefab, PillarsCopy[i].transform.position, PillarsCopy[i].transform.rotation);
                }

                if (i == PillarsCopy.Length - 1)
                    continue;

                Destroy(Pillars[i].gameObject);
            }

            var WallsCopy = new WallPiecePreview[Walls.Count];
            Walls.CopyTo(WallsCopy);

            for (int i = 0; i < WallsCopy.Length; i++)
            {
                if (WallsCopy[i].CanBeBuilt)
                {
                    var wall = Instantiate(WallPrefab, WallsCopy[i].transform.position, WallsCopy[i].transform.rotation);
                    wall.GetComponent<MeshRenderer>().material.SetFloat("_PercentageX", wallAlphaX);
                    wall.GetComponent<BoxCollider>().size = wall.GetComponent<NavMeshObstacle>().size * WallOffsetMultiplier;
                }

                Destroy(Walls[i].gameObject);
            }

            Building firstBuilding = firstObject.GetComponent<Building>();

            if (firstBuilding != null)
            {
                firstBuilding.GetBuildingHealthSystem().Built = false;
                firstBuilding.Team = SelectionManager.Instance.m_Team;
                firstBuilding.GetBuildingData().team = SelectionManager.Instance.m_Team;

                foreach (var unit in SelectionManager.SelectedUnits.Where(unit => unit.HasState(typeof(BuildState))))
                {
                    unit.OnTarget(firstBuilding);
                    break;
                }
            }

            WallPiecePreview _FirstPillar = secondPillar;

            Clear();

            firstPillar = _FirstPillar;
            Pillars.Add(firstPillar);
            SetUpBuilding();

            //gameObject.SetActive(false);
        }

        public void Cancel()
        {
            SelectionManager.isPlacingBuild = false;

            transform.position = new Vector3(0, 999, 0);

            if (currentPillar)
            {
                Destroy(currentPillar.gameObject);
                currentPillar = null;
            }

            if (firstPillar)
            {
                Destroy(firstPillar.gameObject);
                firstPillar = null;
            }

            secondPillar = null;

            foreach (var wall in Walls)
                Destroy(wall.gameObject);

            Walls.Clear();
            Pillars.Clear();

            gameObject.SetActive(false);
        }

        public void Clear()
        {
            currentPillar = null;
            firstPillar = null;
            secondPillar = null;

            Walls.Clear();
            Pillars.Clear();
        }

        public void SetUpBuilding()
        {
            wallSize = WallPrefab.GetComponent<MeshRenderer>().bounds.size.x * WallOffsetMultiplier;

            currentPillar = Instantiate(DummyPillar, transform.position, Quaternion.identity).GetComponent<WallPiecePreview>();

            Pillars.Add(currentPillar);
        }
    }
}
