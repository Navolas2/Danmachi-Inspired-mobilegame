using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

//FLOORLAYOUT KEY
// P - pathway
// R - room
// D - doorway
// W - Wall
// O - Open Air 
// H - hole (Only on floors 13+)
// F - pantry
// S - pathway up a layer
// s - pathway down a Layer
// U - pathway up a floor
// M - mist cuts down sight to 1 when standing on or next to.
// A - water. slows movement
// L - lava. inflicts damage if in
// Movement is allowed in a range of +/- .5 from center of section before changing segments

public delegate void DungeonAction(DungeonUnit d_unit);

public class Dungeon_Floor : MonoBehaviour
{
	

	private string floorName = "Floor ";
	public int floor_number;
	private int spawn_bursts; //defined when loaded
	private int min_monsters;
	private int EnterLocation;
	private int ExitLocation;

	private List<Room> Room_On_Floor;
	private List<List<List<int>>> RoomLayout; //Set up so that a floor can be multi_leveled

	List<Monster> spawnable;


	void Start(){
		
	}

	public bool Initalize(){
		//Load layout and max_monsters
		bool success = BuildFloor();
		if (success) {
			FloorMonsters ();
			foreach( Room r in Room_On_Floor){
				Room_Updater r_u = this.gameObject.AddComponent<Room_Updater> ();
				r_u.attached = r;
				r.Initalize ();
				//r.SpawnMonsters ();
			}
			
		}
		//Spawn monsters to fill the floor
		return success;

	}


	void Update(){
		
	}

	

	public void MoveUnit(DungeonUnit da){
		Room_On_Floor [da._location._room].MoveUnit (da);
	}


	public List<DungeonReactable> AddItemToGrid(DungeonReactable dr, coordinate new_loc){
		return Room_On_Floor [new_loc._room].AddItemToGrid (dr, new_loc);
	}

	private void FloorMonsters(){
		spawnable = MonsterFactory.Monster_Factory.GetSpawnable (floor_number);
	}

	public List<Monster> SpawnMonsters(Room r, int count){
		List<Monster> out_list = new List<Monster>();
		for (int i = 0; i < count; i++) {
			Monster m = SpawnMonster ();
			//Set location of Monster
			if (m != null) {
				do {
					m.explorer.RandomizeLocation (r._layer, r.get_size () [0], r.get_size () [1]);
				} while(!r.ValidSpawnLocation (m.explorer._location));
			
				out_list.Add (m);
			}
		}
		return out_list;
	}

	private Monster SpawnMonster(){
		spawnable.Sort (delegate(Monster x, Monster y) { //Randomly Shuffles the list up
			return Random.Range(0, spawnable.Count);
		});
		foreach (Monster m in spawnable) { 
			if (m._spawn_rate > Random.value) { //Checks to see if the Monster's spawn chance is hit. if it is the monster is returned
				return new Monster(m);
			}
		}
		return null;
	}

	public List<DungeonReactable> AddAdventurerToFloor(DungeonAdventurer a){ //used for when an adventurer is entering from another floor
		((Adventurer)a.attached_unit).AddEnteredFloor (floor_number);
		if (a._location._floor > this.floor_number) {
			return Room_On_Floor [ExitLocation].AddAdventurerToRoom (a);
		} else {
			return Room_On_Floor [EnterLocation].AddAdventurerToRoom (a);
		}
	}

	public List<DungeonReactable> AddMonsterToFloor(Monster m){ //Used for when a monster is entering from another floor
		if (m.explorer._location._floor > this.floor_number) {
			return Room_On_Floor [ExitLocation].AddMonsterToRoom (m);
		} else {
			return Room_On_Floor [EnterLocation].AddMonsterToRoom (m);
		}
	}

	public string DoorToExit(coordinate loc){
		return Room_On_Floor [loc._room].DoorToExit (loc);
	}

	public string DoorToEnter(coordinate loc){
		return Room_On_Floor [loc._room].DoorToEntrance (loc);
	}

	/*public void AddSoundReaction(DungeonReactable dr){
		SoundReactionObjects.Add (dr);
	}*/

	private bool BuildFloor()
	{
		CreateRoomLayout ();
		bool success = true;
		Room exit_room = RoomFactory.TheFactory.GetRandomRoom (floor_number, new string[]{"EXIT"});
		Room_On_Floor = new List<Room> ();
		exit_room.setIndex (0);
		
		while (!RandomlyPlaceRoom (exit_room));
		exit_room.set_Exit_Door ();
		exit_room.parent = this;
		ExitLocation = 0;
	 
		int max_to_goal = CalculateMaxRoomsToGoal ();
		int attempts = 0;
		List<Connection> room_conns;
		List<string> tested = new List<string> ();
		bool room_set = false;


		for (int i = 0; i < max_to_goal; i++) {
			attempts = 0;
			room_conns = Room_On_Floor [i].get_open_connections ();

			tested = new List<string> ();
			room_set = false;
			do {
				
				Room R_new = RoomFactory.TheFactory.GetRandomRoom (new string[]{ "EXIT" }, floor_number);
				while(tested.Contains(R_new._ID) && attempts < 15){
					R_new = RoomFactory.TheFactory.GetRandomRoom (new string[]{ "EXIT" }, floor_number);
					attempts++;
				}
			
				if(attempts < 15){
					R_new.setIndex (Room_On_Floor.Count );
					List<Connection> new_conns = R_new.get_open_connections ();
					room_conns = ShuffleList(room_conns);
					int select = 0;
					bool worked = false;
					while(select < room_conns.Count && !worked){
						Connection con_choice = room_conns[select];
						Connection con_match = Connection.FindMatchingConnection(new_conns, con_choice);
						worked = PlaceRoom(con_choice, con_match, Room_On_Floor[Room_On_Floor.Count -1], R_new);
						if(worked ){
							Connection.CompleteConnection(con_choice, con_match);
							R_new.set_Exit_heat(Room_On_Floor[Room_On_Floor.Count -1]._exit_dist);
							room_set = true;
							R_new.parent = this;
							Room_On_Floor.Add(R_new);
							select = room_conns.Count;
						}
						else{
							tested.Add(R_new._ID);
							select++;
						}
					}
				}
				if(attempts >= 15){ //If too many attempts on a single room attempts it exits out of placing rooms
					room_set = true;
					i = max_to_goal;
				}
				if(!room_set){
					attempts++;
				}
			} while(!room_set);
			
		}
		// place entrance
		bool entrancePlaced;
		do{
			entrancePlaced = Room_On_Floor[Room_On_Floor.Count -1].set_Enter_Door();
			EnterLocation = Room_On_Floor.Count - 1;
		}while(!entrancePlaced);
		
		//Fill in enter_dist
		for (int i = Room_On_Floor.Count - 2; i >= 0; i--) {
			Room_On_Floor [i].set_Enter_heat (Room_On_Floor [i + 1]._enter_dist);
		}


		List<Connection>open_connections = new List<Connection>();
		foreach (Room r in Room_On_Floor) {
			open_connections.AddRange (r.get_open_connections());
		}

		for (int i = 0; i < open_connections.Count; i++) {
			i = Random.Range (i, open_connections.Count);
			BuildPath (open_connections [i]);
		}

		LinkRooms ();

		foreach (Room r in Room_On_Floor) {
			r.Close_Connections ();
		}
		
		return success;
	}
	
	public static List<Connection> ShuffleList(List<Connection> shuffler){
		for (int i = 0; i < shuffler.Count; i++) {
			int r = Random.Range (0, shuffler.Count);
			Connection c = shuffler [i];
			shuffler [i] = shuffler [r];
			shuffler [r] = c;
		}
		return shuffler;
	}

	private void CreateRoomLayout(){
		RoomLayout = new List<List<List<int>>> ();
		int layers = 1;	
		if (floor_number > 12) {
			layers += Mathf.FloorToInt (floor_number / 12);	 
		}
		int size = 150; //increase size or calculate size for different floors
		for (int i = 0; i < layers; i++) {
		RoomLayout.Add (new List<List<int>>());
			for (int j = 0; j < size; j++) {
				RoomLayout [i].Add (new List<int> ());
				for (int k = 0; k < size; k++) {
					RoomLayout [i] [j].Add (0);
				}
			}
		}
	}

	private int CalculateMaxRoomsToGoal(){
		//TODO fill in calculations. f(x) = x ^ 3
		return 6;
	}

	private void LinkRooms(){
		for (int i = 0; i < Room_On_Floor.Count; i++) {
			for (int j = i; j < Room_On_Floor.Count; j++) {
				if (i != j) {
					if (Room_On_Floor [j].get_open_connections ().Count > 0 && Room_On_Floor [i].get_open_connections ().Count > 0) {
						if (Room_On_Floor [i].NextToRoom (Room_On_Floor [j])) {
							List<Connection> conns = Connection.FindMatchingConnection (Room_On_Floor [i].get_open_connections (), Room_On_Floor [j].get_open_connections ());
							if (conns != null) {
								Connection.CompleteConnection (conns [0], conns [1]);
							}
						}
					}
				}
			}
		}
	}

	private void BuildPath(Connection c){
		int randomRange = Random.Range(3, 5);
		Connection next = c;
		Room last = c._room;
		int test_count = 0;
		for(int i = 0; i < randomRange; i++) {
			//Connection con = next[i];
			Room R_new = RoomFactory.TheFactory.GetRandomRoom (new string[0], floor_number);

			R_new.setIndex (Room_On_Floor.Count );
			List<Connection> new_conns = ShuffleList(R_new.get_open_connections ());
			int select = 0;
			bool worked = false;
			
			Connection con_match = Connection.FindMatchingConnection(new_conns, next);
			worked = PlaceRoom(next, con_match, next._room, R_new);
			if(worked ){
				Connection.CompleteConnection(next, con_match);
				R_new.set_Exit_heat(next._room._exit_dist);
				R_new.set_Enter_heat (next._room._enter_dist);
				Room_On_Floor.Add(R_new);
				next = R_new.get_open_connections()[Random.Range(0, R_new.get_open_connections().Count)];
				R_new.parent = this;
				last = R_new;
				test_count = 0;
			}
			else{
				next = last.get_open_connections()[Random.Range(0, last.get_open_connections().Count)];
				i--;
				test_count++;
				if (test_count > 3) {
					i = randomRange + 1;
				}
				if (i == -1) {
					i = randomRange + 1;
				}
			}
			
		}
	}

	private bool RandomlyPlaceRoom(Room r){
		int[] size = r.get_size ();
		int max_tries = 5;
		int i = 0;
		while (i < max_tries) {
			int layer = Random.Range (0, RoomLayout.Count);
			int x = Random.Range (0, RoomLayout [0].Count - size [0]);
			int y = Random.Range (0, RoomLayout [0].Count - size [1]);
			if (Room_fits (x, x + size [0], x, y, y + size [1], y, layer)) {
				r.AdjustPosition (layer, floor_number);
				r.CreateBounds (x, y);
				Room_On_Floor.Add (r);
				i = int.MaxValue;
			} else {
				i++;
			}
		}
		return i == int.MaxValue;
	}
	
	private bool RandomlyPlaceRoom(Room r, int layer){
	if (layer <= RoomLayout.Count - 1 && layer > 0) {
			int[] size = r.get_size ();
			int max_tries = 5;
			int i = 0;
			while (i < max_tries) {
				int x = Random.Range (0, RoomLayout [0].Count - size [0]);
				int y = Random.Range (0, RoomLayout [0].Count - size [1]);
				if (Room_fits (x, x + size [0], x, y, y + size [1], y, layer)) {
					r.AdjustPosition (layer, floor_number);	
					r.CreateBounds (x, y);
					Room_On_Floor.Add (r);
					i = int.MaxValue;
				} else {
					i++;
				}
			}
			return i == int.MaxValue;
		} else {
			return false;
		}
	}

	private bool PlaceRoom(Connection existing_con, Connection con_match, Room existing, Room R_new) {
		if (con_match == null) {
			return false;
		}
		bool out_val = false;
		List<int[]> bound = existing._bounds;	
		int[] size = R_new.get_size ();
		int x_shift = existing_con._location._x_int - con_match._location._x_int;
		int z_shift = existing_con._location._z_int - con_match._location._z_int;
		float dir = Random.value;
		switch (existing_con._Direction) {
		case 1:
			if (Room_fits (bound [3] [0] + x_shift, bound [3] [0] + size [0] + x_shift, bound [3] [0] + x_shift, bound [3] [1], bound [3] [1] + size [1], bound [3] [1], existing._layer)) {
				R_new.AdjustPosition (existing._layer, floor_number);
				R_new.CreateBounds (bound [3] [0], bound [3] [1]);
				out_val = true;
			}
			
			break;
		case 2:
			if (Room_fits (bound [1] [0], bound [1] [0] + size [0], bound [1] [0], bound [1] [1] + z_shift, bound [1] [1] + size [1] + z_shift, bound [1] [1] + z_shift, existing._layer)) {
				R_new.AdjustPosition (existing._layer, floor_number);
				R_new.CreateBounds (bound [1] [0], bound [1] [1]);
				out_val = true;
			}
			break;
		case 3:
			if (Room_fits (bound [0] [0] + x_shift, bound [0] [0] + size [0] + x_shift, bound [0] [0] + x_shift, bound [0] [1] - size [1], bound [0] [1], bound [0] [1] - size [1], existing._layer)) {
				R_new.AdjustPosition (existing._layer, floor_number);
				R_new.CreateBounds (bound [0] [0], bound [0] [1] - size [1]);
				out_val = true;
			}
			break;
		case 4:
			if (Room_fits (bound [0] [0] - size [0], bound [0] [0], bound [0] [0] - size [0], bound [0] [1] + z_shift, bound [0] [1] + size [1] + z_shift, bound [0] [1] + z_shift, existing._layer)) {
				R_new.AdjustPosition (existing._layer, floor_number);
				R_new.CreateBounds (bound [0] [0] - size [0], bound [0] [1]);
				out_val = true;
			}
			break;
		case 5:
			out_val = RandomlyPlaceRoom (R_new, existing._layer + 1);
			break;
		case 6:
			out_val = RandomlyPlaceRoom (R_new, existing._layer - 1);
			break;
		}

		return out_val;
	}

	public string floor_name{
	get{ return floorName + floor_number; }
	}
	
	private bool Room_fits(int x_min, int x_max, int x_curr, int y_min, int y_max, int y_curr, int layer){
		bool res = true;
		if (0 <= x_curr && x_curr < RoomLayout [0] [0].Count) {
			if (0 <= y_curr && y_curr < RoomLayout [0].Count) {
				if (0 <= layer && layer < RoomLayout.Count) {
					if (RoomLayout [layer] [y_curr] [x_curr] == 1) {
						return false;
					}
				}
				else {
					return false;
				}
			} 
			else {
				return false;
			}
		} 
		else {
			return false;
		}

		if (x_curr < x_max) {
			if (x_curr + 1 < RoomLayout [0] [0].Count && y_curr < RoomLayout [0].Count) {
				res = Room_fits (x_min, x_max, x_curr + 1, y_min, y_max, y_curr, layer);
			} else {
				return false;
			}
		} else if (y_curr < y_max) {
			if (x_curr < RoomLayout [0] [0].Count && y_curr + 1 < RoomLayout [0].Count) {
				res = Room_fits (x_min, x_max, x_min, y_min, y_max, y_curr + 1, layer);
			} else {
				return false;
			}
		}
		/*else{
		 * x is max and y is max. Change int to 1
		 * }*/
		if (res) {
			if (x_curr < x_max && x_curr > x_min) {
				if (y_curr < y_max && y_curr > y_min) {
					RoomLayout [layer] [y_curr] [x_curr] = 1;
				} else {
					RoomLayout [layer] [y_curr] [x_curr] = 2;
				}
			} else {
				RoomLayout [layer] [y_curr] [x_curr] = 2;
			}
		}
		return res;
	}

	public Room GetCurrentRoom(int index){
		return Room_On_Floor [index];
	}
}


