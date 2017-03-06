using System;
using System.Collections.Generic;
using UnityEngine;

public class Kill_Goal : Goal
{
	private string target;
	private int count;

	public Kill_Goal (int count)
	{
		target = "";
		this.count = count; 
	}

	public Kill_Goal (string tar, int count)
	{
		target = tar;
		this.count = count;
	}

	public override bool CheckCompletionSingle (Adventurer a)
	{
		int currentCount = 0;
		List<string[]> kills = a.diary.GetLog(new List<string>(){DungeonLog.ENTRY_TYPE_KILL});
		foreach (string[] killItem in kills) {
			if (killItem [2] == target || target == "") {
				currentCount++;
			}
		}
		return (currentCount >= count);
	}

	public override void UpdateTime ()
	{
		//I don't have a time
	}
}


