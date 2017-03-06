using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	private List<Menu_Button> menu_options;
	private float screenTop;
	private float screenBottom = -10;
	public int spacing = 100;
	private int button_count;
	private int buttons_on_screen;
	public Menu_Button button;
	private UnityEngine.UI.Scrollbar menu_scrollbar;
	private float prevValue = 0;

	// Use this for initialization
	void Start () {
		screenTop = (float)Screen.height;
		if (menu_options == null) {
			menu_options = new List<Menu_Button> ();
		}
		buttons_on_screen = Mathf.FloorToInt(Screen.height / 100);
		UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
		BuildMenu ();
	}

	public void SceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1){
		menu_options.Clear ();
		menu_scrollbar = null;
		button_count = 0;
		BuildMenu ();
	}

	private void BuildMenu(){
		GameObject screen_canvas = GameObject.FindGameObjectWithTag ("Canvas");
		if (screen_canvas != null) {
			int[] options = screen_canvas.GetComponent<ScreenManager> ().menu_options;
			for (int i = 0; i < options.Length; i++) {
				Menu_Button mb = Instantiate (button, screen_canvas.transform);
				mb.Menu_option = options [i];
				mb.transform.position = new Vector3 (Screen.width * .15f, Screen.height - 50 - (spacing * i));
			}
			button_count = options.Length;
			if (menu_scrollbar != null) {
				SetUpScrollBar ();
			}
		}
	}

	public void LinkScrollBar(UnityEngine.UI.Scrollbar scroller){
		menu_scrollbar = scroller;
		if (button_count != 0) {
			SetUpScrollBar ();
		}
	}

	private void SetUpScrollBar(){
		if (button_count <= buttons_on_screen) {
			menu_scrollbar.size = 1;
		} else {
			menu_scrollbar.size = .4f;
		}
	}

	public void AddSelf(Menu_Button button){
		if (menu_options == null) {
			menu_options = new List<Menu_Button> ();
		}
		menu_options.Add (button);
	}

	public void CreateCharacter(){
		UnityEngine.SceneManagement.SceneManager.LoadScene ("Character_creation_scene");
	}
		
	public void MoveNodes(float value)
	{
		for(int i = 0; i < menu_options.Count; i++){
			int prev = i - 1;
			if (prev == -1) {
				prev = menu_options.Count - 1;
			}
			Menu_Button tn = menu_options[i];
			Vector3 loc = tn.position;
			loc.y += (value - prevValue) * (100 * (button_count - buttons_on_screen));

			if (loc.y < screenBottom || loc.y > screenTop) {
				tn.rendering = false;
			} else {
				tn.rendering = true;
			}
				


			tn.position = loc;

		}
		prevValue = value;

	}

	public ClickAction GetAction(int item, out string text){
		text = "No action";
		ClickAction action = Menu_Button.NoAction;
		switch (item)
		{
		case 0:
			action = Dungeon.The_Dungeon.ReturnToGuild;
			text = "Return to Guild";
			break;
		case 1:
			action = CreateCharacter;
			text = "Create Character";
			break;
		case 2:
			action = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().UpdateScrolling;
			text = "Print all Dungeon Logs";
			break;
		case 3:
			action = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().OpenMarket;
			text = "Open Market";
			break;
		case 4:
			action = Dungeon.The_Dungeon.StartEncounter;
			text = "Start Encounter";
			break;
		case 5:
			action = GameObject.FindGameObjectWithTag ("Canvas").GetComponent<ScreenManager> ().ClearScreen;
			text = "Clear Center Screen";
			break;
		case 6:
			//action = GameObject.FindGameObjectWithTag ("Guild").GetComponent<ScreenManager> ().CharacterList;
			text = "UNASSIGNED VALUE";
			break;
		case 7:
			action = GameObject.FindGameObjectWithTag ("Guild").GetComponent<Guild_Manager> ().HealGuild;
			text = "Heal Guild Members";
			break;

		}

		return action;
	}
}
