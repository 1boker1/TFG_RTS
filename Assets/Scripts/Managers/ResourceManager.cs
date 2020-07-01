using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class ResourceManager : MonoBehaviour
    {
		public static ResourceManager Instance;
        [SerializeField] private float MaxCapacity = 1000;

        private List<ResourceType> resourceTypes = new List<ResourceType>();

        public List<Resource> resources = new List<Resource>();

        [SerializeField] private Text woodText;
        [SerializeField] private Text foodText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text rockText;

		private void Awake()
		{
			if(Instance==null)
				Instance=this;
			if(Instance!=this)
				Destroy(gameObject);
		}

		private void Start()
        {
            InitializeResources();
			InitialValues();

            UpdateUI();
        }

        public void AddResource(Type type, float amount)
        {
            foreach (var resource in resourceTypes)
            {
                if (resource.GetType() == type)
                {
                    resource.amount += amount;
                    resource.amount = Mathf.Clamp(resource.amount, 0, MaxCapacity);
                }
            }

            UpdateUI();
        }

        public void AddResource(ResourceType resource)
        {
            foreach (var resourceType in resourceTypes)
            {
                if (resourceType.GetType() == resource.GetType())
                {
                    resourceType.amount += resource.amount;
                    resourceType.amount = Mathf.Clamp(resource.amount, 0, MaxCapacity);
                }
            }

            UpdateUI();
        }

        private void InitializeResources()
        {
            var types = Assembly.GetAssembly(typeof(ResourceType)).GetTypes().Where(type => type.IsSubclassOf(typeof(ResourceType)));

            foreach (var type in types)
            {
                resourceTypes.Add(Activator.CreateInstance(type) as ResourceType);
            }
        }

        private void UpdateUI()
        {
            woodText.text = GetResourceAmount(typeof(Wood)).ToString("N0") + " / " + MaxCapacity;
            foodText.text = GetResourceAmount(typeof(Food)).ToString("N0") + " / " + MaxCapacity;
            goldText.text = GetResourceAmount(typeof(Gold)).ToString("N0") + " / " + MaxCapacity;
            rockText.text = GetResourceAmount(typeof(Rock)).ToString("N0") + " / " + MaxCapacity;
        }

        public float GetResourceAmount(Type resource)
        {
            return (from type in resourceTypes where type.GetType() == resource select type.amount).FirstOrDefault();
        }

        public void ModifyMaxCapacity(float newCapacity) => MaxCapacity += newCapacity;

        public void InitialValues()
		{
			foreach (var resource in resourceTypes)
            {
                resource.amount=100;
            }
		}
    }
}
