using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Interfaces;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipGameobject.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipGameobject.Hide();
    }

    public void SetData(ISpendable data)
    {
        tooltipText = "Food: " + data.FoodCost.amount
         + " | " + "Wood: " + data.WoodCost.amount
         + " | " + "Gold: " + data.GoldCost.amount
         + " | " + "Rock: " + data.RockCost.amount;
    }
}