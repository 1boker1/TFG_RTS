using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Interfaces;
using Assets.Scripts.StateMachine.States;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Managers
{
    public class SelectionManager : MonoBehaviour
    {
		public static SelectionManager Instance;
        public static Rect SelectionBox = new Rect(0, 0, 0, 0);

        public static List<Unit.Unit> SelectedUnits = new List<Unit.Unit>();

        public Vector3Event OnRightClick;

        private static Vector3 forwardVector;
        private static Vector3 startClick = -Vector3.one;

        private const int MaxSelection = 20;

        public int m_Team = 1;

        public static bool isPlacingBuild = false;

        public static BindingList<ISelectable> SelectedEntities = new BindingList<ISelectable>();

        public delegate void ProcessSelectableDelegate(ISelectable target);
        public static event ProcessSelectableDelegate OnTarget;

        public float OffsetX = -10;
        public float OffsetY = 2;

        [SerializeField] private Texture2D defaultCursor;
        [SerializeField] private Texture2D attackCursor;
        [SerializeField] private Texture2D gatherCursor;
        [SerializeField] private Texture2D buildCursor;

		private void Awake()
		{
			if(Instance==null)
				Instance=this;
			if(Instance!=this)
				Destroy(gameObject);

			SelectionBox = new Rect(0, 0, 0, 0);
			SelectedUnits = null;
			SelectedEntities = null;	
			SelectedEntities = new BindingList<ISelectable>();
			SelectedUnits = new List<Unit.Unit>();
		}

		private void Start()
		{
			
		}

		private void Update()
        {
            if (isPlacingBuild) return;

            MouseInput();
            ChangeCursor();
        }

        private void ChangeCursor()
        {
            if (SelectedUnits.Count == 0)
            {
                Cursor.SetCursor(defaultCursor, new Vector2(OffsetX, OffsetY), CursorMode.Auto);
                return;
            }

            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity))
                return;

            var target = hit.transform.GetComponent<ISelectable>();

            Texture2D cursorTexture = defaultCursor;

            if (target != null)
            {
                Type actionType = target.GetActionWithoutTarget(SelectedUnits[0], SelectedUnits[0].Team);

                if (SelectedUnits[0].HasState(actionType))
                {
                    if (actionType == typeof(AttackState))
                    {
                        cursorTexture = attackCursor;
                    }
                    else if (actionType == typeof(BuildState))
                    {
                        cursorTexture = buildCursor;
                    }
                    else if (actionType == typeof(GatherState))
                    {
                        cursorTexture = gatherCursor;
                    }
                }
            }

            Cursor.SetCursor(cursorTexture, new Vector2(OffsetX, OffsetY), CursorMode.Auto);
        }

        private void MouseInput()
        {
            if (Input.GetMouseButtonDown(0) && !Utils.IsPointerOverUIObject() && !isPlacingBuild) ClickSelect();
            if (Input.GetMouseButtonUp(0) && !Utils.IsPointerOverUIObject() && !isPlacingBuild) DragMouseSelect();

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
                if (SelectedUnits[0].Team == m_Team)
                    OnRightClick?.Invoke(hit.point + new Vector3(0, 0.25f, 0));

                Group(hit.point.With(y: 0), SelectedUnits);
            }
        }

        private void ClickSelect()
        {
            DeselectAll();

            startClick = Input.mousePosition;

            var selectedEntity = Utils.GetFromRay<ISelectable>();

            selectedEntity?.Select(m_Team);
        }

        private void DragMouseSelect()
        {
            if (startClick == -Vector3.one || Utils.IsPointerOverUIObject()) return;

            SelectionBox = Utils.DragRectangle(startClick);

            foreach (var unit in Unit.Unit.AllUnits.Where(unit => unit.Team == m_Team))
                Contained(unit);
        }

        private void MaxUnitsSelected()
        {
            if (SelectedUnits.Count <= MaxSelection || Input.GetMouseButton(0)) return;

            for (var i = MaxSelection; i < SelectedUnits.Count; i++) SelectedUnits[i].Deselect();
        }

        public void DeselectAll()
        {
            var entitiesDuplicate = new ISelectable[SelectedEntities.Count];

            SelectedEntities.CopyTo(entitiesDuplicate, 0);

            foreach (var entity in entitiesDuplicate) entity.Deselect();
        }

        private void Contained(Unit.Unit unit)
        {
            var cameraPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

            cameraPosition.y = MathExtension.InvertMouseY(cameraPosition.y);

            if (!(SelectionBox.size.magnitude > new Vector2(5, 5).magnitude)) return;

            if (SelectionBox.Contains(cameraPosition, true)) unit.Select(unit.Team);
            else unit.Deselect();
        }

        public void Group(Vector3 worldPoint, List<Unit.Unit> selectedUnits)
        {
            foreach (var unit in selectedUnits)
            {
                if (unit.Team == m_Team)
                {
                    unit.MoveToPosition(worldPoint);
                }
            }
        }

		private void OnDestroy()
		{
			
		}
	}
}