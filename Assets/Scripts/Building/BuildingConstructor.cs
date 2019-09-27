using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Building
{
    public class BuildingConstructor
    {
        private readonly BoxCollider boxCollider;

        private readonly float colliderY;
        private readonly float colliderSizeY;

        public LineRenderer constructionLine;
        public Material lineMaterial;

        private readonly Building building;

        public BuildingConstructor(Building building)
        {
            this.building = building;
            boxCollider = building.GetComponent<BoxCollider>();

            constructionLine = building.GetComponent<LineRenderer>();

            if (constructionLine == null) constructionLine = building.gameObject.AddComponent<LineRenderer>();

            colliderY = boxCollider.center.y;
            colliderSizeY = boxCollider.size.y;

            LineSetup();
        }

        public void LineSetup()
        {
            Vector3[] positions = new Vector3[4];

            positions[0] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.min.z);
            positions[1] = new Vector3(boxCollider.bounds.min.x, 1, boxCollider.bounds.max.z);
            positions[2] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.max.z);
            positions[3] = new Vector3(boxCollider.bounds.max.x, 1, boxCollider.bounds.min.z);

            constructionLine.enabled = false;
            constructionLine.positionCount = positions.Length;
            constructionLine.startWidth = 0.1f;
            constructionLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            constructionLine.receiveShadows = false;

            constructionLine.material = lineMaterial == null ? new Material(Shader.Find("Diffuse")) : lineMaterial;

            for (int i = 0; i < positions.Length; i++)
            {
                constructionLine.SetPosition(i, positions[i]);
            }

            constructionLine.loop = true;
        }

        public void ConstructionAnimation()
        {
            var healthSystem = building.GetComponent<BuildingHealthSystem>();

            var yAmount = colliderSizeY * (1 - ((float)healthSystem.HealthPoints / (float)healthSystem.MaxHealthPoints));

            var center = new Vector3(boxCollider.center.x, colliderY + yAmount, boxCollider.center.z);

            boxCollider.center = center;
            building.GetComponent<NavMeshObstacle>().center = center;

            building.transform.position = building.transform.position.With(y: building.transform.localScale.y * -yAmount);
        }
    }
}