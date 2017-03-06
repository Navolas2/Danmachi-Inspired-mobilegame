using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageToggle : MonoBehaviour {

	private UnityEngine.UI.Image self;
	private bool visible = false;

	// Use this for initialization
	void Start () {
		self = GetComponent<UnityEngine.UI.Image> ();
		self.enabled = visible;
	}

	public void ToggleVisible(){
		visible = !visible;
		self.enabled = visible;
	}
}
