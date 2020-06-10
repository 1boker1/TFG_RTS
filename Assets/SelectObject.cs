using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    public List<GameObject> cubes = new List<GameObject>();

    void Update()
    {
        GameObject selected = null;
        if (Input.GetMouseButtonDown(0))
            selected = Utils.GetFromRay<Transform>().gameObject;

        if (selected != null)
        {
            SetPositions(selected);
        }
    }

    public void SetPositions(GameObject _Object)
    {
        Collider _Collider = _Object.GetComponent<Collider>();

        cubes[0].transform.position = _Collider.bounds.min;
        cubes[1].transform.position = _Collider.bounds.max;
        cubes[2].transform.position = -_Collider.bounds.min;
        cubes[3].transform.position = -_Collider.bounds.max;
    }
}
