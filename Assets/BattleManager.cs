using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour {


	private List<List<DungeonUnit>> Engaged_Parties;
	private DungeonLog battle_log;
	private float x_coord;
	private float y_coord;
	private float z_coord;
	private float floor;
	private bool initialized = false;

	// Use this for initialization
	void Start () {
		if (!initialized) {
			Initalize ();
		}
	}

	public void Initalize(){
		Engaged_Parties = new List<List<DungeonUnit>> ();
		//Adventurer this_adventurer = new Adventurer ();
		//Engaged_Parties.Add (new List<DungeonUnit> (){ this_adventurer.explorer });
		battle_log = new DungeonLog ("BATTLE");
		initialized = true;
	}

	// Update is called once per frame
	void Update () {
		if (Engaged_Parties.Count > 1 && GameClock.The_Clock.TimeBasedUpdate(.2)) {
			CombatRound ();
		} else if(Engaged_Parties.Count < 2 ) {
			if (Engaged_Parties.Count == 1) {
				UpdateDungeonLogs ();
				Dungeon.The_Dungeon.ReturnToExploring (Engaged_Parties [0], this);
			}
			Destroy (this.gameObject);

		}
	}

	private void UpdateDungeonLogs(){
		List<DungeonUnit> remaining = Engaged_Parties [0];
		foreach (DungeonUnit du in remaining) {
			if (du.attached_unit._type == Adventurer.Type_Adventurer) {
				((Adventurer)du.attached_unit).diary.AddEntries (battle_log);
			}
		}
	}

	public void AddUnitToBattle(Unit newEncounter)
	{
		if (newEncounter._type == Monster.type_Monster) {
			Monster mon = (Monster)newEncounter;
			if (mon != null) {
				if (mon._group && mon._aggressive) {
					int index = Engaged_Parties.FindIndex (delegate(List<DungeonUnit> obj) {
						return -1 < obj.FindIndex (delegate(DungeonUnit obj2) {
							return obj2.attached_unit.name.Equals (mon.name);
						});
					});
					if (index > -1) {
						Engaged_Parties [index].Add (mon.explorer);
					}
					else {
						Engaged_Parties.Add (new List<DungeonUnit> (){ newEncounter.explorer });
					}
				} else if (mon._group && !mon._aggressive) {
					int index = Engaged_Parties.FindIndex (delegate(List<DungeonUnit> obj) {
						return -1 < obj.FindIndex (delegate(DungeonUnit obj2) {
							if (obj2.attached_unit._type == Monster.type_Monster) {
								Monster m = (Monster)obj2.attached_unit;
								return !m._aggressive;
							}
							return obj2.attached_unit.name.Equals (mon.name);
						});
					});
					if (index > -1) {
						Engaged_Parties [index].Add (mon.explorer);
					}
					else {
						Engaged_Parties.Add (new List<DungeonUnit> (){ newEncounter.explorer });
					}
				} else {
					Engaged_Parties.Add (new List<DungeonUnit> (){ mon.explorer });
				}
				//Engaged_Enemies.Add (mon);
				//focused_enemy = mon;
			}
		} 
		else {
			int index = Engaged_Parties.FindIndex (delegate(List<DungeonUnit> obj) {
				return -1 < obj.FindIndex (delegate(DungeonUnit obj2) {
					if (obj2.attached_unit._type == Adventurer.Type_Adventurer) {
						Adventurer m = (Adventurer)obj2.attached_unit;
						//If Adventurer is a dick they might fight others
						return true;
					}
					return false;
				});
			});
			if (index > -1) {
				Engaged_Parties [index].Add (newEncounter.explorer);
			} else {
				Engaged_Parties.Add (new List<DungeonUnit> (){ newEncounter.explorer });
			}
		}
	}

	private void RemoveUnit(DungeonUnit du){
		for (int i = 0; i < Engaged_Parties.Count; i++) {
			if (Engaged_Parties [i].Exists (delegate(DungeonUnit obj) {
				return obj == du;
			})) {
				Engaged_Parties [i].Remove (du);
				if (Engaged_Parties [i].Count == 0) {
					Engaged_Parties.RemoveAt (i);
				}
				i = Engaged_Parties.Count;
			}
		}
	}

	private List<DungeonUnit> GetAllyList(DungeonUnit du){
		List<DungeonUnit> allies = new List<DungeonUnit> ();
		for (int i = 0; i < Engaged_Parties.Count; i++) {
			if (Engaged_Parties [i].Exists (delegate(DungeonUnit obj) {
				return obj == du;
			})) {
				for (int j = 0; j < Engaged_Parties [i].Count; j++) {
					if (Engaged_Parties [i] [j] != du) {
						allies.Add (Engaged_Parties [i] [j]);
					}
				}
			}
		}
		return allies;
	}

	private List<DungeonUnit> GetEnemyList(DungeonUnit du){
		List<DungeonUnit> enemies = new List<DungeonUnit> ();
		for (int i = 0; i < Engaged_Parties.Count; i++) {
			if (! Engaged_Parties [i].Exists (delegate(DungeonUnit obj) {
				return obj == du;
			})) {
				for (int j = 0; j < Engaged_Parties [i].Count; j++) {
					enemies.Add (Engaged_Parties [i] [j]);
				}
			}
		}
		return enemies;
	}

	private void distributeExcelia(DungeonUnit d)
	{
		float statCount = d.attached_unit.GetStatTotal ();
		foreach (List<DungeonUnit> DuL in Engaged_Parties) {
			foreach (DungeonUnit du in DuL) {
				if (du.attached_unit._type == Adventurer.Type_Adventurer) {
					((DungeonAdventurer)du).gainExcel (du.attached_unit.GetStatTotal (), statCount);
				}
			}
		}
	}

	private void CombatRound()
	{
		//SOLVE INITIATIVE. SEPERATE LATER
		//int initiativedex = this_adventurer.dex + (int)Random.range(this_adventurer.skill / 4 , this_adventurer.skill);
		List<DungeonUnit> turns = InitativeOrder();

		for (int i = 0; i < turns.Count; i++) {
			if (turns [i].attached_unit.hp <= 0) {
				distributeExcelia (turns [i]);
				RemoveUnit (turns [i]);
				turns.RemoveAt (i);
				i--;
			}
			else if (turns [i].attached_unit._type == Adventurer.Type_Adventurer) {
				AdventurerTurn (turns[i]);
				if (turns[i].target.attached_unit.hp <= 0) {
					battle_log.AddEntry (DungeonLog.ENTRY_TYPE_KILL, turns[i].target.attached_unit.name + " was killed by " + turns[i].attached_unit.name, turns[i].target.attached_unit._type, turns[i].attached_unit._type);
					//THIS CODE LEFT FOR TESTING DELETE LATER
					turns [i].target = null;
					print ("Monster was killed");

				}
			} 
			else if(turns[i].attached_unit._type == Monster.type_Monster){
				MonsterTurn (turns [i]); //Convert from unit to Monster
				if (turns[i].target.attached_unit.hp <= 0) {
					battle_log.AddEntry (DungeonLog.ENTRY_TYPE_KILL, turns[i].target.attached_unit.name + " was killed by " + turns[i].attached_unit.name, turns[i].target.attached_unit._type, turns[i].attached_unit._type);
					//data log and body left in dungeon until can be recovered later
					turns [i].target = null;
					print ("Adventurer has died");
				}
				if (turns [i].attached_unit.hp <= 0) {
					battle_log.AddEntry (DungeonLog.ENTRY_TYPE_KILL, turns[i].attached_unit.name + " was killed by " + turns[i].target.attached_unit.name, turns[i].target.attached_unit._type, turns[i].attached_unit._type);
					if (turns [i].target.target == turns [i]) {
						turns [i].target.target = null;
					}
					print ("Monster was killed by a counter");
				}
			}
			turns [i].attached_unit.StatusAilment ();
			turns [i].attached_unit.RemoveEndedStatus ();
		}

		//Double check to make sure everything still in the list is alive
		for (int i = 0; i < turns.Count; i++) {
			if (turns [i].attached_unit.hp <= 0) {
				distributeExcelia (turns [i]);
				RemoveUnit (turns [i]);
				turns.RemoveAt (i);
				i--;
			}
		}

	}

	private List<DungeonUnit> InitativeOrder(){
		List<DungeonUnit> turns = new List<DungeonUnit> ();
		foreach (List<DungeonUnit> dl in Engaged_Parties) {
			foreach (DungeonUnit du in dl) {
				turns.Add (du);
			}
		}

		turns.Sort (delegate(DungeonUnit left, DungeonUnit right) {
			return (int)(Mathf.RoundToInt(right.attached_unit.agi) - Mathf.RoundToInt(left.attached_unit.agi));
		});

		return turns;
	}

	private void MonsterTurn (DungeonUnit Monster_Unit) //change unit to Monster later
	{
		Monster Monster = (Monster)Monster_Unit.attached_unit;
		DungeonUnit target_unit = Monster_Unit.target;

		//float adv_count = this_adventurer.GetStatTotal();
		//float mon_count = Monster.GetStatTotal ();
		Monster.DetermineAttack(target_unit, GetAllyList(Monster_Unit), GetEnemyList(Monster_Unit), out target_unit);
		Unit target = target_unit.attached_unit;
		Monster_Unit.target = target_unit;

		if (target_unit.target == null) {
			target_unit.target = Monster_Unit;
		}

		int Herodefense = 0;
		Attack_Action response = new Attack_Action ();
		bool isResponse = false;
		int[] stat = new int[0];

		target.Defensive_Action (target, Monster, (target_unit.target.Equals(Monster_Unit)), true, out stat, out isResponse, out response, 0, out Herodefense, false, 1);
		target.attachAction (target.LastDefense);
		if (target._type == Adventurer.Type_Adventurer) {
			//Increase stat
			((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), Monster.GetStatTotal(), stat);
		}
		
		while (isResponse) {
			target.Offensive_Action = response;
			int ResponseDefense = 0;
			int[] responseStat = new int[0];
			response.action(target, Monster, true, true, out responseStat, out isResponse, out response, ResponseDefense, out ResponseDefense, false, 1);
			target.attachAction (target.LastOffense);
			if (target._type == Adventurer.Type_Adventurer) {
				//Increase stat
				((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), Monster.GetStatTotal(), stat);
			}
				
		}
		if (Herodefense == 0 || Herodefense == 1 && Monster.hp > 0) {
			
			int attackDefense = 0;
			Monster.Offensive_Action.action (target, Monster, (target_unit.target.Equals(Monster_Unit)), true, out stat, out isResponse, out response, Herodefense, out attackDefense, false, 2);

			Monster.attachAction (Monster.LastOffense);

			if (target._type == Adventurer.Type_Adventurer) {
				//Increase stat
				((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), Monster.GetStatTotal(),stat);
			}

			while (isResponse) {
				Monster.Offensive_Action = response;
				int ResponseDefense = 0;

				response.action(target, Monster, true, true, out stat, out isResponse, out response, ResponseDefense, out ResponseDefense, false, 2);
				Monster.attachAction (Monster.LastOffense);

				if (target._type == Adventurer.Type_Adventurer) {
					//Increase stat
					((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), Monster.GetStatTotal(),stat);
				}
			}
		} else {
			Monster.attachAction (Monster.Offensive_Action, HeroCombatActions.HeroCombat.QuickCall (target, Monster, 2, false, true, Monster.Offensive_Action.action), 
				false, 0, Monster);
		}
	}

	private void AdventurerTurn (DungeonUnit Adventuer_unit){

		Adventurer currentAdventuer = (Adventurer)Adventuer_unit.attached_unit;
		DungeonUnit target_unit = Adventuer_unit.target;

		//Decide ACTION
		currentAdventuer.DetermineAttack(target_unit, GetAllyList(Adventuer_unit), GetEnemyList(Adventuer_unit), out target_unit);
		Unit target = target_unit.attached_unit;
		Adventuer_unit.target = target_unit;

		float adv_count = currentAdventuer.GetStatTotal();
		float mon_count = target.GetStatTotal ();

		//print(currentAdventuer.Offensive_Action.ActionMessage);
		//Check skills
		int Mondefense = 0;
		Attack_Action response = new Attack_Action();
		bool isResponse = false;
		int[] stat = new int[0];
		target.Defensive_Action (currentAdventuer, target, true, true, out stat, out isResponse, out response, 0, out Mondefense, false, 2);

		target.attachAction (target.LastDefense);

		if (target._type == Adventurer.Type_Adventurer) {
			//Increase stat
			((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), currentAdventuer.GetStatTotal(), stat);
		}


		while (isResponse) {
			target.Offensive_Action = response;
			int ResponseDefense = 0;
			int[] response_stats = new int[0];
			response.action(currentAdventuer, target, true, true, out response_stats, out isResponse, out response, ResponseDefense, out ResponseDefense, false, 2);

			target.attachAction (target.LastOffense);
			if (target._type == Adventurer.Type_Adventurer) {
				//Increase stat
				((DungeonAdventurer)target_unit).IncreaseStat (target.GetStatTotal(), currentAdventuer.GetStatTotal(), response_stats);
			}
		}

		int attackDefense = 0;
		currentAdventuer.Offensive_Action.action (currentAdventuer, target, true, true, out stat, out isResponse, out response, Mondefense, out attackDefense, false, 1);

		currentAdventuer.attachAction (currentAdventuer.LastOffense);
		

		///IncreaseStat (damage, adv_count, mon_count,stat - 1);
		((DungeonAdventurer)Adventuer_unit).IncreaseStat (target.GetStatTotal(),target.GetStatTotal(),stat);

		while (isResponse) {
			currentAdventuer.Offensive_Action = response;
			int ResponseDefense = 0;
			response.action(currentAdventuer, target, true, true, out stat, out isResponse, out response, ResponseDefense, out ResponseDefense, false, 1);

			currentAdventuer.attachAction (currentAdventuer.LastOffense);
			
			//IncreaseStat (damage, adv_count, mon_count,stat - 1);
			((DungeonAdventurer)Adventuer_unit).IncreaseStat (target.GetStatTotal(),target.GetStatTotal(),stat);
		}

		if (target.hp <= 0) {
			DungeonAdventurer d = (DungeonAdventurer)Adventuer_unit;
			d.gainExcel (adv_count, mon_count);
		}
				
	}

		
}
