using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Item : MonoBehaviour {

	private float shopFactor;
	private Item my_item;

	// Use this for initialization
	void Start () {
		UnityEngine.UI.Text item_name = GetComponentInChildren<UnityEngine.UI.Text> ();
		item_name.text = my_item._name;
		UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button> ();

		buttons [2].GetComponentInChildren<UnityEngine.UI.Text> ().text = "" + (my_item.cost * shopFactor);
		UnityEngine.UI.Button.ButtonClickedEvent b_event = new UnityEngine.UI.Button.ButtonClickedEvent ();
		b_event.AddListener (PurchaseItem);
		buttons [2].onClick = b_event;

		buttons [0].gameObject.SetActive (false);
		buttons [1].gameObject.SetActive (false);
	}
	
	public void Initalize(Item i, float factor){
		my_item = i;
		shopFactor = factor;
	}

	public void PurchaseItem(){
		//Check to see if money exists
		GameObject.FindGameObjectWithTag("Guild").GetComponent<Guild_Manager>().addItem(my_item);
		print ("bought an item");
	}
}
