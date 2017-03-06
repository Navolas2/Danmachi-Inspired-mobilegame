using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class Status
{
	private List<float> Stat_Mod;
	private float healthAdjust;
	private int hp_type; //0 is none, 1 is percent, 2 is value
	private int lenght;
	private List<string> elements;

	public Status (List<float> stats, float health, int type, int len, List<string> elem)
	{
		Stat_Mod = stats;
		healthAdjust = health;
		hp_type = type;
		lenght = len;
		elements = elem;
	}

	public Status (Status orig)
	{
		Stat_Mod = orig.Stat_Mod;
		healthAdjust = orig.healthAdjust;
		hp_type = orig.hp_type;
		lenght = orig.lenght;
		elements = orig.elements;
	}

	public List<float> statChanges{
		get{ return Stat_Mod; }
	}

	public List<string> _Elements {
		get{ return elements; }
	}


	public float changeHP (float hp, float max_hp){
		if (hp_type == 1) {
			hp += (max_hp * healthAdjust);
		}
		if (hp_type == 2) {
			hp += healthAdjust;
		}
		return hp;
	}

	public void decrament(){
		lenght--;
	}

	public int _length{
		get{ return lenght; }
	}

	public bool isFinished(){
		return this.lenght == 0;
	}

	public static Status parseNode(XmlNode statusNode){
		List<float> stats = new List<float> ();
		foreach (XmlNode statReq in statusNode.SelectNodes("stats")) {
			float add_float = 0f;
			float.TryParse (statReq.InnerText, out add_float);
			stats.Add (add_float);
		}
		float health = 0f;
		float.TryParse (statusNode.SelectSingleNode ("health").InnerText, out health);

		int hp_type = 0;
		int.TryParse (statusNode.SelectSingleNode ("type").InnerText, out hp_type);

		int len = 0;
		int.TryParse (statusNode.SelectSingleNode ("length").InnerText, out len);

		List<string> elems = new List<string> ();
		foreach (XmlNode link_skill in statusNode.SelectNodes("element")) {
			elems.Add (link_skill.InnerText);
		}

		return new Status (stats, health, hp_type, len, elems);
	}
}


