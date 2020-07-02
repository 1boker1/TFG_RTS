using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    public bool SeeHelpers { get; private set; }
	public static CheatManager Instance;

	private void Awake()
	{
#if UNITY_EDITOR
		Destroy(gameObject);
		if(Instance==null)
				Instance=this;
			if(Instance!=this)
				Destroy(gameObject);

		Debug.LogError("Quitar los cheats para la build final");
#endif

	}
	void Update()
    {
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.L))
			GetComponent<Light>().enabled=!GetComponent<Light>().enabled;
        //if (Input.GetKeyDown(KeyCode.C)) SeeHelpers = !SeeHelpers;
        if (Input.GetKeyDown(KeyCode.RightControl)) Time.timeScale = 5;
        if (Input.GetKeyUp(KeyCode.RightControl)) Time.timeScale = 1;
        if (Input.GetKeyDown(KeyCode.RightShift)) Time.timeScale = 0.5f;
        if (Input.GetKeyUp(KeyCode.RightShift)) Time.timeScale = 1;
        //if (Input.GetKeyDown(KeyCode.Delete))
        //    foreach (var unit in SelectionManager.SelectedUnits)
        //        Destroy(unit.gameObject);
#endif

    }
}
