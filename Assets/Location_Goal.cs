using System;
using System.Collections.Generic;
using UnityEngine;

public class Location_Goal : Goal
{
	coordinate goal;

	public Location_Goal (coordinate c)
	{
		goal = c;
	}

	public override bool CheckCompletionSingle(Adventurer a){
		return goal.IsLocation (a.explorer._location);
	}

	//public abstract bool CheckCompletionGroup(/*Dungeon Party?*/);

	public override void UpdateTime (){

	}
}
