using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        public static Rect SelectionBox = new Rect(0, 0, 0, 0);

        public static List<Unit.Unit> SelectedUnits = new List<Unit.Unit>();

        public Vector3Event OnRightClick;

        private static Vector3 forwardVector;
        private static Vector3 startClick = -Vector3.one;

        private const int MaxSelection = 50;

        public static int team = 1;

        public static bool isPlacingBuild = false;

        public static BindingList<ISelectable> SelectedEntities = new BindingList<ISelectable>();

        public delegate void ProcessSelectableDelegate(ISelectable target);
        public static event ProcessSelectableDelegate OnTarget;

        private void Update()
        {
            if (isPlacingBuild) return;

            MouseInput();
        }

        private void MouseInput()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) ClickSelect();
            if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject()) DragMouseSelect();

            MaxUnitsSelected();

            if (EventSystem.current.IsPointerOverGameObject() || SelectedUnits.Count <= 0) return;

            if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0)) Target();
        }

        private void Target()
        {
            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity)) return;

            var target = hit.transform.GetComponent<ISelectable>();

            if (target != null)
            {
                StartCoroutine(target.Highlight());

                OnTarget?.Invoke(target);
            }
            else
            {
                OnRightClick?.Invoke(hit.point);

                Group(hit.point, SelectedUnits);
            }
        }

        private static void ClickSelect()
        {
            DeselectAll();

            startClick = Input.mousePosition;

            var selectedEntity = Utils.GetFromRay<ISelectable>();

            selectedEntity?.Select(team);
        }

        private static void DragMouseSelect()
        {
            if (startClick == -Vector3.one) return;

            SelectionBox = Utils.DragRectangle(startClick);

            foreach (var unit in Unit.Unit.AllUnits.Where(unit => unit.Team == team)) Contained(unit);
        }

        private static void MaxUnitsSelected()
        {
            if (SelectedUnits.Count <= MaxSelection || Input.GetMouseButton(0)) return;

            for (var i = MaxSelection; i < SelectedUnits.Count; i++) SelectedUnits[i].Deselect();
        }

        public static void DeselectAll()
        {
            var entitiesDuplicate = new ISelectable[SelectedEntities.Count];

            SelectedEntities.CopyTo(entitiesDuplicate, 0);

            foreach (var entity in entitiesDuplicate) entity.Deselect();
        }

        public static void Group(Vector3 worldPoint, List<Unit.Unit> selectedUnits)
        {
            foreach (var unit in selectedUnits) { unit.MoveToPosition(worldPoint); }
        }

        public static void Contained(Unit.Unit unit)
        {
            var cameraPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

            cameraPosition.y = MathExtension.InvertMouseY(cameraPosition.y);

            if (!(SelectionBox.size.magnitude > new Vector2(5, 5).magnitude)) return;

            if (SelectionBox.Contains(cameraPosition, true)) unit.Select(unit.Team);
            else unit.Deselect();
        }
    }
}