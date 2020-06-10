using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class PopulationManager : Singleton<PopulationManager>
    {
        public Text Population;

        public int MaxPopulation { get => maxPopulation; set { maxPopulation = value; OnPopulationChange?.Invoke(); } }
        [SerializeField] private int maxPopulation = 50;

        public int CurrentPopulation { get => currentPopulation; set { currentPopulation = value; OnPopulationChange?.Invoke(); } }
        private int currentPopulation = 0;

        public Action OnPopulationChange;

        private void Start()
        {
            OnPopulationChange += UpdateUI;

            UpdateUI();
        }

        public void UpdateUI()
        {
            Population.text = CurrentPopulation + "/" + MaxPopulation;
        }

        public bool CanSpawn()
        {
            return CurrentPopulation < MaxPopulation;
        }
    }
}
