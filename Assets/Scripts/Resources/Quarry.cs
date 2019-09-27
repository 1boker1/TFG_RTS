using System.Collections;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class Quarry : MonoBehaviour
    {
        public float resourcePerSecond = 10;

        public float multiplier = 1f;
        public float efficiency = 1f;

        public Building.Building building;
        public LineRenderer rangeLinePrefab;
        public LineRenderer rangeLine;

        private StoneEfficiency stoneEfficiency;

        private void Start()
        {
            stoneEfficiency = StoneEfficiency.instance;

            stoneEfficiency.quarryList.Add(this);

            stoneEfficiency.radiusCircleList.Add(rangeLine = Instantiate(rangeLinePrefab, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity));

            efficiency = stoneEfficiency.lastEfficiency / 100f;

            StartCoroutine(CollectResources());
        }

        public IEnumerator CollectResources()
        {
            if (building.Built)
                ResourceManager.Instance.AddResource(typeof(Rock), resourcePerSecond * efficiency * multiplier);

            yield return new WaitForSeconds(1);

            StartCoroutine(CollectResources());
        }

        private void OnDestroy()
        {
            stoneEfficiency.quarryList.Remove(this);
            stoneEfficiency.radiusCircleList.Remove(rangeLine);
        }
    }
}
