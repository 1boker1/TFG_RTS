using System;
using System.Collections;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.StateMachine.States;
using UnityEngine;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Assets.Scripts.Resources
{
    public class Resource : MonoBehaviour, ISelectable, IHealth, IBasicData
    {
        public Collections.ResourceType resourceType;

        [SerializeField] private Sprite image;
        [SerializeField] private string name;

        public string Name => name;
        public Sprite Image => image;

        [SerializeField] private int maxAmount;
        [SerializeField] private int amount;

        public int MaxHealthPoints { get => maxAmount; set => maxAmount = value; }
        public int HealthPoints { get => amount; set => amount = value; }

        public int Team { get; set; }
        public bool Selected { get; set; }

        public float gatherMultiplier = 1f;
        public bool infiniteSource = false;

        private void Start()
        {
            ResourceManager.Instance.resources.Add(this);

            HealthPoints = MaxHealthPoints;
        }

        public void Select(int? Team)
        {
            Selected = true;

            SelectionLine.Instance.LineSetup(gameObject);

            if (!SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Add(this);
        }

        public void Deselect()
        {
            Selected = false;

            if (SelectionLine.Instance != null)
                SelectionLine.Instance.EnableLine(false);

            if (SelectionManager.SelectedEntities.Contains(this))
                SelectionManager.SelectedEntities.Remove(this);
        }

        public IEnumerator Highlight()
        {
            SelectionLine.Instance.HighlightObject(this);
            yield break;
        }

        public Type GetAction(Unit.Unit unit, int? team)
        {
            unit.Target = this;
            return typeof(GatherState);
        }

        public Type GetActionWithoutTarget(Unit.Unit unit, int? team)
        {
            return typeof(GatherState);
        }

        public void ModifyHealth(int value, ISelectable attacker)
        {
            if(amount<=0)
                return;

            if (!infiniteSource)
                amount -= (int)(value * gatherMultiplier);

            var type = typeof(ResourceType);

            switch (resourceType)
            {
                case Collections.ResourceType.Wood: type = typeof(Wood); break;
                case Collections.ResourceType.Gold: type = typeof(Gold); break;
                case Collections.ResourceType.Food: type = typeof(Food); break;
                case Collections.ResourceType.Rock: type = typeof(Rock); break;
            }

            ResourceManager.Instance.AddResource(type, value * gatherMultiplier);

            if (amount <= 0)
                OutOufHealth();
        }
            
        public void OutOufHealth()
        {
            Deselect();

            ResourceManager.Instance.resources.Remove(this);

            Destroy(gameObject);
        }
	}
}