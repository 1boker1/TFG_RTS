using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.Building
{
    public class WallPiecePreview : MonoBehaviour
    {
        public Material RedMaterial;

        private MeshRenderer meshRenderer;
        private Material NormalMaterial;

        public bool CanBeBuilt { get; set; }
        public bool IsPillar;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            NormalMaterial = meshRenderer.sharedMaterial;
            CanBeBuilt = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsTriggerException(other))
                return;

            SetBuildState(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsTriggerException(other))
                return;

            SetBuildState(true);
        }

        private void SetBuildState(bool CanBuild)
        {
            CanBeBuilt = CanBuild;
            meshRenderer.material.SetColor("_Color", CanBuild ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 0.5f));
        }

        private bool IsTriggerException(Collider other)
        {
            if (other.transform.tag == "Floor" || other.transform.tag == "MainCamera")
                return true;
            if (other.isTrigger)
                return true;
            if (WallPreview.builtPillar != null)
            {
                if (other.transform.gameObject == WallPreview.builtPillar ||
                    other.bounds.Intersects(WallPreview.builtPillar.GetComponent<Collider>().bounds))
                {
                    return true;
                }
            }
            return false;
        }
    }
}