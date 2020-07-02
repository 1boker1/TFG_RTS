using System.Collections.Generic;
using Assets.Scripts.Building;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StructureBuilderCanvas : MonoBehaviour
    {
        public GameObject canvas;

        [SerializeField] private List<Button> spawnButtons;

        public void InformationSetUp(BuilderData data)
        {
            if (data == null) { DisableButtons(); return; }

            canvas.SetActive(true);

            for (int i = 0; i < spawnButtons.Count; i++)
            {
                spawnButtons[i].onClick.RemoveAllListeners();

                if (i >= data.AvailableEntities.Count || data.AvailableEntities[i] == null)
                {
                    spawnButtons[i].image.enabled = false;
                    spawnButtons[i].enabled = false;
                }
                else
                {
                    spawnButtons[i].image.sprite = data.AvailableEntities[i].GetComponent<IBasicData>().Image;
                    spawnButtons[i].image.enabled = true;
                    spawnButtons[i].enabled = true;

                    var building = data.AvailableEntities[i];
                    spawnButtons[i].onClick.AddListener(() => BuildingSpawn(building, data.transform.GetComponent<Unit.Unit>()));

                    Tooltip _Tooltip = spawnButtons[i].GetComponent<Tooltip>();

					_Tooltip.SetData(building.GetComponent<ISpendable>());
                }
            }
        }

        private void BuildingSpawn(GameObject entity, Unit.Unit builder)
        {
            BuildingData buildingData = entity.GetComponent<BuildingData>();

            if (buildingData.WoodCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Wood)) &&
                buildingData.GoldCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Gold)) &&
                buildingData.RockCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Rock)) &&
                buildingData.FoodCost.amount <= ResourceManager.Instance.GetResourceAmount(typeof(Food)))
            {
                BuildingPreview.instance.Worker = builder;
                BuildingPreview.instance.SetUpBuilding(entity);
            }
        }

        private void DisableButtons()
        {
            canvas.SetActive(false);

            foreach (var button in spawnButtons)
            {
                button.enabled = false;
                button.image.enabled = false;
            }
        }
    }
}
