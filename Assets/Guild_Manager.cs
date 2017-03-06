using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild_Manager : MonoBehaviour {

	private static Guild_Manager main_manager;
	private string guild_name;
	private List<Adventurer> available_members;
	private List<Adventurer> members;

	private List<Item> warehouse = new List<Item>();
	//God/Goddess

	// Use this for initialization

	void Start () {
		if (main_manager == null) {
			main_manager = this;
			members = new List<Adventurer> ();
			available_members = new List<Adventurer> ();
			UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
			DontDestroyOnLoad (this.gameObject);
		} else if(main_manager != this) {
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public List<Adventurer> GetAvailableMembers(){
		return available_members;
	}

	public void UpdateStats(){
		foreach (Adventurer a in available_members) {
			a.UpdateStats ();
		}
	}

	public void SceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1){
		if (arg1.name != "Character_creation_scene") {
			AddNewMembers ();
		}
		if (arg1.name == "Test scne") {
			foreach(Adventurer a in available_members){
				GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().AddAdventurerBlock (a);
			}
		}
	}

	public void HealGuild(){
		foreach (Adventurer a in available_members) {
			a.FullHeal ();
		}
	}

	private void AddNewMembers(){
		GameObject[] characters = GameObject.FindGameObjectsWithTag ("New_Character");
		foreach (GameObject characterHolder in characters) {
			Adventurer a = characterHolder.GetComponent<CreateCharacter> ().getAdventurer ();

			if (a != null) {
				members.Add (a);
				available_members.Add (a);

				Destroy (characterHolder);
			}
		}

	}

	public void SendToDungeon(){
		//Determine which character is going to the dungeon
		Adventurer a = available_members[0];
		available_members.RemoveAt(0);
		Dungeon.The_Dungeon.SendToDungeon (a);
	}

	public bool SendToDungeon(Adventurer a){
		//Determine which character is going to the dungeon
		if (available_members.Contains (a)) {
			available_members.Remove (a);
			Dungeon.The_Dungeon.SendToDungeon (a);
			return true;
		} else {
			return false;
		}
	}

	public void HomeFromDungeon(Adventurer a){
		if (members.Contains (a)) {
			available_members.Add (a);
			a.explorer.ClearTarget ();
			GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().AddAdventurerBlock (a);
		} else {
			print ("Who is this?");
		}
	}

	public void addItem(Item i){
		warehouse.Add (i);
	}

	public Item retreive_item(){ //Include item to remove
		if (warehouse.Count > 0) {
			Item i = warehouse [0];
			warehouse.RemoveAt (0);
			return i;
		}
		return null;
	}
}
