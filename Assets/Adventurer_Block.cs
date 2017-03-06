using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Adventurer_Block : MonoBehaviour, IDragHandler {

	private ScreenManager parent;
	Adventurer this_hero;
	public Goal_Maker g;
	public CharacterSheet sheet_template;
	public Floor_Selection floor_template;
	public Pop_up pop;
	// Use this for initialization
	void Start () {
		parent = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ();

		GetComponentInChildren<UnityEngine.UI.Text> ().text = this_hero.name;
		UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button> ();

		UnityEngine.UI.Button.ButtonClickedEvent b_event = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event.AddListener (ShowCharacterSheet);
		buttons [0].onClick = b_event;
		buttons [0].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Display Stats";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_1 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_1.AddListener (PrintDungeonLog);
		buttons [1].onClick = b_event_1;
		buttons [1].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Print Dungeon Log";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_2 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_2.AddListener (UpdateStatus);
		buttons [2].onClick = b_event_2;
		buttons [2].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Update Stats";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_3 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_3.AddListener (CreateGoal);
		buttons [3].onClick = b_event_3;
		buttons [3].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Set Goal";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_4 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_4.AddListener (RestockItems);
		buttons [4].onClick = b_event_4;
		buttons [4].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Get Item";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_5 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_5.AddListener (SetTargetFloor);
		buttons [5].onClick = b_event_5;
		buttons [5].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Set Target Floor";

		UnityEngine.UI.Button.ButtonClickedEvent b_event_6 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_6.AddListener (SendToDungeon);
		buttons [6].onClick = b_event_6;
		buttons [6].GetComponentInChildren<UnityEngine.UI.Text> ().text = "Send to Dungeon";
	}

	public void Initalize(Adventurer a){
		this_hero = a;
	}

	public void SendToDungeon(){
		print ("send to dungeon");
		if (((DungeonAdventurer)this_hero.explorer).goal != null) {
			if (this_hero.explorer._location_goal != null) {
				((DungeonAdventurer)this_hero.explorer).goal.UpdateTime ();
				if (GameObject.FindGameObjectWithTag ("Guild").GetComponent<Guild_Manager> ().SendToDungeon (this_hero)) {
					parent.RemoveBlock (this);
					Destroy (this.gameObject);
				}
			} else {
				CreatePopup ("Target Floor is not set");
			}
		} else {
			CreatePopup ("Goal is not set");
		}
	}

	public void CreatePopup(string text){
		print ("creating popup");
		Pop_up message = Instantiate (pop, gameObject.GetComponentInParent<Transform>());
		message.transform.position = this.transform.position;
		message.Initalize (text);
	}

	public void UpdateStatus(){
		CharacterSheet sheet = Instantiate (sheet_template, gameObject.GetComponentInParent<Transform>());
		sheet.transform.position = this.transform.position;
		sheet.Initalize (this_hero);
		this_hero.UpdateStats ();
		sheet.UpdateSheet (this_hero);
	}

	public void ShowCharacterSheet(){
		//GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().AddCharacterSheet (this_hero);
		CharacterSheet sheet = Instantiate (sheet_template, gameObject.GetComponentInParent<Transform>());
		sheet.transform.position = this.transform.position;
		sheet.Initalize (this_hero);
	}

	public void PrintDungeonLog(){
		GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().DisplaySingleLog (this_hero);
	}

	public void CreateGoal(){
		Goal_Maker goal_builder = Instantiate (g, gameObject.GetComponentInParent<Transform>());
		goal_builder.transform.position = this.transform.position;
		goal_builder.Initialize ((DungeonAdventurer)this_hero.explorer);
	}

	public void SetTargetFloor(){
		Floor_Selection floorselect = Instantiate (floor_template, GameObject.FindGameObjectWithTag ("Canvas").gameObject.transform);
		floorselect.transform.localPosition = new Vector3 ();
		floorselect.Initalize (this_hero);
	}

	public void RestockItems(){
		//Open menu to go buy items or grab from warehouse
		this_hero.AddItem(this_hero._guild.retreive_item());
	}

	public void OnDrag(PointerEventData eventData){
		GameObject obj = eventData.pointerDrag;
		Vector2 pointPos = eventData.position;
		Vector3 myloc = obj.transform.position;
		//myloc.y =  pointPos.y;
		float move = pointPos.y - myloc.y;
		parent.MovePlayerNodes (move);
	}

	public bool rendering{
		get{ return gameObject.activeSelf; }
		set{ gameObject.SetActive(value); }
	}
}
