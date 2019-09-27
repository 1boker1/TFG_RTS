using Assets.Scripts.Interfaces;
using Assets.Scripts.Resources;
using UnityEngine;

// ReSharper disable ConvertToAutoProperty
#pragma warning disable 649

namespace Assets.Scripts.Data
{
    public class UnitData : MonoBehaviour, ISpendable, IBasicData, IAttackData, IGatherData, IHealthData, IMovementData
    {
        public int team;
        public float timeToSpawn;

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


        [Header("Attack Data")]
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackDamage = 5;
        public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
        public float AttackRange { get => attackRange; set => attackRange = value; }
        public float AttackDamage { get => attackDamage; set => attackDamage = value; }


        [Header("GatherData")]
        [SerializeField] private float gatherSpeed = 1f;
        [SerializeField] private int gatherAmount = 5;
        public float GatherSpeed { get => gatherSpeed; set => gatherSpeed = value; }
        public int GatherAmount { get => gatherAmount; set => gatherAmount = value; }

        [Header("Movement Data")]
        [SerializeField] private float searchRange = 10;
        [SerializeField] private float movementSpeed = 10;
        public float SearchRange { get => searchRange; set => searchRange = value; }
        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    }
}