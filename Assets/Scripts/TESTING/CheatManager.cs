using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;

public class CheatManager : Singleton<CheatManager>
{
    public bool SeeHelpers { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) SeeHelpers = !SeeHelpers;
        if (Input.GetKeyDown(KeyCode.RightControl)) Time.timeScale = 4;
        if (Input.GetKeyUp(KeyCode.RightControl)) Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.RightShift)) Time.timeScale = 0.5f;
        if (Input.GetKeyUp(KeyCode.RightShift)) Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.Delete))
            foreach (var unit in SelectionManager.SelectedUnits)
                Destroy(unit.gameObject);
    }
}
