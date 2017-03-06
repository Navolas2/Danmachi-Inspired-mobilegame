using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Manager : MonoBehaviour {

	private static float shop_top = 80f;
	private static float shop_bottom = -130f;
	private int current_top;
	public float Shop_Price_Factor = 1f;
	public Shop_Item Item_form;
	UnityEngine.UI.Slider slide;

	List<Shop_Item> items;
	// Use this for initialization
	void Start () {
		slide = GetComponentInChildren<UnityEngine.UI.Slider> ();
		items = new List<Shop_Item> ();
		Vector3 loc = new Vector3 ();
		loc.x = Screen.width / 2;
		loc.y = Screen.height / 2;
		BuiltItemList (Item.Item_Type_Heal);
	}

	private void BuiltItemList(string item_type){
		List<Item> shop_items = Item_Factory.The_Item_Factory.PullItem (item_type);
		for (int i = 0; i < shop_items.Count; i++) {
			Shop_Item n_item = Instantiate (Item_form, this.transform);
			n_item.Initalize (shop_items[i], Shop_Price_Factor);
			Vector3 loc = new Vector3 (0, 80);
			loc.y -= 30 * (i < 8 ? i : 8);
			n_item.transform.localPosition = loc;
			if(i >= 8)
				n_item.gameObject.SetActive (false);
			items.Add (n_item);
		}
		current_top = 0;
		slide.maxValue = (items.Count - 8 >= 0) ? items.Count - 8 : 0;
		slide.value = 0;
		slide.GetComponentsInChildren<UnityEngine.UI.Image> () [2].gameObject.SetActive (items.Count <= 8 ? false : true);
	}

	public void ScrollMenu(float value){
		int iterations =  ((int)value) - current_top;
		while (iterations != 0) {
			int index_top = current_top + (iterations > 0 ? 1 : -1);

			float movement = index_top > current_top ? 30f : -30f;
			for (int i = index_top; i < index_top + 8; i++) {
				Vector3 loc = items [i].transform.position;
				loc.y += movement;
				items [i].transform.position = loc;
			}
			if (index_top > current_top) {
				Vector3 loc = items [current_top].transform.position;
				loc.y += movement;
				items [current_top].transform.position = loc;
				items [current_top].gameObject.SetActive (false);
				items [index_top + 7].gameObject.SetActive (true);
			} else {
				items [index_top].gameObject.SetActive (true);
				Vector3 loc = items [index_top + 8].transform.position;
				loc.y += movement;
				items [index_top + 8].transform.position = loc;
				items [index_top + 8].gameObject.SetActive (false);
			}
			current_top = index_top;
			iterations = ((int)value) - current_top;
		}
	}

	public void CloseShop(){
		Destroy (this.gameObject);
	}
}
