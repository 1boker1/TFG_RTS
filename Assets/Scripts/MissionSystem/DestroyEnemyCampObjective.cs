using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Managers;
using Assets.Scripts.ProceduralGeneration;
using UnityEngine;

public class DestroyEnemyCampObjective : Objective
{
	public GameObject enemyCampPrefab;
	public List<EnemyCamp> enemyCamps=new List<EnemyCamp>();

	public int amountOfCamps;

	public override bool Completed()
	{
		foreach(EnemyCamp camp in enemyCamps)
		{
			if(!camp.Completed())
				return false;
		}
		return true;
	}

	public override void InitObjective(MeshGenerator _MeshGenerator)
	{
		GameObjectives.Objectives.Add(this);
		enemyCamps.Clear();

		for(int i = 0;i<amountOfCamps;i++)
		{
			Vector3 _Position = _MeshGenerator.GetValidMapPosition(new Vector3(25, 1, 25), 10000);

			if (_Position != Vector3.zero)
			{
				_MeshGenerator.FlatMapInRadius(_Position, 35);		
			}

			EnemyCamp _Camp=Instantiate(enemyCampPrefab, _Position, Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0)).GetComponent<EnemyCamp>();
			
			_Camp.currentObjective=this;
			_Camp.SetUp();

			enemyCamps.Add(_Camp);
		}
	}

	public override void OnCompleted()
	{
		if(GameObjectives.AreObjectivesComplete())
			GameObjectives.OnCompletedObjectives();
	}
}
