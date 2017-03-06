using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CastingSpell : Attack_Action
{
	Spell Spell_To_Cast;
	private float castTime;
	private float focus;

	public CastingSpell (Spell castSpell, float magic)
	{
		CODE = -3.5f;
		delay = 0;
		attack_volume = 2;
		Spell_To_Cast = castSpell;
		castTime = Spell_To_Cast._speed;
		focus = Spell_To_Cast._focus + (magic / 2);
		this_action = Prepare_Spell;
		SingleTarget = Spell_To_Cast.single_target;
		cost = Spell_To_Cast._cost;
		affinities = new List<string> ();
		affinities.Add (castSpell._afffinity);
	}

	public override float damage (Unit user)
	{
		if (castTime == 0) {
			return Spell_To_Cast._damage + user.mag;
		} else {
			return 0; //The attack should not be dodgable
		}
	}

	public override float hit (Unit user)
	{
		if (castTime == 0) {
			return Spell_To_Cast._hit + user.mag;
		} else {
			return float.MaxValue; //When casting the action should be unavoidable 
		}
	}

	public float cast{
		get{return castTime; }
	}

	public float damageFocus{
		set{focus -= value * .25f; }
	}

	//THIS IS THE ONLY FUNCTION THAT SHOULD BE CALLED!
	public float Prepare_Spell(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Adventurer hero = (Adventurer)Position_One;
		Unit Target = Position_Two;
		if (You_are == 1) { hero = (Adventurer)Position_One; Target = Position_Two;}
		if (You_are == 2) {	hero = (Adventurer)Position_Two; Target = Position_One;}
		//Temporary out settings
		stat = new int[0];
		isResponse = false;
		response = new Attack_Action();
		defending = 0;

		if (!active && self) {
			return -3.5f;
		} else if (!active) {
			return float.MaxValue;
		}

		//Check to see if you can cast the spell. Otherwise reduce the spell count
		if ((castTime > 0.0f) && (this.focus > 0.0f)) {
			castTime--;
			hero.LastOffense = new PastAction(Prepare_Spell, -3.5f, true, 0, attack_volume, hero);
			hero.diary.AddEntry(DungeonLog.ENTRY_TYPE_COMBAT, hero.name + " is preparing to cast " + Spell_To_Cast._name + "!", hero.name, "self");
		} else if (this.focus < 0.0f) {
			SpellFailure (hero, Target, focus, active, out stat, out isResponse, out response, defended, out defending, self, You_are);
		}else {
			castTime = Spell_To_Cast._speed;
			CastSpell (hero, Target, focus, active, out stat, out isResponse, out response, defended, out defending, self, You_are);
		}

		return 0f;
	}
		
	public float CastSpell(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Adventurer hero = (Adventurer)Position_One;
		Unit TheMon = Position_Two;
		if (You_are == 1) { hero = (Adventurer)Position_One; TheMon = Position_Two;}
		if (You_are == 2) {	hero = (Adventurer)Position_Two; TheMon = Position_One;}
		//Temporary out settings
		stat = new int[0];
		isResponse = false;
		response = new Attack_Action ();
		defending = 0;
		CODE = -3;

		if (!active && self) {
			return CODE;
		} else if (!active && !focus) {
			return hero.mag * Spell_To_Cast._hit;
		} else if (!active && focus) {
			return hero.mag * Spell_To_Cast._damage;
		}
		hero._mana = hero._mana - cost;
		castTime = Spell_To_Cast._speed;
		if (!Spell_To_Cast._self) {
			if (defending != 2) {
				//Cast the spell
				float damage = (hero.mag + hero.statBuff ()) + Spell_To_Cast._damage;
				damage = damage - TheMon.mag;

				TheMon.TakeDamage (damage, affinities);
				//Yout can't block magic!
				//Apply spell effect 
				stat = new int[1]{5};
				//Any sort of response
				hero.LastOffense = new PastAction (CastSpell, -3f, damage > 0, damage, attack_volume, hero);

				hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_COMBAT, hero.name + " casts " + Spell_To_Cast._name + "!", hero.name, TheMon.name);
				hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_COMBAT, Spell_To_Cast._name + " hits " + TheMon.name + " for " + damage + " damage!", hero.name, TheMon.name);
				return damage;
			} else {
				hero.LastOffense = new PastAction (CastSpell, -3f, false, 0f, attack_volume, hero);
				hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_COMBAT, hero.name + " casts " + Spell_To_Cast._name + "!", hero.name, TheMon.name);
				hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_COMBAT, hero.name + "'s attack was dodged!", hero.name, TheMon.name);
				return 0f;
			}
		} else {
			return 0f;
		}
	}

	//THIS SHOULD NEVER BE USED AS AN ATTACK. ONLY USED AGAINST THE HERO WHEN THEY LOSE FOCUS OF THEIR MAGIC AND IT FAILS
	public float SpellFailure(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Adventurer hero = (Adventurer)Position_One;
		if (You_are == 1) { hero = (Adventurer)Position_One;}
		if (You_are == 2) {	hero = (Adventurer)Position_Two;}
		//Temporary out settings
		stat = new int[0];
		isResponse = false;
		response = new Attack_Action ();
		defending = 0;
		CODE = -3;

		if (!active && self) {
			return CODE;
		} else if (!active && !focus) {
			return float.MaxValue;
		} else if (!active && focus) {
			return Spell_To_Cast._damage * .75f;
		}

		//Spell backfiring
		//Output: hero.name 's spell backfired damaging them for damage
		hero.TakeDamage (Spell_To_Cast._damage * .75f, affinities);
		hero.LastOffense = new PastAction(SpellFailure, -3, true, -1f * Spell_To_Cast._damage * .75f, 4, hero);
		hero.diary.AddEntry(DungeonLog.ENTRY_TYPE_COMBAT, hero.name + " loses control of the magic and takes " + Spell_To_Cast._damage * .75f + " damage!", hero.name, "self");

		return 0f;
	}
}


