using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DungeonLog
{
	public static string ENTRY_TYPE_KILL = "KILL";
	public static string ENTRY_TYPE_ENTER = "ENTER"; //only used for entering the dungeon
	public static string ENTRY_TYPE_EXIT = "EXIT"; //only used for exiting the dungeon
	public static string ENTRY_TYPE_DEATH = "DEATH";
	public static string ENTRY_TYPE_COMBAT = "COMBAT";
	public static string ENTRY_TYPE_ITEM = "ITEM";
	public static string ENTRY_TYPE_EVENT = "EVENT";

	private string owner;
	private List<LogEntry> log;
	public DungeonLog (string name)
	{
		owner = name;
		log = new List<LogEntry> ();
	}

	public void AddEntry(string type, string info, string user, string target){
		log.Add (new LogEntry (type, info, user, target));
	}

	public void AddEntries(DungeonLog d){
		List<LogEntry> l = d.GetEntries ();
		foreach (LogEntry entery_log in l) {
			log.Add (entery_log);
		}
		log.Sort (delegate(LogEntry x, LogEntry y) {
			return x.time.Compare (y.time);
		});
	}

	//This may change later to print it to where its suppose to be writing
	/*public void PrintLog(){
		for (int i = 0; i < log.Count; i++) {
			print (log [i].tag + " : " + log [i].entry + " " + log[i].time.ToString());
		}
	}

	public void PrintLog(List<string> wanted){
		for (int i = 0; i < log.Count; i++) {
			if (wanted.Contains (log [i].tag)) {
				print (log [i].tag + " : " + log [i].entry);
			}
		}
	}
*/
	public List<string[]> GetLog(){
		List<string[]> out_list =  new List<string[]>();
		for (int i = 0; i < log.Count; i++) {
			out_list.Add (new string[] {log [i].tag + " : " + log [i].entry + " " + log[i].time.ToString(), log[i]._user});
		}
		return out_list;
	}

	public List<string[]> GetLog(List<string> wanted){
		List<string[]> out_list =  new List<string[]>();
		for (int i = 0; i < log.Count; i++) {
			if (wanted.Contains (log [i].tag)) {
				out_list.Add (new string[] { log [i].tag + " : " + log [i].entry, log [i]._user, log[i]._target });
			}
		}
		return out_list;
	}

	private List<LogEntry> GetEntries(){
		return log;
	}

	public string belongs_to{
		get{ return owner; }
	}

	public void ClearLog(){
		log.Clear ();
	}
}

class LogEntry
{
	private string entry_information;
	private string entry_type;
	private Date_Time entry_time;
	private string user;
	private string target;

	public LogEntry (string type, string info, string user, string target)
	{
		entry_type = type;
		entry_information = info;
		entry_time = GameClock.The_Clock.getTime ();
		this.user = user;
		this.target = target;
	}

	public string tag{
		get{ return entry_type; }
	}

	public string entry{
		get { return entry_information; }
	}

	public Date_Time time{
		get { return entry_time; }
	}

	public string _user{
		get{return user;}
	}

	public string _target{
		get{ return target; }
	}
}


