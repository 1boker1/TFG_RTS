using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private GameObject initialPoint;
    [SerializeField] private LineRenderer line;

    public void SetPositions()
    {
        line.SetPosition(0, FlattenY(initialPoint.transform.position));
        line.SetPosition(1, FlattenY(gameObject.transform.position));
    }

    private Vector3 FlattenY(Vector3 vectorToFlat)
    {
        return new Vector3(vectorToFlat.x, 1, vectorToFlat.z);
    }
}
