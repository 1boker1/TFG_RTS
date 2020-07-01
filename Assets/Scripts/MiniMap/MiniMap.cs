using Assets.Scripts.Managers;
using Assets.Scripts.ProceduralGeneration;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.MiniMap
{
    public class MiniMap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool DebugScene = true;

        public BoxCollider2D miniMapCollider;
        public bool inside = false;

        public Camera minimapCamera;
        public Camera mainCamera;

        private Vector3 destination;

        private Vector3 currentVelocity;

        public float smoothness = 1;

        private MeshGenerator mapInfo;
        private float MapSize;

        public LayerMask MinimapLayer;
        public LayerMask EverythingLayer;

        public RenderTexture backgroundTexture;
        public RenderTexture foregroundTexture;

        private void Start()
        {
            if (DebugScene)
                SetUp();

            StartCoroutine(DelayedRendering());
        }
		public void SetUp()
        {
            mapInfo = FindObjectOfType<MeshGenerator>();
            mainCamera = Camera.main;
            destination = mainCamera.transform.position;

            MapSize = mapInfo.MapDepth * mapInfo.VertexDistance * 0.5f;
            minimapCamera.orthographicSize = MapSize;
            minimapCamera.transform.position = new Vector3(MapSize, 200f, MapSize);

            minimapCamera.targetTexture = backgroundTexture;
            minimapCamera.cullingMask = EverythingLayer;
            minimapCamera.Render();
            minimapCamera.targetTexture = foregroundTexture;
            minimapCamera.cullingMask = MinimapLayer;

            minimapCamera.enabled = false;
        }

        public IEnumerator DelayedRendering()
        {
            while (true)
            {		
                minimapCamera.Render();
                yield return new WaitForSeconds(0.04166667f);
            }
        }

        private void Update()
        {
            if (!inside) return;

            if (Input.GetMouseButton(0)) CalculateSquare();

            MoveCamera();
        }

        private void MoveCamera()
        {
            if (destination == mainCamera.transform.position) return;

            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, destination, ref currentVelocity, smoothness);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            currentVelocity = Vector3.zero;
            destination = mainCamera.transform.position;
            inside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            destination = mainCamera.transform.position;
            inside = false;
        }

        private void CalculateSquare()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 vectorFromCenter = mousePosition - (Vector2)miniMapCollider.bounds.center;

            Vector2 maxBounds = miniMapCollider.bounds.max;
            Vector2 minBounds = miniMapCollider.bounds.min;

            var maxViewport = minimapCamera.ViewportToWorldPoint(Vector2.one);
            var minViewport = minimapCamera.ViewportToWorldPoint(Vector2.zero);

            vectorFromCenter *= (maxViewport - minViewport).magnitude / (maxBounds - minBounds).magnitude;

            var finalCameraPosition = minimapCamera.transform.position + new Vector3(vectorFromCenter.x, 0, vectorFromCenter.y);

            finalCameraPosition -= CalculateMaxViewRange();

            var yCorrection = new Vector3(0, -finalCameraPosition.y + mainCamera.transform.position.y, 0);

            destination = finalCameraPosition + yCorrection;
        }

        private Vector3 CalculateMaxViewRange()
        {
            float distance = mainCamera.transform.position.y / Mathf.Sin(90 - mainCamera.transform.rotation.eulerAngles.x);

            return mainCamera.transform.forward * distance;
        }
    }
}
