using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Skill
{
	//SKILL TYPES. a Skill can have more than one type
	public static string type_counter = "COUNTER";
	public static string type_offense = "OFFENSE";
	public static string type_defense = "DEFENSE";
	//public static string type_block = 	  "BLOCK";
	public static string type_passive = "PASSIVE";
	//public static string type_evade = 	  "EVADE";

	private float proc_chance;
	private float def_value;
	private List<String> Type;
	private string name;
	private bool hasAction;
	private Attack_Action the_action;
	private bool hasLink;
	private List<Skill> link_skills;
	private List<string> linked_skills;
	private bool custom = false;
	private List<float> stat_Req;
	private List<string> Skill_Req;
	private bool has_status;
	private Status Buff;
	private string description;

	public Skill(){
		//used for initializaiton
	}

	public Skill (float proc, int value, List<string> _type, string _name, bool action, bool link, List<float> stat, List<string> skill, bool status, string description)
	{
		proc_chance = proc;
		def_value = value;
		Type = _type;
		name = _name;
		hasAction = action;
		hasLink = link;
		stat_Req = stat;
		Skill_Req = skill;
		has_status = status;
		this.description = description;
	}

	public Skill (Skill orig)
	{
		proc_chance = orig.proc_chance;
		Type = orig.Type;
		name = orig.name;
		hasAction = orig.hasAction;
		the_action = orig.the_action;
		hasLink = orig.hasLink;
		linked_skills = orig.linked_skills;
		link_skills = orig.link_skills;
		stat_Req = orig.stat_Req;
		Skill_Req = orig.Skill_Req;
		has_status = orig.has_status;
		Buff = orig.Buff;

	}

	public void setAction(Attack_Action a_in){
		the_action = a_in;
		if (a_in != null) {
			a_in.related = this;
		}
	}

	public List<string> LinkedNames {
		get{ return linked_skills; }
		set{ linked_skills = value; }
	}

	public List<Skill> linkedSkills {
		get{ return link_skills; }
		set{ link_skills = value; }
	}

	public Status effects{
		get{ return Buff; }
		set{Buff = value;}
	}
}


