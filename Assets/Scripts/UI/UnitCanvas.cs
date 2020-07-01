using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Building;
using Assets.Scripts.Managers;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.UI;

public class UnitCanvas : MonoBehaviour
{
	public static UnitCanvas Instance;
    [SerializeField] private Image unitSprite;

    [SerializeField] private Text attackDamage;
    [SerializeField] private Text attackSpeed;
    [SerializeField] private Text movementSpeed;
    [SerializeField] private Text unitAmount;

    [SerializeField] private List<Button> buttons = new List<Button>();

    [SerializeField] private GameObject GroupCanvas;

    private List<GroupedUnits> selectedUnitsData = new List<GroupedUnits>();

    public Unit lastUnitCanvas;
    public int lastUnitCanvasCount = 0;

    public bool isShowingUI;

	private void Awake()
	{
		if(Instance==null)
				Instance=this;
		if(Instance!=this)
				Destroy(gameObject);
	}

	private void Update()
    {
        SelectedUI(SelectionManager.SelectedUnits);
    }

    public void ChangeGroup(Unit unit, int amount)
    {
        unitSprite.sprite = unit.unitData.Image;

        attackDamage.text = "Attack Damage: " + unit.unitData.AttackDamage;
        attackSpeed.text = "Attack Speed: " + unit.unitData.AttackSpeed;
        movementSpeed.text = "Movement Speed: " + unit.unitData.MovementSpeed;
        unitAmount.text = amount.ToString();
    }

    public void ButtonClicked(Button clickedButton)
    {
        int index = buttons.IndexOf(clickedButton);

        ArrangeButtons(index + 1);
    }

    public void SetParameters(List<GroupedUnits> data)
    {
        selectedUnitsData = data;

        ClearButtons();

        ChangeGroup(selectedUnitsData[0].unit, selectedUnitsData[0].amount);

        for (int i = 1; i < data.Count; i++)
        {
            if (i == buttons.Count) break;
            if (buttons[i - 1] == null || selectedUnitsData[i] == null) break;

            buttons[i - 1].image.sprite = selectedUnitsData[i].unit.unitData.Image;
            buttons[i - 1].gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    public void SetCanvas(bool active)
    {
        GroupCanvas.SetActive(active);
    }

    private void ClearButtons()
    {
        foreach (var button in buttons)
        {
            if (button == null) continue;

            button.image.sprite = null;
            button.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    private void ArrangeButtons(int index)
    {
        var temp = selectedUnitsData[index];
        selectedUnitsData[index] = selectedUnitsData[0];
        selectedUnitsData[0] = temp;

        SetParameters(selectedUnitsData);
    }

    public void SelectedUI(List<Unit> unitList)
    {
        if (unitList.Count > 0)
        {
            if (isShowingUI && unitList[0] == lastUnitCanvas && lastUnitCanvasCount == unitList.Count) return;

            lastUnitCanvas = unitList[0];
            lastUnitCanvasCount = unitList.Count;

            var units = new List<GroupedUnits>();

            foreach (var unit in unitList)
            {
                var found = false;
                var index = 0;

                foreach (var data in units.Where(data => data.unit.unitData.Name != unit.unitData.Name))
                {
                    found = true;
                    index = units.IndexOf(data);
                    break;
                }

                if (found) units[index].amount++;
                else units.Add(new GroupedUnits(unit, 1));
            }

            SetParameters(units);
            SetCanvas(true);

            isShowingUI = true;
        }
        else
        {
            SetCanvas(false);

            isShowingUI = false;
        }
    }
}
