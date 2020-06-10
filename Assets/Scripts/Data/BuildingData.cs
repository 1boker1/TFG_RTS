using Assets.Scripts.Interfaces;
using Assets.Scripts.Resources;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class BuildingData : MonoBehaviour, ISpendable, IBasicData, IHealthData
    {
        public bool built;
        public int team;

        [Header("Basic Data")]
        [SerializeField] private Sprite image;
        [SerializeField] private string name;
        public string Name => name;
        public Sprite Image => image;


        [Header("Health Data")]
        [SerializeField] private int maxHealthPoints = 10;
        [SerializeField] private int healthPoints = 10;
        public int MaxHealthPoints { get => maxHealthPoints; set => maxHealthPoints = value; }
        public int HealthPoints { get => healthPoints; set => healthPoints = value; }


        [Header("Spendable Data")]
        [SerializeField] private int woodCost;
        [SerializeField] private int foodCost;
        [SerializeField] private int rockCost;
        [SerializeField] private int goldCost;

        public Wood WoodCost => new Wood(woodCost);
        public Food FoodCost => new Food(foodCost);
        public Rock RockCost => new Rock(rockCost);
        public Gold GoldCost => new Gold(goldCost);

        [Header("OtherData")]
        public bool IsWallPillar;
    }
}
