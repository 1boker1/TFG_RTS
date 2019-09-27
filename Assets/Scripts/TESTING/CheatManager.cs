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
    }
}
