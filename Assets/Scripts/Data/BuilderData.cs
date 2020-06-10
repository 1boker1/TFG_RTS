using System.Collections.Generic;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class BuilderData : MonoBehaviour, IBuildData, ISpawnData
    {
        [Header("Build Data")]
        [SerializeField] private float buildSpeed = 5;
        [SerializeField] private int buildAmount = 5;

        public float BuildSpeed { get => buildSpeed; set => buildSpeed = value; }
        public int BuildAmount { get => buildAmount; set => buildAmount = value; }

        [Header("Buildings")]
        [SerializeField] private List<GameObject> availableEntities;

        public List<GameObject> AvailableEntities { get => availableEntities; set => availableEntities = value; }
    }
}