using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack_Action
{
	protected CombatAction this_action;
	protected string name;
	protected List<float> hit_chance; //List of multipliers and bonus. Stats in normal order
	protected List<float> damage_power; //List of multipliers and bonus
	protected int[] stats;
	protected float CODE;
	protected bool SingleTarget;
	protected string message;
	protected List<string> affinities;
	protected int delay;
	protected float cost;
	protected int attack_volume;
	protected bool has_skill = false;
	protected Skill related_skill = null;
	protected bool has_status = false;
	protected Status related_status = null;
	public bool useable = true;

	public Attack_Action ()
	{/*Shouldn't really be used*/}

	public Attack_Action(string code)
	{
		if (code.Equals ("none")) {
			name = "none";
			message = "Nothing";
			hit_chance = new List<float>(){0f,0f,0f,0f,0f,0f};
			damage_power = new List<float>(){0f,0f,0f,0f,0f,0f};
			CODE = -4f;
			SingleTarget = true;
			affinities = new List<string>(){Spell.Affinity_None};
			delay = 0;
			this.cost = 0;
			stats = new int[0];
			attack_volume = 0;
		}
	}

	public Attack_Action(string nm, List<float> hit, List<float> damage, float code, bool s_tar, List<string>affinity, int pause, float cost, string message, bool stat, int volume)
	{
		name = nm;
		hit_chance = hit;
		damage_power = damage;
		CODE = code;
		SingleTarget = s_tar;
		affinities = affinity;
		delay = pause;
		this.cost = cost;
		this.message = message;
		has_status = stat;
		stats = new int[]{ 0 };
		attack_volume = volume;
	}

	public Attack_Action (Attack_Action orig){
		name = orig.name;
		hit_chance = orig.hit_chance;
		damage_power = orig.damage_power;
		CODE = orig.CODE;
		SingleTarget = orig.SingleTarget;
		affinities = orig.affinities;
		delay = orig.delay;
		this.cost = orig.cost;
		has_skill = orig.has_skill;
		related_skill = orig.related_skill;
		message = orig.message;
		stats = orig.stats;
		attack_volume = orig.attack_volume;
	}
		
	public void Attached_to_whom(int user){
		if (user == 1) { //This combat is for a hero
			this_action = HeroCombatActions.HeroCombat.Attack;
		}
		if (user == 2) {
			this_action = MonsterCombatActions.MonsterCombat.Attack;
		}
	}

	public void Attach_Status(Status s)
	{
		related_status = s;
	}

	public virtual float hit(Unit user){
		float hit = 0f;
		hit += user.str * hit_chance [0];
		hit += user.def * hit_chance [1];
		hit += user.dex * hit_chance [2];
		hit += user.agi * hit_chance [3];
		hit += user.mag * hit_chance [4];
		hit += hit_chance [5];
		return hit;
	}

	public virtual float damage(Unit user){
		float damage = 0f;
		damage += user.str * damage_power [0];
		damage += user.def * damage_power [1];
		damage += user.dex * damage_power [2];
		damage += user.agi * damage_power [3];
		damage += user.mag * damage_power [4];
		damage += damage_power [5];
		return damage;
	}

	public float code {
		get{ return CODE; }
	}

	public Skill related{
		get{ return related_skill; }
		set{
			has_skill = true;
			related_skill = value;
		}
	}

	public bool HasStatus{
		get{ return has_status; }
	}

	public Status status{
		get{ return related_status; }
	}

	public CombatAction action {
		get{ return this_action; }
	}

	public string ActionMessage{
		get{ return message; }
		set{ message = value; }
	}

	public List<string> elements {
		get{ return affinities; }
	}

	public float _cost{
		get{ return this.cost; }
	}

	public int[] StatGains {
		get { return this.stats; }
		set { stats = value; }
	}

	public int volume{
		get { return attack_volume; }
	}
}


