using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonAdventurer : DungeonUnit
{
	private List<float> statBoosts;
	private float exp_gain;
	private float excelia_gain;
	private Goal currentGoal;
	private bool completion_recorded;

	public DungeonAdventurer (Unit u)
	{
		me = u;
		u.SetDungeonUnit (this);
		currentGoal = null;
		completion_recorded = false;

		statBoosts = new List<float>(){0f, 0f, 0f, 0f, 0f};
		exp_gain = 0f;
		excelia_gain = 0f;
	}

	public bool IsGoalComplete(){
		if (!completion_recorded) {
			bool out_status = currentGoal.CheckCompletionSingle ((Adventurer)attached_unit);
			if (out_status && !completion_recorded) {
				SetTargetLocation (new coordinate (0));
				((Adventurer)attached_unit).diary.AddEntry (DungeonLog.ENTRY_TYPE_EVENT, "Goal was completed", attached_unit.name, "self");
				completion_recorded = true;
			}
			return out_status;
		} else {
			return true;
		}
	}

	public List<float> getIncrease(){
		List<float> out_stats = statBoosts;
		out_stats.Add (exp_gain);
		out_stats.Add (excelia_gain);

		//Reset stats to their base value
		statBoosts = new List<float>(){0f, 0f, 0f, 0f, 0f};
		exp_gain = 0f;
		excelia_gain = 0f;

		return out_stats;
	}

	public void setGoal(Goal g){
		currentGoal = g;
		completion_recorded = false;
	}

	public void gainExcel(float adv_count, float mon_count)
	{
		if (adv_count >= mon_count) {
			excelia_gain += .5f;
		} else if (mon_count - adv_count < 400) {
			excelia_gain += 1f;
		} else {
			excelia_gain += 5f;
		}
	}

	public void IncreaseStat (float adv_count, float mon_count, int[] index_array)
	{
		float statDiff = adv_count - mon_count;

		float skill = 0;
		for (int i = 0; i < index_array.Length; i++) {
			float BoostStat = 0f;
			int index = index_array [i];
			if (index != -1) {
				if (statDiff > 200) {
					BoostStat = .1f;
					skill += .05f;
				} else if (statDiff >= -100) {
					BoostStat = .05f;
					skill += .025f;
				} else if (statDiff <= -100) {
					BoostStat = 0f;
				}

				statBoosts [index] = statBoosts [index] + BoostStat;
				exp_gain += skill;
			}
		}
	}

	public Goal goal{
		get{ return currentGoal; }
	}

	public override void MoveFloor ()
	{
		if (location._floor == 0) {
			completion_recorded = false;
		}
		base.MoveFloor ();
	}

	public override bool MovePosition ()
	{
		rounds_exploring++;
		rounds_on_current_floor++;
		rounds_in_current_room++;
		Room currentRoom = Dungeon.The_Dungeon.GetFloor (location._floor).GetCurrentRoom (location._room);
		LearnedRoom (currentRoom);
		int direction = 0;
		int floor = location._floor;
		List<string[]> heats;
		List<char> tiles;
		List<int> moveable = currentRoom.MoveableDirections (location, out heats, out tiles);
		if(target_location != null){
			
			if (target_location.IsLocation (location) && !target_location.FloorOnly ()) {
				//No movement 
			} else if (target_location._room == location._room) {
				do {
					direction = RandomMovement(moveable);
					if(tiles[direction - 1] == 'D'){
						direction = 0;
					}
				} while(direction == 0);
			}
			else {
				string direction_code = "";
				bool knownPath = false;
				if (target_location._floor > location._floor) {
					direction_code = currentRoom.DoorToExit (location);
					currentRoom.RemoveDoorway (moveable, tiles, location, 7);
					//aim to get to lower floor
					if (((Adventurer)attached_unit).ClearedFloor (location._floor)) {
						knownPath = true;
					}
				} else if (target_location._floor < location._floor) {
					direction_code = currentRoom.DoorToEntrance (location);
					currentRoom.RemoveDoorway (moveable, tiles, location, 8);
					if (((Adventurer)attached_unit).ClearedFloor (location._floor) || rounds_on_current_floor < 20) {
						knownPath = true;
					}
				} else if (known_rooms.Contains (currentRoom._Individual_ID)) {
					knownPath = true;
				}
				heats = AdjustHeat (heats, direction_code);

				if (knownPath) {
					int lowest_Heat = int.MaxValue;
					string lowest_code = "@";
					foreach (int move in moveable) {
						int check_heat = int.Parse(heats [move - 1][0]);
						if (check_heat <= -1) {
							check_heat = int.MaxValue;
						}
						if (heats [move - 1] [1].Contains (direction_code)) {
							if (lowest_code == direction_code) {
								if (check_heat < lowest_Heat) {	
									direction = move;
									lowest_Heat = check_heat;
									lowest_code = direction_code;
								} else if (check_heat == lowest_Heat) {
									if (Random.value > .5) {
										direction = move;
									}
								}
							} else {
								direction = move;
								lowest_Heat = check_heat;
								lowest_code = direction_code;
							}
						} else if(lowest_code != direction_code) {
							if (check_heat < lowest_Heat) {	
								direction = move;
								lowest_Heat = check_heat;
								lowest_code = heats [move - 1] [1];
							} else if (check_heat == lowest_Heat) {
								if (Random.value > .5) {
									direction = move;
									lowest_code = heats [move - 1] [1];
								}
							}
						}
					}
				}
				else {
					//Random movement
					direction = RandomMovement(moveable);
				}
			}

		}
		else {
			//Random movement
			direction = RandomMovement(moveable);
		}
		location.Move(direction);
		Last_move.Add(direction);
		if (direction == 0) {
			return false;
		}
		return true;
	}

	public void SpawnReaction(List<Monster> mons){
		//TODO fill out reaction to a Monster Spawning in room
		if (mons.Count > 0) {

		}
	}
}


