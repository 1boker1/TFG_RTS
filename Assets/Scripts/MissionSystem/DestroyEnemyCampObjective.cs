using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Building;
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

			Building[] buildings= FindObjectsOfType<Building>();

			bool _TooClose=false;

			for(int x = 0;x<buildings.Length;x++)
			{
				if(buildings[x].Team==SelectionManager.Instance.m_Team)
				{
					if((buildings[x].transform.position-_Position).magnitude<200)
					{
						_TooClose=true;
						break;
					}
				}
			}

			if(_TooClose)
			{ 
				if(!(enemyCamps.Count==0 && i==amountOfCamps-1))
				continue;
			}		

			EnemyCamp _Camp =Instantiate(enemyCampPrefab, _Position, Quaternion.Euler(0,UnityEngine.Random.Range(0,360),0)).GetComponent<EnemyCamp>();
			
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
