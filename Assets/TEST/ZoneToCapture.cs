using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Unit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ZoneToCapture : MonoBehaviour
{
    private HashSet<Unit> insideUnits = new HashSet<Unit>();

    [SerializeField] private Image sprite;

    [SerializeField] private float timeToCapture = 30f;
    private float progress = 0f;

    [SerializeField] private UnityEvent OnCapture;

    private void Update()
    {
        if (insideUnits.Count == 0) return;

        progress += Time.deltaTime;

        sprite.fillAmount = progress / timeToCapture;

        if (progress >= timeToCapture) OnCapture?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Unit>() != null) insideUnits.Add(other.GetComponent<Unit>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>() != null) insideUnits.Remove(other.GetComponent<Unit>());
    }

    public void ChangeColor()
    {
        sprite.color = Color.green;
    }
}
