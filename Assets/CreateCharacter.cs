using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateCharacter : MonoBehaviour {

	Adventurer built;
	private string character_name;
	private Personality p;
	private List<float> stats;
	private static int Max_Points = 25;
	private int AvailablePoints;
	public UnityEngine.UI.Text pointField;
	private Guild_Manager guild;
	// Use this for initialization
	void Awake () {
		guild = GameObject.FindGameObjectWithTag ("Guild").GetComponent<Guild_Manager> ();
		AvailablePoints = Max_Points;
		//pointField = GetComponentsInChildren<UnityEngine.UI.Text> () [2];
		pointField.text = "" + AvailablePoints;
		p = new Personality ();
		stats = new List<float> ();
		for (int i = 0; i < 6; i++) {
			if (i == 4) {
				stats.Add (0); //Magic always defaults to 0
			} else {
				stats.Add (5);
			}
		}
		DontDestroyOnLoad (gameObject);
	}

	public void Built_Character(){
		//Create Dungeon Character
		built = new Adventurer(character_name, p, stats, guild);
		ChangeScene ();
		//UnityEngine.SceneManagement.SceneManager.LoadScene ("Test scne");
	}

	public void ChangeScene(){
		if (built != null) {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("Test scne");
		} else {
			//display error message
		}
	}

	public void Randomize(){
		p.reset ();
		for (int i = 0; i < 6; i++) {
			p.UpdateTrait (Random.Range (-100, 100), i);
		}
		//Reset the stats
		for (int i = 0; i < 6; i++) {
			if (i == 4) {
				stats[i] = 0; //Magic always defaults to 0
			} else {
				stats[i] = 5;
			}
		}
		AvailablePoints = Max_Points;
		for (int i = 0; i < Max_Points; i++) {
			int stat = Random.Range (0, 4);
			if (stat == 4) {
				stat = 5;
			}
			if (pointChange (1)) {
				stats [stat]++;
			}
		}
		GameObject g = GameObject.FindGameObjectWithTag ("StatBlock");
		StatModification[] T = g.GetComponentsInChildren<StatModification> ();
		for (int i = 0; i < T.Length; i++) {
			T [i].UpdateText ();
		}

		character_name = "Random";//TODO Random name generator
	}

	public void setName(string nm){
		character_name = nm;
	}

	public Personality personality {
		get{ return p; }
	}
	
	public List<float> stat_list {
		get{ return stats; }
		set{ stats = value; }
	}
		
	public bool pointChange(int change){
		change *= -1;
		if (AvailablePoints + change >= 0 && AvailablePoints + change <= Max_Points) {
			AvailablePoints += change;
			pointField.text = "" + AvailablePoints;
			return true;
		}
		return false;
	}

	public Adventurer getAdventurer(){
		if (built != null) {
			return built;
		} else {
			return null;
		}
	}
}
