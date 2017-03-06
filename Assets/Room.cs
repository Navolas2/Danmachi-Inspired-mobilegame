using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Room
{
	public Dungeon_Floor parent;

	private static List<char> validSpawn = new List<char>{'P', 'R', 'F', 'S', 's', 'M', 'A', 'D'};

	List<List<char>> layout;
	List<List<string[]>> pathway_heat;
	List<Connection> doorways;
	List<int> Allowed_Floors; //negative number, exclude from all except given floor. Positive number, only allowed on these floors. 0 no restrictions. NO MIXING
	int[] size; //X and then Y
	List<string> tags;
	string ID;
	private string indID;

	int floor;
	int index;
	int exit_dist;
	int enter_dist;
	List<int[]> bounds;
	int layer;
	int min_monsters;
	Date_Time last_spawn;
	List<DungeonReactable> sound;
	List<DungeonAdventurer> Adventurers_in_Room;
	List<Monster> Monsters_in_room;
	List<BattleManager> Battles_in_Room;
	Location_Grid Room_grid;

	/********************/
	//Constructors
	/*******************/

	public Room (Room r_copy)
	{
		layout = new List<List<char>> ();
		for (int i = 0; i < r_copy.layout.Count; i++) {
			layout.Add (new List<char> ());
			char[] c_temp = new char[r_copy.layout[i].Count];
			r_copy.layout [i].CopyTo (c_temp);
			for (int j = 0; j < c_temp.Length; j++) {
				layout [i].Add (c_temp [j]);
			}
		}

		pathway_heat = r_copy.pathway_heat;
		doorways = new List<Connection> ();
		Allowed_Floors = r_copy.Allowed_Floors;
		size = r_copy.size;
		tags = r_copy.tags;
		ID = r_copy.ID;
		indID = ID + Random.Range (0, int.MaxValue / 4);
		for (int i = 0; i < r_copy.doorways.Count; i++) {
			Connection c = new Connection (r_copy.doorways [i], this); 
			doorways.Add(c);
		}

		exit_dist = 0;
		enter_dist = 0;
	}


	public Room( List<List<char>> lay, List<List<string[]>> path, List<coordinate> doors, List<int> directions, List<string> label, List<int> Allowed, int[] size_amount, List<string> tag, string id){
		layout = lay;
		pathway_heat = path;
		doorways = new List<Connection> ();
		for (int i = 0; i < doors.Count; i++) {
			doorways.Add (new Connection (doors[i], directions[i], label[i], this));
		}
		Allowed_Floors = Allowed;
		size = size_amount;
		tags = tag;
		ID = id;
	}


	/********************/
	//Running Function
	/*******************/

	public void Update(){
		if (GameClock.The_Clock.TimeBasedUpdate (.5) || GameClock.The_Clock.TimeBasedUpdate (1)) { //Actions happen here

			for(int i = 0; i < Adventurers_in_Room.Count; i++){
				int count = Adventurers_in_Room.Count;
				DungeonAdventurer da = Adventurers_in_Room[i];
				da.DecideAction ();
				da.action (da);
				if (Adventurers_in_Room.Count < count) {
					count--;
				}
			}
			foreach (Monster  mon in Monsters_in_room) {
				DungeonUnit da = mon.explorer;
				//da.DecideAction ();
				//da.action (da);
			}
		}
			/*
			/*float value = Random.value;
			if (value > .9 && adventurers_in_dungeon.Count > 0) {
				//Adventurer a = (Adventurer)adventurers_in_dungeon [0].attached_unit;
				//adventurers_in_dungeon.RemoveAt (0);
				//ReturnToGuild (a);
			} else if (value > .75) {
				if (adventurers_in_dungeon.Count > 0) {
					StartEncounter (0, 1);
				} else if (Battles_in_dungeon.Count > 0) {
					print ("Reinforcements!");
					Battles_in_dungeon [0].AddUnitToBattle (Encounter_Enemy (1));
				}
			}


		}
		*/
		
		/*
		if (GameClock.The_Clock.TimeBasedUpdate (.3) || GameClock.The_Clock.TimeBasedUpdate (.8)) { //Sound Reactions happen here
			

		} //Sound Reactions happen here
		*/

		if (GameClock.The_Clock.TimeBasedUpdate (.3) || GameClock.The_Clock.TimeBasedUpdate (.8)) { //spawning happen here
			Spawn();
		} //Spawning Happens here
		
		if (GameClock.The_Clock.Current_Tick % (GameClock.The_Clock.Game_Speed * .75) == 0) //Goal checking
		{ 
			for (int i = 0; i < Adventurers_in_Room.Count; i++) {
				DungeonAdventurer da = Adventurers_in_Room [i];
				if (da.IsGoalComplete ()) {
					da.SetTargetLocation (new coordinate (0));
				}
			}
		} //Goal Checking

	}

	private void CalcMinMonsters(){
		//TODO fillout calculation
		min_monsters = 2;
	}

	private void Spawn(){
		//TODO fillout spawn System
		//Determine is Spawning should be done
		//Factors: floor, Monsters in room, health of Adventurer, time since last spawn
		if (last_spawn != null) {
			int length = last_spawn.Compare (GameClock.The_Clock.getTime ());
			length = Mathf.Abs (length);
			if (Monsters_in_room.Count < min_monsters) {
				
				if (length > 1000) {
					SpawnMonsters ();
				}

			} else {
				if (floor >= Dungeon.MiddleLevels) {
					if (Monsters_in_room.Count < Space () * 2) {
						float healthTotal = 0;
						float healthCurrent = 0;
						foreach (DungeonAdventurer da in Adventurers_in_Room) {
							healthTotal += da.attached_unit.hp_max;
							healthCurrent += da.attached_unit.hp;
						}
						if (healthCurrent <= (healthTotal / 2)) {
							if (length > 300) {
								SpawnMonsters (Random.Range (min_monsters, min_monsters * 2));
							}
						} else {
							if (length > 300) {
								SpawnMonsters (Random.Range (min_monsters, min_monsters * 3));
							}
						}
					}
				}
			}
		} else {
			SpawnMonsters ();
		}
	}

	private void SpawnMonsters(){
		List<Monster> new_mons = parent.SpawnMonsters (this, Random.Range(2, 5));
		foreach (Monster m in new_mons) {
			SpawnMonsterInRoom (m);
			Debug.Log ("Monsters have spawned");
		}
		foreach (DungeonAdventurer a in Adventurers_in_Room) {
			a.SpawnReaction (new_mons);
		}
		last_spawn = GameClock.The_Clock.getTime ();
	}

	private void SpawnMonsters(int num){
		List<Monster> new_mons = parent.SpawnMonsters (this, num);
		foreach (Monster m in new_mons) {
			SpawnMonsterInRoom (m);
		}
		foreach (DungeonAdventurer a in Adventurers_in_Room) {
			a.SpawnReaction (new_mons);
		}
		last_spawn = GameClock.The_Clock.getTime ();
	}

	public bool ValidLocation(coordinate loc){
		char location = layout[Mathf.RoundToInt(loc._z)][Mathf.RoundToInt(loc._x)];
		if (location == 'W' || location == 'O') {
			return false;
		}
		return true;
	}

	public bool ValidSpawnLocation(coordinate loc){
		if (loc._z_int < layout.Count && loc._x_int < layout [0].Count) {
			char location = layout [Mathf.RoundToInt (loc._z)] [Mathf.RoundToInt (loc._x)];
			if (!validSpawn.Contains (location)) {
				return ValidLocation (loc);
			}
			return true;
		} else {
			return false;
		}
	}

	public List<int> MoveableDirections (coordinate location, out List<string[]> heats, out List<char> tiles)
	{
		List<char> around = new List<char> ();
		around.Add(layout [location._z_int + 1] [location._x_int]);
		around.Add(layout [location._z_int] [location._x_int + 1]);
		around.Add(layout [location._z_int - 1] [location._x_int]);
		around.Add(layout [location._z_int] [location._x_int - 1]);
		List<int> moveable = new List<int> ();
		for (int i = 0; i < around.Count; i++) {
			if (around [i] != 'W') {
				if (around [i] != 'O') {
					moveable.Add (i + 1); //1 up, 2 right, 3 down, 4 left
				}
			}
		}
		tiles = around;
		List<Connection> c = doorways;

		heats = SurroundingHeat (location);
		return moveable;
	}

	public List<DungeonReactable> AddUnitToRoom(DungeonUnit du){
		if (du.attached_unit._type == Adventurer.Type_Adventurer) {
			return AddAdventurerToRoom ((DungeonAdventurer)du);
		} else {
			return AddMonsterToRoom ((Monster)du.attached_unit);
		}
	}

	public List<DungeonReactable> AddAdventurerToRoom(DungeonAdventurer a){
		Adventurers_in_Room.Add (a);
		SetLocation (a);
		return Room_grid.AddItem (a, a._location);
	}

	public List<DungeonReactable> AddMonsterToRoom(Monster m){
		Monsters_in_room.Add (m);
		SetLocation (m.explorer);
		return Room_grid.AddItem(m.explorer, m.explorer._location);
	}
		
	public List<DungeonReactable> SpawnMonsterInRoom(Monster m){
		Monsters_in_room.Add (m);
		m.explorer._location.AdjustPosition (layer, floor, index);
		return Room_grid.AddItem(m.explorer, m.explorer._location);
	}

	public void SetLocation(DungeonUnit du){
		List<Connection> c = doorways;
		if (du._location._floor != this.floor) {
			Connection con = c.Find (delegate(Connection obj) {
				if (obj._Direction == 7 || obj._Direction == 8) {
					return true;
				}
				return false;
			});
			du._location.ChangePosition (con._grid_location);
		} else {
			Connection con = c.Find (delegate(Connection obj) {
				if (du._location._room == obj._pair_index){
					return true;
				}
				return false;
			});
			du._location.ChangePosition (con._grid_location);
		}
	}

	public void MoveUnit(DungeonUnit da){
		coordinate old_loc = new coordinate(da._location);
		if (da.MovePosition ()) {
			coordinate new_loc = da._location;
			//Debug.Log (old_loc.ToString () + "-->" + new_loc.ToString ());
			List<DungeonReactable> reactors = UpdatePosition (da, old_loc, new_loc);
			if (da._location._floor != old_loc._floor) {
				Room_grid.RemoveItem (da, old_loc);
			}
			if (reactors != null) {
				foreach (DungeonReactable dr in reactors) {
					//dr.SpaceReact (da); //TODO uncomment code
				}
			}
		}
	}

	public List<string[]> SurroundingHeat(coordinate coor){
		List<string[]> heats = new List<string[]>();
		heats.Add(new string[]{pathway_heat [coor._z_int + 1] [coor._x_int][0], pathway_heat [coor._z_int + 1] [coor._x_int][1]});
		heats.Add(new string[]{pathway_heat [coor._z_int] [coor._x_int + 1][0], pathway_heat [coor._z_int] [coor._x_int + 1][1]});
		heats.Add(new string[]{pathway_heat [coor._z_int - 1] [coor._x_int][0], pathway_heat [coor._z_int - 1] [coor._x_int][1]});
		heats.Add(new string[]{pathway_heat [coor._z_int] [coor._x_int - 1][0], pathway_heat [coor._z_int] [coor._x_int - 1][1]});
		return heats;
	}

	public string DoorToExit(coordinate coor){
		List<Connection> doors = doorways;
		Connection lowest = null;
		Connection exit = doors.Find (delegate(Connection obj) {
			if (obj._Direction == 8) {
				return true;
			}
			return false;
		});
		if (exit != null) {
			return exit._label;
		}
		for (int i = 0; i < doors.Count; i++) {
			if (lowest != null) {
				if (doors [i]._pair != null && lowest._pair != null) {
					if (doors [i]._pair_room._exit_dist < lowest._pair_room._exit_dist) {
						lowest = doors [i];
					}
				}
			} else {
				if (doors [i]._pair != null) {
					lowest = doors [i];
				}
			}
		}
		return lowest._label;
	}

	public string DoorToEntrance(coordinate coor){
		List<Connection> doors = doorways;
		Connection lowest = null;
		Connection enter = doors.Find (delegate(Connection obj) {
			if (obj._Direction == 7) {
				return true;
			}
			return false;
		});
		if (enter != null) {
			return enter._label;
		}

		for (int i = 0; i < doors.Count; i++) {
			if (lowest != null) {
				if (doors [i]._pair != null && lowest._pair != null) {
					if (doors [i]._pair_room._enter_dist < lowest._pair_room._enter_dist) {
						lowest = doors [i];
					}
				}
			} else {
				if (doors [i]._pair != null) {
					lowest = doors [i];
				}
			}
		}
		return lowest._label;
	}

	public List<DungeonReactable> AddItemToGrid(DungeonReactable dr, coordinate new_loc){
		return Room_grid.AddItem (dr, new_loc);
	}

	public int ConnectionDirection(DungeonUnit du){
		Connection match = doorways.Find (delegate (Connection con) {
			if(con.AtConnection(du._location)){
				return true;
			}
			return false;
		});
		if (match != null) {
			return match._Direction;
		} else {
			return -1;
		}
	}

	public void RemoveUnit(DungeonUnit du){
		if (du.attached_unit._type == Adventurer.Type_Adventurer) {
			RemoveAdventurer ((DungeonAdventurer)du);
		} else {
			RemoveMonster ((Monster)du.attached_unit);
		}
	}

	public void RemoveAdventurer(DungeonAdventurer da){
		Adventurers_in_Room.Remove (da);
	}

	public void RemoveMonster(Monster mon){
		Monsters_in_room.Remove (mon);
	}

	public List<DungeonReactable> MoveRoom(coordinate loc, DungeonUnit du){
		Connection match = doorways.Find (delegate (Connection con) {
			if(con.AtConnection(du._location)){
				return true;
			}
			return false;
		});
		if (match != null) {
			if (match._Direction != 7 && match._Direction != 8) {
				List<DungeonReactable> out_list =  match._pair_room.AddUnitToRoom (du);
				du.MoveRoom ();
				RemoveUnit (du);
				return out_list;
			} else {
				return null;
			}
		}
		return null;

	}

	public List<DungeonReactable> UpdatePosition(DungeonUnit du, coordinate old_loc, coordinate new_loc){
		if (layout [new_loc._z_int] [new_loc._x_int] == 'D') {
			List<DungeonReactable> dr = MoveFloor (du);
			if (dr != null) {
				return dr;
			} else {
				dr = MoveRoom (du._location, du);
				if (dr == null) {
					du._location.ChangePosition (old_loc);
					return null;
				} else {
					return dr;
				}
			}
		} else {
			return Room_grid.MoveItem (du, old_loc, new_loc);
		}
	}

	public List<DungeonReactable> MoveFloor (DungeonUnit moving){
		if (moving.attached_unit._type == Adventurer.Type_Adventurer) {
			return SendAdventurerToNewFloor ((DungeonAdventurer)moving);
		} else if (moving.attached_unit._type == Monster.type_Monster) {
			return SendMonsterToNewFloor ((Monster)moving.attached_unit);
		} else {
			return null;
		}
	}

	public List<DungeonReactable> SendAdventurerToNewFloor(DungeonAdventurer da){
		//Check to see if moving up or down
		int direction = ConnectionDirection(da);
		if (direction == 7 || direction == 8) {
			int room = da._location._room;
			int n_floor = floor + (direction == 8 ? 1 : -1);
			List<DungeonReactable> worked = Dungeon.The_Dungeon.PassAdventurerToFloor (n_floor, da);
			if (worked != null) {
				Debug.Log ("Moving floor from " + floor + " to " + n_floor);
				da.MoveFloor ();
				RemoveAdventurer (da);
				return worked;
			} else {
				return null;
			}
		}
		return null;
	}

	public List<DungeonReactable> SendMonsterToNewFloor(Monster mon){
		//Check to see if moving up or down
		int direction = ConnectionDirection(mon.explorer);
		if (direction == 7 || direction == 8) {
			int room = mon.explorer._location._room;
			int n_floor = floor + (direction == 8 ? 1 : -1);
			List<DungeonReactable> worked = Dungeon.The_Dungeon.PassMonsterToFloor (n_floor, mon);
			if (worked != null) {
				RemoveMonster (mon);
				return worked;
			} else {
				return null;
			}
		}
		return null;

	}
		
	public void RemoveDoorway (List<int>moveable, List<char> tiles, coordinate location, int dir){
		if (tiles.Contains ('D')) {
			Connection c = doorways.Find (delegate(Connection obj) {
				return obj._grid_location.IsLocation (location);
			});
			if (c._Direction == dir) {
				moveable.Remove (c._Orig_Direction);
			}
		}
	}
	/******************/
	//Setup Functions
	/*****************/

	public void Initalize(){
		sound = new List<DungeonReactable> ();
		Adventurers_in_Room = new List<DungeonAdventurer>();
		Monsters_in_room = new List<Monster>();
		Battles_in_Room = new List<BattleManager>();
		Room_grid = new Location_Grid(1, layout.Count, layout[0].Count);
		CalcMinMonsters ();
	}

	public void setIndex(int index_in){
		this.index = index_in;
	}

	public void AdjustPosition(int y, int floor){
		layer = y;
		foreach (Connection c in doorways) {
			c.AdjustPosition (y, floor, index);
		}
		this.floor = floor;
	}

	public bool has_open_connection(){
		return get_open_connections ().Count > 0;
	}

	public List<Connection> get_open_connections(){
		List<Connection> out_list = new List<Connection> ();
		foreach (Connection c in doorways) {
			if (c.is_open ()) {
				out_list.Add (c);
			}
		}
		return out_list;
	}

	public List<Connection> get_connections(){
		return doorways;
	}
		
	public void CreateBounds(int left_bottom_x, int left_bottom_y){
		bounds = new List<int[]> ();
		bounds.Add(new int[] {left_bottom_x, left_bottom_y}); //left bottom
		bounds.Add(new int[] {left_bottom_x + size[0], left_bottom_y}); //right bottom
		bounds.Add(new int[] {left_bottom_x + size[0], left_bottom_y + size[1]}); //right top
		bounds.Add(new int[] {left_bottom_x, left_bottom_y + size[1]}); //left top
	}

	public bool NextToRoom(Room r2){
		List<int[]> r2_bounds = r2._bounds;
		for (int i = 0; i < 4; i++) {
			int pair = i < 2 ? i + 2 : i - 2;
			if (bounds [i] [0] == r2_bounds [pair] [0] || bounds [i] [1] == r2_bounds [pair] [1]) {
				return true;
			}
		}
		return false;
	}

	public void Close_Connections(){
		List<Connection> open = get_open_connections ();
		foreach (Connection c in open) {
			coordinate coor = c._location;
			layout [coor._z_int] [coor._x_int] = 'W';
			doorways.Remove (c);
		}
	}

	public bool set_Enter_Door(){
		List<Connection> open = get_open_connections ();
		int index = Random.Range (0, open.Count);
		if (open.Count == 0) {
			return false;
		}
		open [index].ConvertToEntrance ();

		return true;
	}

	public bool set_Exit_Door(){
		List<Connection> open = get_open_connections ();
		int index = Random.Range (0, open.Count);
		if (open.Count == 0) {
			return false;
		}
		open [index].ConvertToExit ();

		return true;
	}

	public bool SetStairs(int dir){
		List<Connection> open = get_open_connections ();
		int index = Random.Range (0, open.Count);
		if (open.Count == 0) {
			return false;
		}
		open [index].ConvertToStairs (dir);

		return true;
	}

	public int Space(){
		int space = 1;
		space *= (size [0] - 2);
		space *= (size [1] - 2);
		return space;
	}

	public void set_Exit_heat(int heat_in){
		exit_dist = heat_in + 1;
	}

	public void set_Enter_heat(int heat_in){
		enter_dist = heat_in + 1;
	}

	public string _ID{
		get{ return ID; }
	}

	public int _door_count{
		get{ return doorways.Count; }
	}

	public string _Individual_ID{
		get { return indID; }
	}

	public int _exit_dist{
		get{ return exit_dist; }
	}

	public int _enter_dist{
		get{ return enter_dist; }
	}

	public int[] get_size(){
		return size;
	}

	public int _layer{
		get{ return layer; }
	}

	public List<int[]> _bounds{
		get{ return bounds; }
	}

	public List<string> get_tags(){
		return tags;
	}

	public int _knownTime{
		get{return Mathf.FloorToInt (((size [0] - 2) * (size [1] - 2)) / 2);}
	}

	public bool Equal (Room r)
	{
		return r._Individual_ID == this._Individual_ID;
	}

	public bool[] has_tag(string[] tag){
		bool[] match = new bool[tag.Length];
		for (int i = 0; i < tag.Length; i++) {
			match[i] = tags.Contains (tag[i]);
		}
		return match;
	}

	public bool allowed_on_floor(int floor){
		if (Allowed_Floors.Contains (0)) {
			return true;
		} else if (Allowed_Floors.Contains (floor)) {
			return true;
		} else if (Allowed_Floors.Contains (floor * -1)) {
			return false;
		} else if (Allowed_Floors [0] < 0) {
			return true;
		} else {
			return false;
		}
	}
}


