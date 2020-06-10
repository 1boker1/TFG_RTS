using Assets.Scripts.Building;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class ShowStoneEfficiency : MonoBehaviour, IRangeBuildable
    {
        private BuildingPreview buildingPreview;

        private StoneEfficiency stoneEfficiency;

        public GameObject linePrefab;
        private GameObject line;

        private void Start()
        {
            buildingPreview = GetComponent<BuildingPreview>();

            stoneEfficiency = StoneEfficiency.instance;
            stoneEfficiency.efficiencyText.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 50, 0);
            stoneEfficiency.efficiencyText.gameObject.SetActive(true);
            stoneEfficiency.SetCirclesState(true);

            line = Instantiate(linePrefab, position: transform.position, rotation: Quaternion.identity);
        }

        private void FixedUpdate()
        {
            stoneEfficiency.lastEfficiency = StoneEfficiency.instance.GetEfficiencyInPosition(transform.position);

            stoneEfficiency.efficiencyText.text = StoneEfficiency.instance.lastEfficiency.ToString() + "%";
            stoneEfficiency.efficiencyText.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 50, 0);
        }

        private void Update()
        {
            line.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        }

        private void OnDestroy()
        {
            Destroy(line.gameObject);
            stoneEfficiency.SetCirclesState(false);
            stoneEfficiency.efficiencyText.gameObject.SetActive(false);
        }

        public bool CanBuild()
        {
            bool canBuild = true;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 47f);

            foreach (Collider coll in hitColliders)
            {
                if (coll.GetComponent<Quarry>() != null) canBuild = false;
            }

            return canBuild;
        }
    }
}
