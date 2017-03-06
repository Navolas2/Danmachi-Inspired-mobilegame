using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModification : MonoBehaviour {

	public int stat;
	private static int LOWESTVALUE = 5;
	public CreateCharacter main;
	private UnityEngine.UI.Text displayField;
	// Use this for initialization
	void Start () {
		displayField = GetComponentsInChildren<UnityEngine.UI.Text> () [1];
		displayField.text = "" + main.stat_list[stat];
	}
		
	public void IncreaseStat(){
		if (main.pointChange (1)) {
			List<float> stats = main.stat_list;
			stats [stat]++;
			main.stat_list = stats;
		} else {
			print ("No more points to spend");
		}
		displayField.text = "" + main.stat_list[stat];
	}

	public void DecreaseStat(){
		List<float> stats = main.stat_list;
		if (stats [stat] - 1 >= LOWESTVALUE) {
			if (main.pointChange (-1)) {
				stats [stat]--;
				main.stat_list = stats;
			} else {
				print ("No more points to spend");
			}
		} else {
			print ("stat can't go lower");
		}
		displayField.text = "" + main.stat_list[stat];
	}

	public void UpdateText(){
		displayField.text = "" + main.stat_list[stat];
	}
}
