using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Building;
using Assets.Scripts.Resources;
using UnityEngine;

public class Isvisible : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Resource>() != null || other.GetComponent<Building>() != null)
        {
            if (other.GetComponent<Renderer>() != null)
                other.GetComponent<Renderer>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Resource>() != null || other.GetComponent<Building>() != null)
        {
            if (other.GetComponent<Renderer>() != null)
                other.GetComponent<Renderer>().enabled = true;
        }
    }
}

