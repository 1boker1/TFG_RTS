using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    public void SetPositions(Vector3 InitialPoint)
    {
        line.positionCount = 2;
        line.useWorldSpace = true;

        line.SetPosition(0, MathExtension.CorrectVerticalPosition(InitialPoint) + Vector3.up);
        line.SetPosition(1, MathExtension.CorrectVerticalPosition(transform.position + Vector3.up));
    }

    private Vector3 FlattenY(Vector3 vectorToFlat)
    {
        return new Vector3(vectorToFlat.x, 1, vectorToFlat.z);
    }
}
