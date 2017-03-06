using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Floor_Selection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {

	public Floor_Button template_button;
	private Adventurer setter;
	private List<Floor_Button> list_buttons;

	private Vector2 start_position;
	private Vector2 last_position;
	private int drag_time;
	private int last_drag = 0;
	private float velocity;
	private float friction = .9f;
	// Use this for initialization
	void Start () {
		//TESTCODE
		/*
		list_buttons = new List<Floor_Button>();
		for (int i = 0; i < 10; i++) {
			Floor_Button mb = Instantiate (template_button, this.transform);
			mb.GetComponentInChildren<UnityEngine.UI.Text> ().text = "tester button";
			//UnityEngine.UI.Button.ButtonClickedEvent event_button = new UnityEngine.UI.Button.ButtonClickedEvent ();
			mb.parent = this;
			mb.GetComponent<Pointer_Child> ().Parent_index = 2;
			mb.floor_number = i + 1;
			mb.transform.localPosition = new Vector3 (0, -40 * i);
			list_buttons.Add (mb);
		}
		*/
	}

	public void Initalize(Adventurer hero){
		setter = hero;
		List<int> entered = new List<int> ();
		hero.FloorExploringInformation (out entered);
		List<string> dungeon_names = Dungeon.The_Dungeon.GetFloorNames ();
		list_buttons = new List<Floor_Button>();
		Floor_Button mb;
		float alpha_value;
		int i;
		for (i = 0; i < entered.Count; i++) {
			mb = Instantiate (template_button, this.transform);
			mb.Initalize ();
			mb.GetComponentInChildren<UnityEngine.UI.Text> ().text = "Floor " + (i + 1) + ": ";// + dungeon_names [i];
			mb.parent = this; 
			mb.floor_number = i + 1;
			mb.transform.localPosition = new Vector3 (0, -40 * i);
			list_buttons.Add (mb);
			if (Mathf.Abs (mb.transform.localPosition.y) < 100) {
				alpha_value = 1 - (mb.transform.localPosition.y / 100);
			} else {
				alpha_value = 0;
			}


			mb.AdjustAlpha (alpha_value);
		}

		mb = Instantiate (template_button, this.transform);
		mb.Initalize ();
		mb.GetComponentInChildren<UnityEngine.UI.Text> ().text = "Floor " + (entered.Count + 1) + ": UNENTERED FLOOR";
		mb.parent = this;
		mb.floor_number = (entered.Count + 1);
		list_buttons.Add (mb);
		mb.transform.localPosition = new Vector3 (0, -40 * i);
		if (Mathf.Abs (mb.transform.localPosition.y) < 100) {
			alpha_value = 1 - (mb.transform.localPosition.y / 100);
		} else {
			alpha_value = 0;
		}
		mb.AdjustAlpha (alpha_value);

/*		if (list_buttons.Count == 0) {
			Floor_Button mb = Instantiate (template_button, this.transform);
			mb.Initalize ();
			mb.GetComponentInChildren<UnityEngine.UI.Text> ().text = "Floor 1: UNENTERED FLOOR";
			mb.parent = this;
			mb.floor_number = 1;
			mb.GetComponent<Pointer_Child> ().Parent_index = 2;
			mb.transform.localPosition = new Vector3 (0, 0);
			list_buttons.Add (mb);
		}

*/	}

	// Update is called once per frame
	void Update () {
		last_drag++;
		if (velocity > .001 || velocity < -.001) {
			MoveNodes (velocity);
			velocity *= friction;
		}
	}

	private void MoveNodes(float move){
		if (!((list_buttons [0].transform.localPosition.y <= 0 && move < 0) || (list_buttons [list_buttons.Count - 1].transform.localPosition.y >= 0 && move > 0))) {
			for (int i = 0; i < list_buttons.Count; i++) {
				int prev = i - 1;
				if (prev == -1) {
					prev = list_buttons.Count - 1;
				}
				Floor_Button tn = list_buttons [i];
				Vector3 loc = tn.position;
				loc.y += move;

				float alpha_value;
				if (Mathf.Abs (loc.y) < 100) {
					alpha_value = 1 - (loc.y / 100);
				} else {
					alpha_value = 0;
				}

				tn.AdjustAlpha (alpha_value);


				tn.position = loc;
			}
		}
	}

	public void SetFloorTarget (int floor_number){
		setter.explorer.SetFloorTarget (floor_number);
		CloseBox ();
	}

	public void SetFloorTarget (int floor_number, char note){
		setter.explorer.SetTargetLocation (new coordinate(floor_number, note));
		CloseBox ();
	}

	public void OnDrag(PointerEventData data){
		//stuff here that hasn't been implemented yet
		//MoveNodes(data.position.y - last_position.y > 5 ? 5 : data.position.y - last_position.y);
		//last_position = data.position;
		drag_time++;
		if (last_drag > 10) {
			start_position = data.position;
			drag_time = 0;
		}
		last_drag = 0;
	}

	public void OnPointerDown(PointerEventData data){
		velocity = 0;
		start_position = data.position;
		last_position = start_position;
		drag_time = 0;
		last_drag = 0;
	}

	public void OnPointerUp(PointerEventData data){

		float difference = data.position.y - start_position.y;
		difference = difference / drag_time;
		if (last_drag < 10) {
			velocity = difference;
		}
	}

	public void CloseBox(){
		Destroy (this.gameObject);
	}
}
