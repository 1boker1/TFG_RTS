using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.Managers.SelectionManager;

namespace Assets.Scripts.Building
{
    public class UnitSpawnerCanvas : MonoBehaviour
    {
        public GameObject canvas;

        [SerializeField] private Button[] spawnButtons;
        [SerializeField] private Button[] queueButtons;
        [SerializeField] private Image spawnFillImage;

        private SpawnerBuilding selectedBuilding;

        private void Start()
        {
            ResetQueue();
        }

        private void FixedUpdate()
        {
            if (selectedBuilding)
            {
                spawnFillImage.fillAmount = selectedBuilding.SpawnPercentage;

                UpdateQueue();
            }
        }

        public void InformationSetUp(SpawnerBuilding building)
        {
            selectedBuilding = building;

            if (selectedBuilding == null || !selectedBuilding.building.GetBuildingHealthSystem().Built) { DisableButtons(); return; }

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

                spawnButtons[i].onClick.RemoveAllListeners();
                spawnButtons[i].onClick.AddListener(() => { building.AddToQueue(spawnObject); });
                spawnButtons[i].image.sprite = spawnObject.GetComponent<IBasicData>().Image;
                spawnButtons[i].image.enabled = true;
                spawnButtons[i].enabled = true;

                Tooltip _Tooltip = spawnButtons[i].GetComponent<Tooltip>();

                _Tooltip.SetData(spawnObject.GetComponent<ISpendable>());
            }
        }

        private void UpdateQueue()
        {
            for (var i = 0; i < selectedBuilding.queue.Count; i++)
            {
                var unit = selectedBuilding.queue[i];

                queueButtons[i].image.sprite = unit.Image;
                queueButtons[i].image.enabled = true;
                queueButtons[i].enabled = true;

                int index = i;

                queueButtons[i].onClick.RemoveAllListeners();
                queueButtons[i].onClick.AddListener(() => { CancelAtIndex(index); });
            }
        }

        private void ResetQueue()
        {
            foreach (var button in queueButtons)
            {
                button.GetComponent<Image>().enabled = false;
                button.enabled = false;
            }
        }

        private void DisableButtons()
        {
            canvas.SetActive(false);

            foreach (var button in spawnButtons)
            {
                button.GetComponent<Image>().enabled = false;
                button.enabled = false;
            }
        }

        private void CancelAtIndex(int i)
        {
            if (i == 0)
                spawnFillImage.fillAmount = 0;

            selectedBuilding.RemoveFromQueue(i);
            ResetQueue();
            UpdateQueue();
        }

        private void OnEnable()
        {
            if (selectedBuilding != null)
                selectedBuilding.OnUnitSpawned += ResetQueue;

            ResetQueue();
        }

        private void OnDisable()
        {
            if (selectedBuilding != null)
                selectedBuilding.OnUnitSpawned -= ResetQueue;

            ResetQueue();
        }
    }
}
