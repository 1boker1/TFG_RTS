using UnityEngine;

namespace Assets.Scripts.Building
{
    public class LineConstructor
    {
        private BoxCollider boxCollider;
        private LineRenderer constructionLine;

        public LineConstructor(LineRenderer constructionLine, BoxCollider collider)
        {
            this.constructionLine = constructionLine;
            boxCollider = collider;
        }

        public void SetUpLine()
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

            for (int i = 0; i < positions.Length; i++)
            {
                constructionLine.SetPosition(i, positions[i]);
            }

            constructionLine.loop = true;

            PositionLine();

            constructionLine.enabled = true;
        }

        public void PositionLine()
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