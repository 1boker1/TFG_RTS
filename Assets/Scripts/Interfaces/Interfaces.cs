using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Resources;
using UnityEngine;
// ReSharper disable IdentifierTypo

namespace Assets.Scripts.Interfaces
{
    public interface IMonobehaviour
    {
        Transform transform { get; }
    }

    public interface IHealth : IHealthData
    {
        void ModifyHealth(int value, ISelectable attacker);

        void OutOufHealth();
    }

    public interface ISelectable : IMonobehaviour
    {
        int Team { get; set; }

        bool Selected { get; set; }

        void Select(int? team);
        void Deselect();

        IEnumerator Highlight();

        Type GetAction(Unit.Unit unit, int? team);
        Type GetActionWithoutTarget(Unit.Unit unit, int? team);
    }

    public interface IGuardable
    {
        void GuardUnit(Unit.Unit unit);

        void RestoreUnit(int index);

        void RestoreAllUnits();

        void SetGuardedUnitsDestination();
    }

    public interface IRangeBuildable
    {
        bool CanBuild();
    }

    public interface IBuildable
    {
        bool Built { get; set; }

        void Build(int amount);
    }

    public interface IHealthData
    {
        int MaxHealthPoints { get; set; }
        int HealthPoints { get; set; }
    }

    public interface ISpendable
    {
        Wood WoodCost { get; }
        Food FoodCost { get; }
        Rock RockCost { get; }
        Gold GoldCost { get; }
    }

    public interface IBasicData
    {
        Sprite Image { get; }

        string Name { get; }
    }

    public interface IGatherData
    {
        float GatherSpeed { get; set; }
        int GatherAmount { get; set; }
    }

    public interface IBuildData
    {
        float BuildSpeed { get; set; }
        int BuildAmount { get; set; }
    }

    public interface IAttackData
    {
        float AttackSpeed { get; set; }
        float AttackRange { get; set; }
        float AttackDamage { get; set; }
    }

    public interface IMovementData
    {
        float SearchRange { get; set; }
        float MovementSpeed { get; set; }
    }

    public interface ISpawnData
    {
        List<GameObject> AvailableEntities { get; set; }
    }
}