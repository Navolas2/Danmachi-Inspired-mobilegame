using UnityEngine;
using System.Collections;

public class GameClock : MonoBehaviour
{
	public static GameClock The_Clock;
	private static int Minimum_Game_Speed = 30; //currently experimenting with 

	public int Game_Speed = 0;
	public int minutes_in_hour = 60;
	public int hours_in_day = 24;
	public int days_in_year = 200;

	private int currentTick;
	private int hour;
	private int minute;
	private int year;
	private int day;

	void Awake(){
		if (The_Clock == null) {
			DontDestroyOnLoad (gameObject);
			The_Clock = this;
			//LoadData ();
		} else if (The_Clock != this) {
			Destroy (gameObject);
		}
	} 

	// Use this for initialization
	void Start ()
	{
		//Load time information
		if (Game_Speed < Minimum_Game_Speed) {
			Game_Speed = Minimum_Game_Speed;
		}
		currentTick = 0;
		hour = 0;
		minute = 0;
		year = 0;
		day = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTick++;
		if (currentTick % Game_Speed == 0) {
			currentTick = 0;
			minute++;
			if (minute % minutes_in_hour == 0) {
				minute = 0;
				hour++;
				if (hour % hours_in_day == 0) {
					hour = 0;
					day++;
					if (day % days_in_year == 0) {
						year++;
					}
				}
			}
		}
	}

	/// <summary>
	/// Returns true or false if it is the proper time to trigger something that should only fire at certain times
	/// </summary>
	/// <returns><c>true</c>, if it is time to fire, <c>false</c> otherwise returns false</returns>
	/// <param name="frequency"> This is when in a sequence the update should fire. ie: 1 being at the beginning/end and .5 being half way through</param>
	public bool TimeBasedUpdate(double frequency){
		if (currentTick == 0 && (frequency != 1 && frequency != 0)) {
			return false;
		}
		return currentTick % (Game_Speed * frequency) == 0;
	}
	// CURRENT TIME BASED UPDATES AND TIMES
	//.1 - NONE
	//.2 - BATTLE
	//.3 - SOUND REACTIONS
	//.4 - SPAWNING
	//.5 - DUNGEON FLOOR ACTIONS
	//.6 - NONE
	//.7 - BATTLE
	//.75 - GOAL CHECKING
	//.8 - SOUND REACTIONS
	//.9 - SPAWNING
	//0/1 - DUNGEON FLOOR ACTIONS (0 and 1 are considered the same)


	public int Current_Tick{
		get{ return currentTick; }
	}

	public Date_Time getTime(){
		return new Date_Time (minute, hour, day, year, currentTick);
	}
}

public class Date_Time{
	private int hour;
	private int minute;
	private int year;
	private int day;
	private int tick;

	public Date_Time(int m, int h, int d, int y, int t){
		minute = m;
		hour = h;
		day = d;
		year = y;
		tick = t;
	}

	public Date_Time(Date_Time copy){
		hour = copy.hour;
		minute = copy.minute;
		year = copy.year;
		day = copy.day;
		tick = copy.tick;
	}

	public int Compare(Date_Time d){
		int out_val = 0;
		out_val += ((year - d.year) * 100000000);
		out_val += ((day - d.day) * 100000);
		out_val += ((hour - d.hour) * 1000);
		out_val += ((minute - d.minute) * 10);
		out_val += tick - d.tick;
		return out_val;
	}

	public void AddTime(int minutes, int hours, int days, int years){
		minute += minutes;
		hour += hours;
		day += days;
		year += years;
	}

	public override string ToString ()
	{
		return string.Format ( "{0}:{1:00}  [ {2} / {3} ]", hour, minute, day, year);
		//return string.Format (hour + ":" + minute + " [" + day + "/" + year + "]");
	}
}
