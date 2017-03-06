using System;
using System.Collections.Generic;
using UnityEngine;

public class Multi_Goal : Goal
{
	private List<Goal> goals;
	private bool all_goals;

	public Multi_Goal ()
	{
		goals = new List<Goal> ();
		all_goals = true;
	}

	public Multi_Goal(bool all_goals){
		goals = new List<Goal> ();
		this.all_goals = all_goals;
	}

	public void AddGoal(Goal new_goal){
		goals.Add (new_goal);
	}

	public void AddGoals(List<Goal> goal_list){
		foreach (Goal g in goal_list) {
			goals.Add (g);
		}
	}

	public bool IsEmpty(){
		return goals.Count == 0;
	}

	public override bool CheckCompletionSingle (Adventurer a)
	{
		bool complete = all_goals;
		foreach (Goal g in goals) {
			if (all_goals) {
				complete = complete && g.CheckCompletionSingle (a);
			} else {
				complete = complete || g.CheckCompletionSingle (a);
			}
		}
		return complete;
	}

	public override void UpdateTime ()
	{
		foreach (Goal g in goals) {
			g.UpdateTime ();
		}
	}
}


