using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Floor_Button : MonoBehaviour {

	private bool clickable = true;
	public int floor_number = 0;
	public Floor_Selection parent;
	protected ClickAction clickresponse = NoAction;

	UnityEngine.UI.Button main_button;
	UnityEngine.UI.Button less_button;
	UnityEngine.UI.Button more_button;
	UnityEngine.UI.Text my_text;
	// Use this for initialization
	void Start () {
		
	}

	public void Initalize(){
		UnityEngine.UI.Button[] buttons = this.GetComponentsInChildren< UnityEngine.UI.Button> ();
		main_button = buttons [0];
		less_button = buttons [1];
		more_button = buttons [2];
		my_text = this.GetComponentInChildren<UnityEngine.UI.Text> ();

		UnityEngine.UI.Button.ButtonClickedEvent b_event = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event.AddListener (SetFloor);
		main_button.onClick = b_event;

		UnityEngine.UI.Button.ButtonClickedEvent b_event_1 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_1.AddListener (SetFloorLess);
		less_button.onClick = b_event_1;

		UnityEngine.UI.Button.ButtonClickedEvent b_event_2 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_2.AddListener (SetFloorMore);
		more_button.onClick = b_event_2;
	}

	public void AdjustAlpha(float alpha){ //alpha is a percent of how visible it should be
		Color text_color = my_text.color;
		text_color.a = alpha * 255f;
		my_text.color = text_color;

		UnityEngine.UI.ColorBlock button_color = main_button.colors;
		Color normColor = button_color.normalColor;
		normColor.a = alpha * 255f;
		button_color.normalColor = normColor;
		main_button.colors = button_color;
		more_button.colors = button_color;
		less_button.colors = button_color;

		if (alpha < .25) {
			clickable = false;
		} else {
			clickable = true;
		}
		if (alpha < .01) {
			rendering = false;
		} else {
			rendering = true;
		}
	}

	public static void NoAction(){
		//nothing happens
	}
		
	public void SetFloor(){
		if (clickable)
			parent.SetFloorTarget (floor_number);
	}

	public void SetFloorLess(){
		if (clickable)
			parent.SetFloorTarget (floor_number, '-');
	}

	public void SetFloorMore(){
		if (clickable)
			parent.SetFloorTarget (floor_number, '-');
	}

	public Vector3 position{
		get{ return transform.localPosition; }
		set{ transform.localPosition = value; }
	}

	public bool rendering{
		get{ return gameObject.activeSelf; }
		set{ gameObject.SetActive(value); }
	}
}
