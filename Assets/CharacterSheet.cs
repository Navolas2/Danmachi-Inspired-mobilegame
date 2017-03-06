using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour {

	// Use this for initialization
	void Start(){
		//Vector3 position = new Vector3 ();
		//position.y = Screen.height * .5f;
		//position.y = gameObject.transform.position.y;
		//position.x = Screen.width * .75f; 
		//gameObject.transform.position = position;
	}

	public void Initalize(Adventurer hero){
		UnityEngine.UI.Text[] nodes = GetComponentsInChildren<UnityEngine.UI.Text> ();
		nodes [7].text = "" + hero.name;
		nodes [8].text = hero.hp + " / " + hero.hp_max;
		nodes [9].text = "" + hero.str_shown;
		nodes [10].text = "" + hero.def_shown;
		nodes [11].text = "" + hero.dex_shown;
		nodes [12].text = "" + hero.agi_shown;
		nodes [13].text = "" + hero.mag_shown;
		UnityEngine.UI.Button close = GetComponentInChildren<UnityEngine.UI.Button> ();
		UnityEngine.UI.Button.ButtonClickedEvent b_event = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event.AddListener (CloseSheet);
		close.onClick = b_event;
	}

	public void UpdateSheet(Adventurer hero){
		UnityEngine.UI.Text[] nodes = GetComponentsInChildren<UnityEngine.UI.Text> ();
		nodes [9].text = nodes [9].text + " -> " + hero.str_shown;
		nodes [10].text = nodes [10].text + " -> " + hero.def_shown;
		nodes [11].text = nodes [11].text + " -> " + hero.dex_shown;
		nodes [12].text = nodes [12].text + " -> " + hero.agi_shown;
		nodes [13].text = nodes [13].text + " -> " + hero.mag_shown;
	}

	public void CloseSheet(){
		Destroy (this.gameObject);
	}

	public Vector3 position{
		get{ return transform.position; }
		set{ transform.position = value; }
	}

	public bool rendering{
		get{ return this.gameObject.activeSelf; }
		set{ gameObject.SetActive(value); }
	}
}
