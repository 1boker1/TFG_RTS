using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Building;
using Assets.Scripts.Data;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace Assets.Scripts.UI
{
    public class SelectionCanvas : Singleton<SelectionCanvas>
    {
        public GameObject canvas;

        [Header("Entity")]
        [SerializeField] private Text entityName;
        [SerializeField] private Image entityImage;

        [SerializeField] private Image entityHealthBar;
        [SerializeField] private Text entityHealthText;

        [SerializeField] private Text entityAttackDamageText;
        [SerializeField] private Text entityAttackRangeText;
        [SerializeField] private Text entityAttackSpeedText;

        private List<ISelectable> selectedEntities = new List<ISelectable>();
        private ISelectable showedEntity;

        [SerializeField] private UnitSpawnerCanvas unitSpawnerCanvas;
        [SerializeField] private StructureBuilderCanvas structureBuilderCanvas;

        private void Start()
        {
            SelectionManager.SelectedEntities.ListChanged += SelectedEntitiesListChanged;

            EnableCanvas(false);
        }

        private void FixedUpdate()
        {
            if (selectedEntities.Count == 0 || selectedEntities[0] == null)
                return;

            AssignHealthData(showedEntity.transform.GetComponent<IHealth>());
        }

        private void SelectedEntitiesListChanged(object sender, ListChangedEventArgs args)
        {
            selectedEntities = SelectionManager.SelectedEntities.ToList();
            EnableCanvas(false);
            UpdateCanvasData();
        }

        private void UpdateCanvasData()
        {
            if (selectedEntities.Count == 0 || selectedEntities[0] == null) return;

            AssignCanvasData(selectedEntities[0]);
        }

        private void AssignCanvasData(ISelectable entity)
        {
            showedEntity = entity;

            if (showedEntity == null) return;

            EnableCanvas(true);

            AssignBasicData(showedEntity.transform.GetComponent<IBasicData>());
            AssignHealthData(showedEntity.transform.GetComponent<IHealth>());
            AssignAttackData(showedEntity.transform.GetComponent<IAttackData>());

            unitSpawnerCanvas.InformationSetUp(showedEntity.transform.GetComponent<SpawnerBuilding>());
            structureBuilderCanvas.InformationSetUp(showedEntity.transform.GetComponent<BuilderData>());
        }

        private void EnableCanvas(bool enable)
        {
            canvas.SetActive(enable);
            unitSpawnerCanvas.canvas.SetActive(enable);
            structureBuilderCanvas.canvas.SetActive(enable);
        }

        private void AssignBasicData(IBasicData data)
        {
            if (data == null) return;

            entityImage.sprite = data.Image;
            entityName.text = data.Name;
        }

        private void AssignHealthData(IHealthData data)
        {
            if (data == null) return;

            entityHealthText.text = data.HealthPoints + " / " + data.MaxHealthPoints;
            entityHealthBar.fillAmount = data.HealthPoints / (float)data.MaxHealthPoints;
        }

        private void AssignAttackData(IAttackData data)
        {
            entityAttackDamageText.enabled = data != null;
            entityAttackRangeText.enabled = data != null;
            entityAttackSpeedText.enabled = data != null;

            if (data == null) return;

            entityAttackDamageText.text = "Strength: " + data.AttackDamage;
            entityAttackRangeText.text = "Range: " + data.UnitRange;
            entityAttackSpeedText.text = "Attack Speed: " + data.AttackSpeed;
        }
    }
}
