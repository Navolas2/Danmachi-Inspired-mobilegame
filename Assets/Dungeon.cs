using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour {

	public static Dungeon The_Dungeon;
	public static int MiddleLevels = 13;
	public static int HighLevels = 50;
	public GameObject Battle_Class;
	public GameObject empty;

	private List<DungeonAdventurer> adventurers_in_dungeon;
	private List<BattleManager> Battles_in_dungeon;
	private List<List<Monster>> Monsters_On_Floor; //Change to Monsters
	private List<Dungeon_Floor> Floors_of_Dungeon;

	void Awake(){
		if (The_Dungeon == null) {
			DontDestroyOnLoad (gameObject);
			The_Dungeon = this;
			adventurers_in_dungeon = new List<DungeonAdventurer> ();
			Battles_in_dungeon = new List<BattleManager> ();
			Floors_of_Dungeon = new List<Dungeon_Floor> ();
			//LoadData ();
		} else if (The_Dungeon != this) {
			Destroy (gameObject);
		} else {
			LoadData ();
		}
	}
	// Use this for initialization
	void Start () {
		//Adventurer a = new Adventurer ();
		//adventurers_in_dungeon.Add ((DungeonAdventurer)a.explorer);
		Monsters_On_Floor = new List<List<Monster>> ();
		Monsters_On_Floor.Add(new List<Monster>());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			print ("Building Room");
			Dungeon_Floor next_floor;
			bool success = true;
			GameObject go = Instantiate(empty, this.transform);
			next_floor = go.AddComponent<Dungeon_Floor> ();
			next_floor.floor_number = Floors_of_Dungeon.Count + 1;
				success = success && next_floor.Initalize ();
				if (success) {
					Floors_of_Dungeon.Add (next_floor);
				}
		}
	}

	public void StartEncounter(){
		if (adventurers_in_dungeon.Count > 0) {
			print ("Starting Battle");
			BattleManager b;
			GameObject battle = (GameObject)Instantiate (Battle_Class);
			b = battle.GetComponent<BattleManager> ();
			b.Initalize ();
			//UPDATE to add the adventurerers and monsters that encountered each other
			b.AddUnitToBattle (adventurers_in_dungeon [0].attached_unit);
			adventurers_in_dungeon.RemoveAt (0);
			Monster mon = The_Dungeon.Encounter_Enemy (1);
			if (mon != null) {
				b.AddUnitToBattle (mon);
			} else {
				print ("No monster?!");
			}
			b.enabled = true;
			Battles_in_dungeon.Add (b);
		} else if(Battles_in_dungeon.Count > 0) {
			BattleManager b = Battles_in_dungeon [0];
			Monster mon = The_Dungeon.Encounter_Enemy (1);
			if (mon != null) {
				b.AddUnitToBattle (mon);
			} else {
				print ("No monster?!");
			}
		}
	}

	public void StartEncounter(int adventer, int monster){
		print ("A BATTLE!");
		BattleManager b;
		GameObject battle = (GameObject)Instantiate (Battle_Class);
		b = battle.GetComponent<BattleManager> ();
		b.Initalize();
		//UPDATE to add the adventurerers and monsters that encountered each other
		b.AddUnitToBattle (adventurers_in_dungeon [adventer].attached_unit);
		adventurers_in_dungeon.RemoveAt (adventer);
		Monster mon = The_Dungeon.Encounter_Enemy (1);
		if (mon != null) {
			b.AddUnitToBattle (mon);
		} else {
			print ("No monster?!");
		}
		b.enabled = true;
		Battles_in_dungeon.Add (b);
	}

	public void SendToDungeon(Adventurer hero){
		print (hero.name + " has entered the dungeon");
		hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_ENTER, "Entered the Dungeon", hero.name, "self");
		DungeonAdventurer da = (DungeonAdventurer)hero.explorer;
		List<DungeonReactable> success = PassAdventurerToFloor(1, da);
		if (success == null) {
			ReturnToGuild (hero);
		}
	}

	public void ReturnToGuild(){
		
		Adventurer a = (Adventurer)adventurers_in_dungeon [0].attached_unit;
		a.diary.AddEntry (DungeonLog.ENTRY_TYPE_EXIT, "Returned home from Dungeon", a.name, "self");
		print (a.name + " has left the dungeon");
		adventurers_in_dungeon.RemoveAt (0);
		a._guild.HomeFromDungeon (a);
	}

	public void ReturnToGuild(Adventurer hero){
		print (hero.name + " has left the dungeon");
		hero.diary.AddEntry (DungeonLog.ENTRY_TYPE_EXIT, "Returned home from Dungeon", hero.name, "self");
		hero._guild.HomeFromDungeon (hero);
	}

	public void ReturnToExploring(List<DungeonUnit> units, BattleManager b){
		foreach (DungeonUnit du in units) {
			if (du.attached_unit._type == Adventurer.Type_Adventurer) {
				adventurers_in_dungeon.Add ((DungeonAdventurer)du);
			} else {
				Monsters_On_Floor [du._location._floor].Add ((Monster)du.attached_unit);
			}
		}
		Battles_in_dungeon.Remove (b);
	}

	public Monster Encounter_Enemy(int floor){
		//Randomly checks to see if adventurer finds an enemy.
		//if enemy is found it is removed from the floor list and returned;

		//TEMPORARY TEST CODE
		if (Monsters_On_Floor [0].Count > 0) {
			Monster encountered = Monsters_On_Floor [0] [(int)Mathf.RoundToInt (Random.Range (0, Monsters_On_Floor [0].Count))];
			Monsters_On_Floor [0].Remove (encountered);
			return encountered; //test case
		} else {
			print ("Something went wrong");
			return null;
		}
	}

	private void LoadData(){
		
	}

	public List<DungeonReactable> PassAdventurerToFloor(int n_floor, DungeonAdventurer da){
		//Check to see if floor exists or if floor is 0. 0 is outside
		Dungeon_Floor next_floor;
		bool success = true;
		if (Floors_of_Dungeon.Count < n_floor) { //if floor does not exist create now
			GameObject go = Instantiate(empty, this.transform);
			next_floor = go.AddComponent<Dungeon_Floor> ();
			next_floor.floor_number = n_floor;
			success = success && next_floor.Initalize ();
			if (success) {
				Floors_of_Dungeon.Add (next_floor);
			} else {
				return null;
			}
		} else if (n_floor == 0) {
			if (da.IsGoalComplete ()) {
				da._location.AdjustPosition (0, 0, 0);
				ReturnToGuild ((Adventurer)da.attached_unit);
				return new List<DungeonReactable> ();
			} else {
				return null;
			}
		}else {
			next_floor = Floors_of_Dungeon [n_floor - 1];
		}
		return next_floor.AddAdventurerToFloor (da);
	}

	public List<DungeonReactable> PassMonsterToFloor (int n_floor, Monster mon){
		Dungeon_Floor next_floor;
		if (Floors_of_Dungeon.Count < n_floor) { //floors cannot be created by a monster
			return null;
		} else if (n_floor == 0) { //Monsters cannot leave the dungeon
			return null;
		}else {
			next_floor = Floors_of_Dungeon [n_floor - 1];
		}
		return next_floor.AddMonsterToFloor (mon);
	}
		
	public List<Adventurer> GetDungeonExplorers(){
		List<Adventurer> heroes = new List<Adventurer> ();
		foreach (DungeonAdventurer da in adventurers_in_dungeon) {
			heroes.Add ((Adventurer)da.attached_unit);
		}
		return heroes;
	}

	public List<string> GetFloorNames (){
		List<string> names = new List<string> ();
		foreach (Dungeon_Floor d_f in Floors_of_Dungeon) {
			names.Add (d_f.floor_name);
		}
		return names;
	}
		
	public string DoorToExit(coordinate loc){
		return Floors_of_Dungeon [loc._floor - 1].DoorToExit (loc);
	}

	public string DoorToEnter(coordinate loc){
		return Floors_of_Dungeon [loc._floor - 1].DoorToEnter (loc);
	}
		
	public Dungeon_Floor GetFloor(int floor_number){
		return Floors_of_Dungeon [floor_number - 1];
	}
}
