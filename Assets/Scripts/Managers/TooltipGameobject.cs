using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipGameobject : MonoBehaviour
{
    //Use singleton And split infromation + Add More Of it
    public static GameObject TooltipObject;
    public static Text TooltipText;

    private void Awake()
    {
        TooltipObject = gameObject;
        TooltipText = GetComponentInChildren<Text>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        TooltipObject.transform.position = Input.mousePosition + Vector3.up * 10;
    }

    public static void Show(string Text)
    {
        TooltipText.text = Text;
        TooltipObject.transform.position = Input.mousePosition + Vector3.up * 10;
        TooltipObject.SetActive(true);
    }

    public static void Hide()
    {
        TooltipObject.SetActive(false);
    }
}

