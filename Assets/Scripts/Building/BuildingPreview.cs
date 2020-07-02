using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Building
{
    public class BuildingPreview : MonoBehaviour
    {
        public static BuildingPreview instance;

        [SerializeField] private bool isColliding;
        [SerializeField] private bool isOnGround;
        [SerializeField] private bool canBuild;

        private int layerMask;

        public GameObject targetBuild;

        public int woodCost;
        public int foodCost;
        public int goldCost;
        public int rockcost;

        public Unit.Unit Worker;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public BoxCollider boxCollider;
        public Material redMaterial;
        private Material mainMaterial;

        private void Awake()
        {
            layerMask = LayerMask.GetMask("Floor");

            if (instance == null) instance = this;
            else Destroy(instance.gameObject);

            gameObject.SetActive(false);
        }

        private void Start()
        {
            SelectionManager.isPlacingBuild = true;

            CanBuild();
        }

        private void Update()
        {
            Positioning();

            if (Input.GetMouseButtonUp(0) && canBuild)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    ResourceManager.Instance.AddResource(typeof(Wood), -woodCost);
                    ResourceManager.Instance.AddResource(typeof(Food), -foodCost);
                    ResourceManager.Instance.AddResource(typeof(Gold), -goldCost);
                    ResourceManager.Instance.AddResource(typeof(Rock), -rockcost);

                    Build();
                }
            }

            if (Input.GetMouseButtonDown(1))
                Cancel();

            if (Worker != null && Worker.Selected == false)
                Worker.Selected = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.tag == "Floor" || other.transform.tag == "MainCamera" || other.isTrigger) return;

            isColliding = true;
            CanBuild();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Floor" || other.transform.tag == "MainCamera" || other.isTrigger) return;

            isColliding = false;
            CanBuild();
        }

        private void Positioning()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 999f, layerMask))
            {
                isOnGround = Mathf.Abs(hit.point.y) < 0.5f;
                CanBuild();

                transform.position = hit.point.With(y: 0.01f);
            }

            if (Input.GetKeyDown(KeyCode.R))
                transform.Rotate(0, 45, 0);
        }

        private void Build()
        {
            Building building = Instantiate(targetBuild, transform.position, transform.rotation).GetComponent<Building>();

            building.GetBuildingHealthSystem().Built = false;
            building.Team = SelectionManager.Instance.m_Team;
            building.GetBuildingData().team = SelectionManager.Instance.m_Team;

            foreach (var unit in SelectionManager.SelectedUnits.Where(unit => unit.HasState(typeof(BuildState))))
            {
                unit.OnTarget(building);
                break;
            }

            Cancel();
        }

        private void Cancel()
        {
            SelectionManager.isPlacingBuild = false;

            ResetValues();
        }

        private void ChangeMaterialColor(bool CanBuild)
        {
            ChangeMaterial(CanBuild ? mainMaterial : redMaterial);
        }

        public void CanBuild()
        {
            canBuild = isOnGround && !isColliding;
            ChangeMaterialColor(canBuild);
        }

        public void ChangeMaterial(Material material)
        {
            meshRenderer.material = material;
            meshRenderer.material.enableInstancing = true;
            meshRenderer.material.SetFloat("_Percentage", 1);
        }

        public void SetUpBuilding(GameObject Target)
        {
            if (Target == null)
                return;

            if (Target.GetComponent<BuildingData>().IsWallPillar)
            {
                WallPreview.instance.gameObject.SetActive(true);
                return;
            }

            targetBuild = Target;
            transform.localScale = targetBuild.transform.localScale;

            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();

            meshFilter.mesh = targetBuild.GetComponent<MeshFilter>().sharedMesh;
            meshRenderer.material = targetBuild.GetComponent<MeshRenderer>().sharedMaterial;
            mainMaterial = targetBuild.GetComponent<MeshRenderer>().sharedMaterial;

            boxCollider.center = targetBuild.GetComponent<BoxCollider>().center;
            boxCollider.size = targetBuild.GetComponent<BoxCollider>().size;

			woodCost=(int)Target.GetComponent<BuildingData>().WoodCost.amount;
			foodCost=(int)Target.GetComponent<BuildingData>().FoodCost.amount;
			goldCost=(int)Target.GetComponent<BuildingData>().GoldCost.amount;
			rockcost=(int)Target.GetComponent<BuildingData>().RockCost.amount;

			isColliding = false;
            isOnGround = true;

			SelectionManager.isPlacingBuild=true;

            gameObject.SetActive(true);
        }

        public void ResetValues()
        {
            gameObject.SetActive(false);
            WallPreview.instance.Cancel();
            transform.position = new Vector3(0, 999, 0);
            targetBuild = null;
        }
    }
}
