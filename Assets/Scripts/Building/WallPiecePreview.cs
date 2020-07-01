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
        public bool IsOnGround{ get; set; }
        public bool IsColliding{ get; set; }

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
			IsColliding=true;
            SetBuildState();
        }

		private void FixedUpdate()
		{
			if(!IsPillar)
			{
				Ray ray = new Ray(transform.position+Vector3.up*50, Vector3.down);

				if (Physics.Raycast(ray, out var hit, 999f, LayerMask.GetMask("Floor")))
				{
				    IsOnGround = Mathf.Abs(hit.point.y) < 0.5f;
				}

				SetBuildState();
			}
		}

		private void OnTriggerExit(Collider other)
        {
            if (IsTriggerException(other))
                return;
			IsColliding=false;
            SetBuildState();
        }

        public void SetBuildState()
        {
            CanBeBuilt = !IsColliding && IsOnGround;
            meshRenderer.material.SetColor("_Color", CanBeBuilt ? new Color(1, 1, 1, 1) : new Color(1, 0, 0, 0.5f));
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