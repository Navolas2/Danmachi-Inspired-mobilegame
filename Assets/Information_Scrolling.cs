using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Information_Scrolling : MonoBehaviour {

	//private UnityEngine.UI.Text myText;
	private List<TextNode> text_objects;
	public GameObject text_node;
	public int side = 0;
	private float screenTop;
	private float screenBottom = -10;
	// Use this for initialization
	void Start () {
		if (text_node.GetComponent<TextNode> () == null) {
			print ("IMPROPER GAME OBJECT AS TEXT NODE FOR " + this.gameObject + "!!!");
			throw new UnityException ("game object was wrong, quitting");
		}
		TextNode[] myText = GetComponentsInChildren<TextNode> ();
		text_objects = new List<TextNode> ();
		foreach (TextNode tn in myText) {
			tn._parent = this;
			text_objects.Add (tn);
		}

		//myText.text = "I AM TEXT";
		screenTop = (float)Screen.height;
		float screenWidth = (float)Screen.width;
		Vector3 myloc = this.transform.position;
		myloc.y = 10;
	

		myloc.x = (screenWidth / 2) + (170 * side);
		this.transform.position = myloc;
	}
	
	public void Clear(){
		for (int i = text_objects.Count - 1; i > -1  ; i--) {
			TextNode remNode = text_objects[i];
			text_objects.Remove (remNode);
			Destroy (remNode.gameObject);
		}
	}

	public void MoveNodes(float move)
	{
		foreach(TextNode tn in text_objects){
			Vector3 loc = tn.position;
			loc.y += move;
			tn.position = loc;

			if (loc.y < screenBottom || loc.y > screenTop) {
				tn.rendering = false;
			} else {
				tn.rendering = true;
			}
		}
	}

	public void CreateNewNode(string information){
		GameObject n_node = (GameObject)Instantiate (text_node, this.transform);
		n_node.transform.position = this.transform.position;
		MoveNodes (n_node.GetComponent<RectTransform>().rect.height);
		TextNode n_text = n_node.GetComponent<TextNode> ();
		n_text.SetUp (information);
		n_text._parent = this;
		text_objects.Add (n_text);

	}
}
