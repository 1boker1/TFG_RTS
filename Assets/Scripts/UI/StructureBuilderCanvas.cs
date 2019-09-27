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
                    spawnButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    spawnButtons[i].gameObject.SetActive(true);
                    spawnButtons[i].image.sprite = data.AvailableEntities[i].GetComponent<IBasicData>().Image;

                    var building = data.AvailableEntities[i];
                    spawnButtons[i].onClick.AddListener(() => BuildingSpawn(building, data.transform.GetComponent<Unit.Unit>()));
                }
            }
        }

        private void BuildingSpawn(GameObject entity, Unit.Unit builder)
        {
            BuildingPreview buildingPreview = entity.GetComponent<BuildingPreview>();

            if (buildingPreview.woodCost <= ResourceManager.Instance.GetResourceAmount(typeof(Wood)) &&
                buildingPreview.goldCost <= ResourceManager.Instance.GetResourceAmount(typeof(Gold)) &&
                buildingPreview.foodCost <= ResourceManager.Instance.GetResourceAmount(typeof(Food)))
            {
                GameObject building = Instantiate(entity, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                building.GetComponent<BuildingPreview>().Worker = builder;
            }
        }

        private void DisableButtons()
        {
            canvas.SetActive(false);

            foreach (var button in spawnButtons)
            {
                button.gameObject.SetActive(false);
            }
        }
    }
}
