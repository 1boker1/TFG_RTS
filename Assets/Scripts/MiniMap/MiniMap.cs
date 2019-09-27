using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.MiniMap
{
    public class MiniMap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public BoxCollider2D miniMapCollider;
        public BoxCollider mapCollider;

        public bool inside = false;

        public Camera minimapCamera;
        public Camera mainCamera;

        private Vector3 destination;

        private Vector3 currentVelocity;

        public float smoothness = 1;

        private void Start()
        {
            mainCamera = Camera.main;
            destination = mainCamera.transform.position;
        }

        private void Update()
        {
            if (!inside) return;

            if (Input.GetMouseButton(0)) CalculateSquare();
            if (Input.GetMouseButton(1)) MoveUnits();

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

            finalCameraPosition -= (CalculateMaxViewRange() - finalCameraPosition) / 2;

            var yCorrection = new Vector3(0, -finalCameraPosition.y + mainCamera.transform.position.y, 0);

            destination = finalCameraPosition + yCorrection;
        }

        private void MoveUnits()
        {
            if (SelectionManager.SelectedUnits.Count <= 0) return;

            Vector2 mousePosition = Input.mousePosition;

            var miniMapVector = (mousePosition - new Vector2(miniMapCollider.bounds.min.x, miniMapCollider.bounds.min.y));

            var xOffset = miniMapVector.x / (miniMapCollider.bounds.max.x - miniMapCollider.bounds.min.x);
            var yOffset = miniMapVector.y / (miniMapCollider.bounds.max.y - miniMapCollider.bounds.min.y);

            xOffset *= mapCollider.gameObject.transform.localScale.x;
            yOffset *= mapCollider.gameObject.transform.localScale.z;

            var unitGroup = new Vector3(mapCollider.bounds.min.x + xOffset, 0, mapCollider.bounds.min.z + yOffset);

            SelectionManager.Group(unitGroup, SelectionManager.SelectedUnits);
        }

        private Vector3 CalculateMaxViewRange()
        {
            var ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height, 0));

            var worldPoint = Vector3.zero;

            if (Physics.Raycast(ray, out var hit, 9000, LayerMask.GetMask("MiniMap")))
            {
                if (hit.transform != null)
                {
                    worldPoint = hit.point - new Vector3(0, hit.point.y, 0);
                }
            }

            return worldPoint;
        }
    }
}
