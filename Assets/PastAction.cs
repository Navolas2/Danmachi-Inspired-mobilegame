using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PastAction
{
	private Attack_Action the_action;
	private CombatAction comb_action;
	private float Action_Code;
	private bool success;
	private bool Completed;
	private float damage;
	private string Target_Name;
	private int attack_sound;

	public PastAction(){
		Action_Code = -4f;
		success = true;
		Completed = true;
		damage = 0f;
		Target_Name = "none";
		attack_sound = 0;
	}

	public PastAction (Attack_Action action, float code, bool worked, float damage, int volume, Unit target)
	{
		the_action = action;
		Action_Code = code;
		success = worked;
		this.damage = damage;
		Target_Name = target.name;

		if (code == -3.5) {
			Completed = false;
		} else {
			Completed = true;
		}

	}


	public PastAction (CombatAction action, float code, bool worked, float damage, int volume, Unit target)
	{
		comb_action = action;
		Action_Code = code;
		success = worked;
		this.damage = damage;
		Target_Name = target.name;

		if (code == -3.5) {
			Completed = false;
		} else {
			Completed = true;
		}

	}
	public float code{
		get{ return Action_Code; }
	}

	public bool worked{
		get { return success; }
	}

	public float _damage{
		get { return damage; }
	}

	public bool finished{
		get { return Completed; }
	}

	public Attack_Action action{
		get{ return the_action; }
	}

	public CombatAction combat {
		get { return comb_action; }
	}

	public string target{
		get{ return Target_Name; }
	}

	public int Volume{
		get{ return attack_sound; }
	}
}


