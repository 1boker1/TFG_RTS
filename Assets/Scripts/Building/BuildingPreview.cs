using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Building
{
    public class BuildingPreview : MonoBehaviour
    {
        private static BuildingPreview instance;

        [SerializeField] private bool isColliding;
        private bool canBuild;

        private int layerMask;

        private GameObject insideLight = null;

        public GameObject targetBuild;

        public LineRenderer constructionLine;

        public int woodCost;
        public int foodCost;
        public int goldCost;

        public Color greenMarker;
        public Color redMarker;

        public Unit.Unit Worker;

        #region Refactor

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public BoxCollider boxCollider;
        public Material mainMaterial;

        #endregion

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();

            layerMask = LayerMask.GetMask("Floor");

            if (instance == null) instance = this;
            else Destroy(instance.gameObject);
        }

        private void Start()
        {
            Positioning();

            if (constructionLine == null) constructionLine = gameObject.AddComponent<LineRenderer>();

            LineSetup();

            SelectionManager.isPlacingBuild = true;

            CanBuild(true);
        }

        private void Update()
        {
            Positioning();

            if (insideLight == null) CanBuild(false);

            if (Input.GetMouseButtonUp(0) && canBuild)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    ResourceManager.Instance.AddResource(typeof(Wood), -woodCost);
                    ResourceManager.Instance.AddResource(typeof(Food), -foodCost);
                    ResourceManager.Instance.AddResource(typeof(Gold), -goldCost);

                    Build();
                }
            }

            if (Input.GetMouseButtonDown(1)) Cancel();

            if (Worker != null && Worker.Selected == false) Worker.Selected = true;
        }

        private void Positioning()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 999f, layerMask))
            {
                transform.position = hit.point;

                if (constructionLine != null)
                    LinePosition();

                transform.position = new Vector3(transform.position.x, 0.01f, transform.position.z);

                CanBuild(!isColliding);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.Rotate(0, 45, 0);
            }
        }

        private void Build()
        {
            Building building = Instantiate(targetBuild, transform.position.With(y: 0.5f), transform.rotation).GetComponent<Building>();

            building.Team = SelectionManager.team;
            building.buildingData.team = SelectionManager.team;

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

            Destroy(gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.tag != "Floor" && other.transform.tag != "MainCamera")
            {
                if (!other.isTrigger)
                {
                    isColliding = true;
                    ChangeMaterialColor(redMarker);
                }

                if (other.gameObject.CompareTag("Torch"))
                {
                    insideLight = other.gameObject;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isColliding = false;
            ChangeMaterialColor(greenMarker);

            if (other.gameObject.CompareTag("Torch")) insideLight = null;
        }

        public void CanBuild(bool state)
        {
            canBuild = state;

            if (constructionLine != null) ChangeMaterialColor(canBuild ? greenMarker : redMarker);
        }

        private void ChangeMaterialColor(Color color)
        {
            foreach (var material in meshRenderer.materials)
            {
                material.color = color;
            }

            constructionLine.material.color = color;
        }

        public void ChangeMaterial(Material material)
        {
            for (var index = 0; index < meshRenderer.materials.Length; index++)
            {
                meshRenderer.materials[index] = material;
            }
        }

        private void LineSetup()
        {
            constructionLine.enabled = false;

            Vector3[] positions = new Vector3[4];

            positions[0] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.min.z);
            positions[1] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.max.z);
            positions[2] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.max.z);
            positions[3] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.min.z);

            constructionLine.positionCount = positions.Length;
            constructionLine.startWidth = 0.1f;
            constructionLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            constructionLine.receiveShadows = false;

            constructionLine.material = mainMaterial;

            for (int i = 0; i < positions.Length; i++)
            {
                constructionLine.SetPosition(i, positions[i]);
            }

            constructionLine.loop = true;

            LinePosition();

            constructionLine.enabled = true;
        }

        private void LinePosition()
        {
            Vector3[] positions = new Vector3[4];

            positions[0] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.min.z);
            positions[1] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.max.z);
            positions[2] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.max.z);
            positions[3] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.min.z);

            for (int i = 0; i < positions.Length; i++)
            {
                constructionLine.SetPosition(i, positions[i]);
            }
        }
    }
}
