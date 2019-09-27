using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Managers.SelectionManager;

namespace Assets.Scripts.Building
{
    public class SpawnBuildingCanvas : MonoBehaviour
    {
        public GameObject canvas;

        [SerializeField] private Button[] spawnButtons;
        [SerializeField] private Image spawnFillImage;

        private SpawnerBuilding selectedBuilding;

        private void FixedUpdate()
        {
            if (selectedBuilding) spawnFillImage.fillAmount = selectedBuilding.SpawnPercentage;
        }

        public void InformationSetUp(SpawnerBuilding building)
        {
            selectedBuilding = building;

            if (selectedBuilding == null) { DisableButtons(); return; }

            AssignBuildingSpawnButtons(selectedBuilding);
        }

        private void AssignBuildingSpawnButtons(SpawnerBuilding building)
        {
            DisableButtons();

            canvas.SetActive(true);

            for (var i = 0; i < building.unitsToSpawn.Length; i++)
            {
                spawnButtons[i].onClick.RemoveAllListeners();

                var spawnObject = building.unitsToSpawn[i];

                spawnButtons[i].onClick.AddListener(() => { building.AddToQueue(spawnObject); });
                spawnButtons[i].image.sprite = building.unitsToSpawn[i].GetComponent<IBasicData>().Image;
                spawnButtons[i].gameObject.SetActive(true);
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
