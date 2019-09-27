using System.Linq;
using Assets.Scripts.Managers;
using UnityEngine;

public class SelectorDrawer : MonoBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Texture2D rectangleTexture;

    private Vector3 startPosition = -Vector3.one;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) startPosition = Input.mousePosition;
        if (Input.GetMouseButtonUp(0)) startPosition = -Vector3.one;
    }

    private void OnGUI()
    {
        if (startPosition == -Vector3.one) return;

        GUI.color = color;
        GUI.DrawTexture(Utils.DragRectangle(startPosition), rectangleTexture);
    }

}