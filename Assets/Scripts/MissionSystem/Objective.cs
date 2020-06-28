using System.Collections;
using Assets.Scripts.ProceduralGeneration;
using UnityEngine;

public abstract class Objective : MonoBehaviour 
{
	public abstract void InitObjective(MeshGenerator _MeshGenerator);
	public abstract void OnCompleted();
	public abstract bool Completed();
}
