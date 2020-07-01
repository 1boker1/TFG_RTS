using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Building;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SelectionLine : MonoBehaviour
{
	public static SelectionLine Instance;

    [SerializeField] private LayerMask FloorMask;
    [SerializeField] private LineRenderer lineRenderer;

    private GameObject currentObject;

	private void Awake()
	{
		if(Instance==null)
			Instance=this;
		if(Instance!=this)
			Destroy(gameObject);
	}

	public void LineSetup(GameObject SelectedObject)
    {
		if(SelectedObject==null)
			return;

        SetParent(SelectedObject.transform);

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        Bounds _LocalBounds = SelectedObject.GetComponent<MeshFilter>().mesh.bounds;

        Vector3[] _Positions = SetLinePositions(transform.right * _LocalBounds.extents.x, transform.forward * _LocalBounds.extents.z);

        lineRenderer.positionCount = _Positions.Length;
        lineRenderer.startWidth = 0.25f;

        for (int i = 0; i < _Positions.Length; i++)
            lineRenderer.SetPosition(i, _Positions[i]);

        transform.localRotation = Quaternion.Euler(new Vector3(0, -transform.parent.transform.localRotation.eulerAngles.y, 0));

        EnableLine(true);

        if (currentObject != SelectedObject.gameObject)
        {
            currentObject = SelectedObject.gameObject;
            StopAllCoroutines();
        }
    }

    private void SetParent(Transform Parent)
    {
        transform.parent = Parent.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private Vector3[] SetLinePositions(Vector3 Right, Vector3 Forward)
    {
        Vector3[] _Positions = new Vector3[4];

        _Positions[0] = (Right + Forward).With(y: 0.2f);
        _Positions[1] = (Right - Forward).With(y: 0.2f);
        _Positions[2] = (-Right - Forward).With(y: 0.2f);
        _Positions[3] = (-Right + Forward).With(y: 0.2f);

        return _Positions;
    }

    public void HighlightObject(ISelectable highlightedObject)
    {
        LineSetup(highlightedObject.transform.gameObject);

        StartCoroutine(Highlight(highlightedObject));
    }

    public IEnumerator Highlight(ISelectable selectableObject)
    {
        EnableLine(false);

        for (int i = 0; i < 6; i++)
        {
            EnableLine(!lineRenderer.enabled);
            yield return new WaitForSeconds(0.15f);
        }

        EnableLine(selectableObject.Selected);
    }

    public void EnableLine(bool Enabled)
    {
		if(lineRenderer!=null)
		{
			if(Enabled==false)
				transform.parent=null;

			lineRenderer.enabled = Enabled;
		}
    }
}
