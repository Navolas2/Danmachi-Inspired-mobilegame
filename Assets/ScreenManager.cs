using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour {

	private Information_Scrolling left;
	private Information_Scrolling right;
	public GameObject adventurerer_block;
	private List<Adventurer_Block> blocks;
	private UnityEngine.UI.Text date_information;
	public GameObject shop_manager;
	private Shop_Manager current_shop;

	private float screenTop;
	private float screenBottom = -10;
	private float player_node_spacing = 260;

	public int[] menu_options;


	// Use this for initialization
	void Start () {
		left = GetComponentsInChildren<Information_Scrolling> () [0];
		right = GetComponentsInChildren<Information_Scrolling> () [1];
		if (blocks == null) {
			blocks = new List<Adventurer_Block> ();
		}

		date_information = GetComponentsInChildren<UnityEngine.UI.Text> () [0];
		date_information.text = "I AM THE DATE";

		Vector3 position = new Vector3 ();
		position.y = Screen.height - date_information.GetComponent<RectTransform>().rect.height;
		position.x = Screen.width - date_information.GetComponent<RectTransform>().rect.width;
		date_information.gameObject.transform.position = position;
		screenTop = (float)Screen.height;

		current_shop = null;

		UnityEngine.UI.Scrollbar scroller = GetComponentInChildren<UnityEngine.UI.Scrollbar> ();
		scroller.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (40f, Screen.height);
		scroller.transform.localPosition = new Vector3 (Screen.width * -.45f, 0);
		UnityEngine.UI.Scrollbar.ScrollEvent s_event = new UnityEngine.UI.Scrollbar.ScrollEvent ();
		s_event.AddListener (GameObject.FindGameObjectWithTag ("Globals").GetComponent<MenuManager> ().MoveNodes);
		scroller.onValueChanged = s_event;
		GameObject.FindGameObjectWithTag ("Globals").GetComponent<MenuManager> ().LinkScrollBar (scroller);
	}
	
	// Update is called once per frame
	void Update () {
		date_information.text = GameClock.The_Clock.getTime ().ToString ();
	}

	public void ClearScreen(){
		left.Clear ();
		right.Clear ();
		if (current_shop != null) {
			Destroy (current_shop.gameObject);
		}
	}

	public void MovePlayerNodes(float move){
		foreach(Adventurer_Block tn in blocks){
			Vector3 loc = tn.transform.position;
			loc.y += move;
			tn.transform.position = loc;

			/*
			if (loc.y < screenBottom || loc.y > screenTop) {
				tn.rendering = false;
			} else {
				tn.rendering = true;
			}

			tn.transform.position = loc;
			*/
		}
	}

	public void OpenMarket(){
		ClearScreen ();
		GameObject n_node = (GameObject)Instantiate (shop_manager, this.transform);
		current_shop = n_node.GetComponent<Shop_Manager> ();
		n_node.transform.localPosition = new Vector3 ();
	}

	public void AddAdventurerBlock(Adventurer a){
		if (blocks == null) {
			blocks = new List<Adventurer_Block> ();
		}
		GameObject n_node = (GameObject)Instantiate (adventurerer_block, this.transform);
		Adventurer_Block cs = n_node.GetComponent<Adventurer_Block> ();
		cs.Initalize (a);
		Vector3 position = new Vector3 ();
		position.y = Screen.height * .5f;
		position.x = Screen.width * .85f;
		n_node.transform.position = position;
		MovePlayerNodes (player_node_spacing);
		blocks.Add (cs);
	}

	public void RemoveBlock(Adventurer_Block a_b){
		blocks.Remove (a_b);
		if (blocks.Count > 1) {
			if (blocks [0].transform.position.y < screenBottom) { //Make sure that the top block isn't below the bottom of the screen
				Vector3 pos = blocks [0].transform.position;
				pos.y = 0;
				blocks [0].transform.position = pos;
			}
			for (int i = 1; i < blocks.Count; i++) {
				Vector3 pos = blocks [i - 1].transform.position;
				pos.y -= player_node_spacing;
				blocks [i].transform.position = pos;
			}
		}
	}

	public void DisplaySingleLog(Adventurer hero){
		ClearScreen ();
		List<string[]> entries = hero.diary.GetLog ();
		foreach (string[] log in entries) {
			//TEST CODE!
			if (log [1] == Adventurer.Type_Adventurer) {
				left.CreateNewNode (log [0]);
			} else {
				right.CreateNewNode (log [0]);
			}
		}
	}

	public void UpdateScrolling(){
		ClearScreen ();
		List<Adventurer> heroes = GameObject.FindGameObjectWithTag ("Guild").GetComponent<Guild_Manager> ().GetAvailableMembers ();
		foreach (Adventurer hero in heroes) {
			List<string[]> entries = hero.diary.GetLog ();
			foreach (string[] log in entries) {
				//TEST CODE!
				if (log [1] == Adventurer.Type_Adventurer) {
					left.CreateNewNode (log [0]);
				} else {
					right.CreateNewNode (log [0]);
				}
			}
		}
	}
}
