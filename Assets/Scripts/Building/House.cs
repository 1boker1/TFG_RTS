using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;

public class House : MonoBehaviour
{
    [SerializeField] private int m_Population;

    private bool m_AddedPopulation;

    public void AddPopulation()
    {
        PopulationManager.Instance.MaxPopulation += m_Population;

        m_AddedPopulation = true;
    }

    public void RemovePopulation()
    {
        PopulationManager.Instance.MaxPopulation -= m_Population;
    }

    private void OnDestroy()
    {
        if (m_AddedPopulation && PopulationManager.Instance != null)
            RemovePopulation();
    }
}
