using System.Collections.Generic;
using UnityEngine;

public static class GameObjectives
{
	public static List<Objective> Objectives=new List<Objective>();

	public delegate void OnCompleteObjectives();
	public static OnCompleteObjectives OnObjectivesCompleted;

	public static bool AreObjectivesComplete()
	{
		foreach(Objective objective in Objectives)
		{
			if(!objective.Completed())
				return false;
		}

		return true;
	}

	public static void OnCompletedObjectives()
	{
		Debug.Log("Completed Objectives!");
		OnObjectivesCompleted?.Invoke();
		ResetGameObjectives();
	}

	public static void ResetGameObjectives()
	{
		Objectives.Clear();

		OnObjectivesCompleted=null;
	}

}
