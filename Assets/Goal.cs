using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Goal
{

	public Goal ()
	{
	}

	public abstract bool CheckCompletionSingle(Adventurer a);

	//public abstract bool CheckCompletionGroup(/*Dungeon Party?*/);

	public abstract void UpdateTime ();
}

