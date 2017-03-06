using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonUnit : DungeonReactable
{
	protected Unit me;

	protected coordinate location;
	protected coordinate target_location = null;
	protected int rounds_exploring = 0;
	protected int rounds_in_current_room = 0;
	protected int rounds_on_current_floor = 0;
	protected List<string> known_rooms;
	protected List<int> Last_move;
	public DungeonAction action;

	private DungeonUnit focused_enemy;

	public DungeonUnit(){
		known_rooms = new List<string> ();
		location = new coordinate ();
		Last_move = new List<int> ();
	}

	public DungeonUnit (Unit u)
	{
		me = u;
		u.SetDungeonUnit (this);
		location = new coordinate ();
		Last_move = new List<int> ();
	}

	public void SetFloorTarget(int tar){
		SetTargetLocation (new coordinate(tar));
	}

	public void SetTargetLocation(coordinate tar){
		target_location = tar;
	}

	public void ClearTarget(){
		target_location = null;
	}

	public virtual void DecideAction(){
		//TODO fill out
		action = Dungeon.The_Dungeon.GetFloor(location._floor).MoveUnit;
	}

	public virtual void MoveFloor(){
		rounds_on_current_floor = 0;
		MoveRoom ();
		if (location._floor == 0) {
			rounds_exploring = 0;
		}
	}

	public void MoveRoom(){
		rounds_in_current_room = 0;
	}

	public virtual bool MovePosition(){
		rounds_exploring++;
		rounds_on_current_floor++;
		rounds_in_current_room++;
		Room currentRoom = Dungeon.The_Dungeon.GetFloor (location._floor).GetCurrentRoom (location._room);
		LearnedRoom (currentRoom);
		int direction = 0;
		List<string[]> heats;
		List<char> tiles;
		List<int> moveable = currentRoom.MoveableDirections (location, out heats, out tiles);
		if(target_location != null){
			
			if (target_location.IsLocation (location) && !target_location.FloorOnly()) {
				//No movement 
			} else {
				string direction_code = "";
				bool knownPath = false;
				if (known_rooms.Contains (currentRoom._Individual_ID)) {
					knownPath = true;
				}

				if (target_location._floor > location._floor) {
					//aim to get to lower floor
					currentRoom.RemoveDoorway (moveable, tiles, location, 7);
					direction_code = currentRoom.DoorToExit (location);
				} 
				else if (target_location._floor < location._floor) {
					direction_code = currentRoom.DoorToEntrance (location);
					currentRoom.RemoveDoorway (moveable, tiles, location, 8);
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
							if (check_heat < lowest_Heat) {	
								direction = move;
								lowest_Heat = check_heat;
								lowest_code = direction_code;
							} else if (check_heat == lowest_Heat) {
								if (Random.value > .5) {
									direction = move;
								}
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
		
	protected void LearnedRoom(Room r){
		if (!known_rooms.Contains (r._Individual_ID)) {	
			if (rounds_in_current_room >= r._knownTime) {		
				known_rooms.Add (r._Individual_ID);
			}
		}
	}

	protected int RandomMovement(List<int> moveable){
		int choice = Random.Range(0, moveable.Count);
		int direction = moveable [choice];
		if (Last_move.Count > 0) {
			if ((direction - 2 == Last_move [Last_move.Count - 1] || direction + 2 == Last_move [Last_move.Count - 1]) ||
			   (direction == 5 && Last_move [Last_move.Count - 1] == 6) || (direction == 6 && Last_move [Last_move.Count - 1] == 5) && moveable.Count > 1) {
				if (Random.value < .85) {
					int lastDir = direction;
					while (lastDir == direction) {
						choice = Random.Range (0, moveable.Count);
						direction = moveable [choice];
					}
				}
			}
		}
		return direction;
	}

	protected List<string[]> AdjustHeat(List<string[]> heat_in, string code){
		for(int i = 0; i < heat_in.Count; i++) {
			string[] point = heat_in[i];
			if (!point [1].Contains (code)) {
				if (point [1].Contains ("@")) {
					int val = int.Parse (point [0]);
					val *= 3;
					point [0] = val.ToString ();
				} else {
					int val = int.Parse (point [0]);
					val *= 2;
					point [0] = val.ToString ();
				}
			
				heat_in [i] = point;

			}
		}
		int switcher = -1;
		List<int> rotate = new List<int> ();
		for (int i = 0; i < heat_in.Count; i++) {
			rotate.Add (i);
		}
		rotate = ShuffleList (rotate);

		for(int i = 0; i < heat_in.Count; i++) {
			string[] point = heat_in[rotate[i]];
			if (point [1].Contains (code)) {
				if (switcher == -1) {
					switcher = rotate[i];
				} else {
					//compare heat. if not same switch
					if (int.Parse (point [0]) != int.Parse (heat_in [switcher][0])) {
						heat_in [rotate [i]] = heat_in [switcher];
						heat_in [switcher] = point;
						switcher = -1;
					}
				}

			}
		}

		return heat_in;

	
	}

	public static List<int> ShuffleList(List<int> shuffler){
		for (int i = 0; i < shuffler.Count; i++) {
			int r = Random.Range (0, shuffler.Count);
			int c = shuffler [i];
			shuffler [i] = shuffler [r];
			shuffler [r] = c;
		}
		return shuffler;
	}

	private bool Match_Code(string one, string two){
		
		char[] codes = one.ToCharArray ();
		for (int i = 0; i < codes.Length; i++) {
			if (codes [i] != '0') {
				if (two.Contains (codes + "")) {
					return true;
				}
			}
		}
		return false;
	}

	public void RandomizeLocation(int layers, int x, int z){
		location.Randomize (layers, x, z);
	}

	public DungeonUnit target{
		get{ return focused_enemy; }
		set{ focused_enemy = value; }
	}

	public Unit attached_unit {
		get{return me;}
	}

	public coordinate _location {
		get { return location; }
		set{
			location = value;
			location.RandomShift ();
		}
	}

	public coordinate _location_goal{
		get{ return target_location; }
	}

	/*************************/
	//Dungeon Reactable Stuff//
	/************************/

	public string object_type(){
		return attached_unit._type;
	}

	public void SpaceReact (DungeonUnit du){
		attached_unit.Encounter (du);
	}

	public void SoundReact (DungeonReactable sound_source){
		attached_unit.HearSound (sound_source);
	}

	public int threat_sound {
		get{return attached_unit._type == Monster.type_Monster ? attached_unit._level : -2;}
	}

	public int sound_volume {
		get{ return attached_unit.LastOffense.Volume; }
	}
}


