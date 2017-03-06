using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Maker : MonoBehaviour {

	private static float One_Option_Height = 180;
	private static float Three_Option_Height = 300;
	private static float Width = 350;

	private bool OptionStatus;
	private int goal_type;

	private DungeonAdventurer goalSeeker;
	private UnityEngine.UI.Dropdown main;
	private UnityEngine.UI.Text Option1_label;
	private UnityEngine.UI.Dropdown Option1;
	private UnityEngine.UI.Text Option2_label;
	private UnityEngine.UI.Dropdown Option2;
	private UnityEngine.UI.Text TimeOption_Label;
	private UnityEngine.UI.Dropdown TimeOption;

	private UnityEngine.UI.Image background;
	private UnityEngine.UI.Image border;

	private UnityEngine.UI.Button completeButton;
	private UnityEngine.UI.Button closeButton;
	// Use this for initialization
	void Start ()
	{
		OptionStatus = true;
		goal_type = 0;

		UnityEngine.UI.Dropdown[] drops = GetComponentsInChildren<UnityEngine.UI.Dropdown> ();
		main = drops [0];
		Option1 = drops [1];
		Option2 = drops [2];
		TimeOption = drops [3];

		UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text> ();
		Option1_label = texts [2];
		Option2_label = texts [3];
		TimeOption_Label = texts [4];

		UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image> ();
		background = images [0];
		Rect r = background.rectTransform.rect;
		r.width = Width;
		background.rectTransform.sizeDelta = new Vector2 (r.width, r.height);
		border = images [1];
		border.rectTransform.sizeDelta = new Vector2 (r.width, r.height);

		completeButton = GetComponentInChildren<UnityEngine.UI.Button> ();

		UnityEngine.UI.Button.ButtonClickedEvent b_event = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event.AddListener (BuiltGoal);
		completeButton.onClick = b_event;

		closeButton = GetComponentsInChildren<UnityEngine.UI.Button> () [1];
		UnityEngine.UI.Button.ButtonClickedEvent b_event_1 = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event_1.AddListener (CloseBuilder);
		closeButton.onClick = b_event_1;

		ToggleOptions (false);

		CreateTimeGoalList ();
		//Check to see if Goal is already set
		//IF set, set stats to be for current goal

	}

	public void Initialize(DungeonAdventurer da){
		goalSeeker = da;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	private void ToggleOptions(bool newStatus){
		if (newStatus == false && OptionStatus != false) {
			Rect r = background.rectTransform.rect;
			r.height = One_Option_Height;
			background.rectTransform.sizeDelta = new Vector2 (r.width, r.height);
			border.rectTransform.sizeDelta = new Vector2 (r.width, r.height);

			main.transform.localPosition = new Vector3 (0, 60);

			Option1.gameObject.SetActive (false);
			Option1_label.gameObject.SetActive (false);
			Option2.gameObject.SetActive (false);
			Option2_label.gameObject.SetActive (false);

			TimeOption_Label.gameObject.transform.localPosition = new Vector3 (TimeOption_Label.transform.localPosition.x, -0, 0);
			//TimeOption_Label.transform.position = new Vector3 (TimeOption_Label.transform.position.x, -60, 0);
			TimeOption.gameObject.transform.localPosition = new Vector3 (TimeOption.transform.localPosition.x, -0, 0);

			completeButton.transform.localPosition = new Vector3 (0, -60, 0);
			closeButton.transform.localPosition = new Vector3 (156, 71, 0);
		} 
		else if (newStatus == true && OptionStatus != true) {
			Rect r = background.rectTransform.rect;
			r.height = Three_Option_Height;
			background.rectTransform.sizeDelta = new Vector2 (r.width, r.height);
			border.rectTransform.sizeDelta = new Vector2 (r.width, r.height);

			main.transform.localPosition = new Vector3 (0, 120);

			Option1.gameObject.SetActive (true);
			Option1_label.gameObject.SetActive (true);
			Option2.gameObject.SetActive (true);
			Option2_label.gameObject.SetActive (true);

			TimeOption_Label.gameObject.transform.localPosition = new Vector3 (TimeOption_Label.transform.localPosition.x, -60, 0);
			TimeOption.gameObject.transform.localPosition = new Vector3 (TimeOption.transform.localPosition.x, -60, 0);

			completeButton.transform.localPosition = new Vector3 (0, -120, 0);
			closeButton.transform.localPosition = new Vector3 (156, 131, 0);
		}
		OptionStatus = newStatus;
	}

	public void OnGoalChanged(int value){
		goal_type = value;

		switch (value) {
		case 0:
			ToggleOptions (false);
			break;
		case 1:
			ToggleOptions (true);
			CreateKillGoalList ();
			break;
		}
	}

	/*
	//GOAL LIST CREATION
	*/

	public void BuiltGoal(){
		Multi_Goal g_out = new Multi_Goal();
		switch (goal_type) {
		case 0: //TimeBased Goal
			if (TimeOption.value != 0) {
				if (TimeOption.value < GameClock.The_Clock.hours_in_day) {
					g_out.AddGoal(new Time_Goal (0, TimeOption.value));
				} else {
					g_out.AddGoal(new Time_Goal (0, 0, (TimeOption.value + 1) - GameClock.The_Clock.hours_in_day));
				}
			}
			break;
		case 1: //KillBased Goal
			if (TimeOption.value != 0) {
				if (TimeOption.value < GameClock.The_Clock.hours_in_day) {
					g_out.AddGoal (new Time_Goal (0, TimeOption.value));
				} else {
					g_out.AddGoal (new Time_Goal (0, 0, (TimeOption.value + 1) - GameClock.The_Clock.hours_in_day));
				}
			}
			if (Option2.value == 0) {
				g_out.AddGoal (new Kill_Goal (Option1.value));
			} else {
				g_out.AddGoal (new Kill_Goal (Option2.options [Option2.value].text, Option1.value));
			}
			break;
		}

		if (!g_out.IsEmpty ()) {
			goalSeeker.setGoal (g_out);
			//GameObject.FindGameObjectWithTag ("Guild").GetComponent<Guild_Manager> ().SendToDungeon ((Adventurer)goalSeeker.attached_unit);
			Destroy (this.gameObject);
		}
	}

	private void CreateTimeGoalList(){
		List<UnityEngine.UI.Dropdown.OptionData> options = new List<UnityEngine.UI.Dropdown.OptionData> ();
		options.Add (new UnityEngine.UI.Dropdown.OptionData ("Until Finished"));
		for (int i = 1; i < GameClock.The_Clock.hours_in_day; i++) {
			options.Add (new UnityEngine.UI.Dropdown.OptionData (i + " hours"));
		}
		for (int i = 1; i < 10; i++) {
			options.Add (new UnityEngine.UI.Dropdown.OptionData (i + " days"));
		}
		TimeOption.options = options;
		TimeOption_Label.text = "Time in Dungeon";
	}

	private void CreateKillGoalList(){
		Option1_label.text = "Amount to Kill:";
		List<UnityEngine.UI.Dropdown.OptionData> options = new List<UnityEngine.UI.Dropdown.OptionData> ();
		for (int i = 0; i < 60; i++) {
			options.Add (new UnityEngine.UI.Dropdown.OptionData (i + ""));
		}
		Option1.options = options;
		Option2_label.text = "Monster to kill:";
		List<UnityEngine.UI.Dropdown.OptionData> options_2 = new List<UnityEngine.UI.Dropdown.OptionData> ();
		/*for (int i = 0; i < 60; i++) {
			options.Add (new UnityEngine.UI.Dropdown.OptionData (i + ""));
		}*/
		//Build list based on known monsters for floor range
		options_2.Add(new UnityEngine.UI.Dropdown.OptionData("Any"));
		options_2.Add(new UnityEngine.UI.Dropdown.OptionData("Goblin"));
		Option2.options = options_2;
	}

	public void CloseBuilder(){
		Destroy (this.gameObject);
	}
}
