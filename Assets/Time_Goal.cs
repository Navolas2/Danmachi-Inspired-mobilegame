using System;
using System.Collections.Generic;
using UnityEngine;

public class Time_Goal : Goal
{
	private Date_Time endTime;
	private Date_Time startTime;
	private int[] lengths;

	public Time_Goal (int minutes = 0, int hours = 0, int days = 0, int years = 0)
	{
		lengths = new int[4];
		lengths [0] = minutes;
		lengths [1] = hours;
		lengths [2] = days;
		lengths [3] = years;
		startTime = GameClock.The_Clock.getTime ();
		endTime = new Date_Time(startTime);
		endTime.AddTime (minutes, hours, days, years);
	}
		

	public override bool CheckCompletionSingle (Adventurer a)
	{
		return (GameClock.The_Clock.getTime().Compare(endTime) > 0);
	}

	public override void UpdateTime ()
	{
		startTime = GameClock.The_Clock.getTime ();
		endTime = startTime;
		endTime.AddTime (lengths[0], lengths[1], lengths[2], lengths[3]);
	}
}


