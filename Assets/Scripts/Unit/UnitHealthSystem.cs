using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.StateMachine;
using Assets.Scripts.StateMachine.States;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Unit
{
    public class UnitHealthSystem : MonoBehaviour, IHealth
    {
        private Unit unit;

        public int MaxHealthPoints { get; set; }
        public int HealthPoints { get; set; }

        private void Start()
        {
            unit = GetComponent<Unit>();

            MaxHealthPoints = unit.unitData.MaxHealthPoints;
            HealthPoints = unit.unitData.MaxHealthPoints;
        }

        public void ModifyHealth(int amount, ISelectable attacker)
        {
            if (HealthPoints <= 0) return;

            if (unit.stateMachine.currentState.GetType() == typeof(IdleState)) unit.OnTarget(attacker);

            HealthPoints -= amount;
 
            if (HealthPoints <= 0)
            {
                HealthPoints = 0;

                OutOufHealth();
            }
        }

        public void OutOufHealth()
        {
            unit.StopAllCoroutines();
            unit.Deselect();

            Unit.AllUnits.Remove(unit);

            if (SelectionManager.SelectedUnits.Contains(unit))
                SelectionManager.SelectedUnits.Remove(unit);

            Destroy(unit.gameObject);
        }
    }
}