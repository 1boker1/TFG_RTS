using Assets.Scripts.Unit;
using UnityEngine;

namespace Assets.Scripts.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public State currentState;

		private void Start() 
		{
			currentState?.Enter();
		}

		private void Update() => ExecuteState();

        private void OnGUI() => DisplayState();

        public void ChangeState(State nextState)
        {
            if (currentState != null)
                currentState.Exit();

            currentState = nextState;
            currentState.Enter();
        }

        public void ExecuteState() { if (currentState != null) currentState.Execute(); }

        private void DisplayState()
        {
            if (!Application.isEditor) return;
            if (CheatManager.Instance==null || !CheatManager.Instance.SeeHelpers) return;

            var screenPoint = Camera.main.WorldToScreenPoint(transform.position);

            var textRect = new Rect(new Vector2(screenPoint.x, Screen.height - screenPoint.y), new Vector2(200f, 30f));
            var targetRect = new Rect(new Vector2(screenPoint.x, (Screen.height - screenPoint.y) + 15f), new Vector2(200f, 30f));

            if (currentState != null) GUI.Label(textRect, currentState.GetType().Name);

            var unit = GetComponent<Unit.Unit>();

            if (unit) GUI.Label(targetRect, unit.Target == null ? "Null" : unit.Target.GetType().Name);
        }
    }
}

