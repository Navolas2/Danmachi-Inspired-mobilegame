using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop_up : MonoBehaviour {

	private string myText;
	// Use this for initialization
	void Start () {
		print ("start");
		UnityEngine.UI.Text text_box = GetComponentInChildren<UnityEngine.UI.Text> ();
		text_box.text = myText;
	}

	public void Initalize(string text){
		myText = text;
	}

	public void CloseBox(){
		Destroy (this.gameObject);
	}
}
