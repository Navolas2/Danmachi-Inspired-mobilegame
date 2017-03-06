using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void SpellEffect();
/*SpellEffect explanation
Enacts any special effects that a spell has. 
No more explanation at this time
*/

public class Spell
{
	public static string Spell_Type_Buff = "BUFF";
	public static string Spell_Type_Heal = "HEAL";
	public static string Spell_Type_Offense = "OFFENSE";
	public static string Spell_Type_Defense = "DEFENSE";
	private static List<string> types = new List<string>(){Spell_Type_Buff, Spell_Type_Offense, Spell_Type_Defense};

	public static string Affinity_None = "NONE";
	public static string Affinity_Fire = "FIRE";
	public static string Affinity_Earth = "EARTH";
	public static string Affinity_Wind = "WIND";
	public static string Affinity_Water = "WATER";
	public static string Affinity_Elec = "ELEC";
	public static string Affinity_Light = "LIGHT";
	public static string Affinity_Shadow = "SHADOW";
	private static List<string> affinities = new List<string>(){Affinity_None, Affinity_Fire,
																Affinity_Earth, Affinity_Wind,
																Affinity_Water, Affinity_Elec,
																Affinity_Shadow, Affinity_Light};

	private string name;
	private int speed;
	private float hit;
	private float damage;
	private float focus;
	private int level;
	private float cost;
	private bool singleTarget;
	private bool self;
	private string type = Spell_Type_Offense;
	private string affinity = Affinity_None;


	private SpellEffect effect;

	public Spell (string name_, int speed_, float hit_, float damage_, float focus_, float cost_, bool self)
	{
		name = name_;
		speed = speed_;
		hit = hit_;
		damage = damage_;
		focus = focus_;
		cost = cost_;
		singleTarget = true;
		effect = Spell_Effects.effects.NoEffect;
		this.self = self;
	}

	public Spell (string name_, int speed_, float hit_, float damage_, float focus_, string effect_, int level_, float cost_, string affinity_, string type_, bool self)
	{
		name = name_;
		speed = speed_;
		hit = hit_;
		damage = damage_;
		focus = focus_;
		level = level_;
		cost = cost_;
		singleTarget = true;
		effect = Spell_Effects.effects.GetEffect(effect_);
		affinity = ValidAffinity(affinity_);
		type = ValidType (type_);
		this.self = self;
	}

	public Spell (string name_, int speed_, float hit_, float damage_, float focus_, SpellEffect effect_, float cost_, string affinity_, string type_, bool self)
	{
		name = name_;
		speed = speed_;
		hit = hit_;
		damage = damage_;
		focus = focus_;
		effect = effect_;
		cost = cost_;
		singleTarget = true;
		affinity = ValidAffinity(affinity_);
		type = ValidType (type_);
		this.self = self;
	}

	private string ValidAffinity(string affinity)
	{
		if (!affinities.Contains (affinity)) {
			return Affinity_None;
		}
		return affinity;
	}

	private string ValidType(string type)
	{
		if (!types.Contains (type)) {
			return Spell_Type_Offense;
		}
		return type;
	}

	public string _name{
		get{return name;}
	}

	public float _cost {
		get{ return cost; }
	}

	public float _speed{
		get{ return speed;}
	}

	public float _hit{
		get{ return hit;}
	}

	public float _damage{
		get{ return damage; }
	}

	public float _focus{
		get{ return focus; }
	}

	public int _level{
		get{ return level; }
	}

	public string _afffinity {
		get{ return affinity; }
	}

	public bool single_target{
		get{ return singleTarget; }
		set{ singleTarget = value; }
	}

	public string _type {
		get{ return type; }
	}

	public bool _self {
		get{ return self; }
	}
}
	
public class Spell_Effects
{
	/*Spell Effects list. Data call and brief summary
	 * NONE : No effect at all
	 * 
	 */
	public static Spell_Effects effects = new Spell_Effects();

	private Spell_Effects()
	{
		if (effects == null) {
			effects = this;
		}
	}

	public SpellEffect GetEffect(string effect_name)
	{
		//Switch based on name... or something...
		if (effect_name.Equals ("NONE")) {
			return NoEffect;
		}
		return NoEffect;
	}

	public void NoEffect(){
		//Nothing happens
	}
}
