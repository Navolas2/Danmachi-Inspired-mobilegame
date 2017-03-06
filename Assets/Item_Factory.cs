using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Item_Factory : MonoBehaviour {

	public static Item_Factory The_Item_Factory;

	private List<List<Item>> All_Items; //make into list of lists

	void Awake(){
		if (The_Item_Factory == null) {
			DontDestroyOnLoad (gameObject);
			The_Item_Factory = this;
			LoadData ();
		} else if (The_Item_Factory != this) {
			Destroy (gameObject);
		}
	}

	//FORM FOR SPELL XML
	/*
		<ITEM>
			<name></name>
			<></speed>
			<hit></hit>
			<cost></cost>
			<damage></damage>
			<focus></focus>
			<effect></effect> (NONE if the spell has no effect)
			<affinity></affinity>
			<type></type>
			<level></level>
		</SPELL>
				*/

	private void LoadData(){
		All_Items= new List<List<Item>> ();
		All_Items.Add (BuildHealingItems ());
	}

	private List<Item> BuildHealingItems(){
		List<Item> heals = new List<Item> ();
		if (File.Exists (Application.dataPath + "/Heal_Item_Information.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/Heal_Item_Information.xml");
			foreach (XmlNode item_data in data.DocumentElement.ChildNodes) {
				bool success = true;
				string _name = item_data.SelectSingleNode ("name").InnerText;
				int health_type = 0;
				float health_amount = 0f;
				float price = 0f;
				int mana_type = 0;
				float mana_amount = 0f;
				success = success && int.TryParse (item_data.SelectSingleNode ("health_type").InnerText, out health_type);
				success = success && float.TryParse (item_data.SelectSingleNode ("health_amount").InnerText, out health_amount);
				success = success && int.TryParse (item_data.SelectSingleNode ("mana_type").InnerText, out mana_type);
				success = success && float.TryParse (item_data.SelectSingleNode ("mana_amount").InnerText, out mana_amount);
				success = success && float.TryParse (item_data.SelectSingleNode ("price").InnerText, out price);
				if (success) {
					//MULTIPLE LISTS BASED ON RANK
					Item new_item;
					if (mana_type == 0 && health_type != 0) {
						new_item = new HealingItem (_name, health_type, health_amount, price);
					} else if (mana_type != 0 && health_type == 0) {
						new_item = new HealingItem (_name, mana_type, mana_amount, price, "");
					} else {
						new_item = new HealingItem (_name, health_type, health_amount, mana_type, mana_amount, price);
					}
					heals.Add(new_item);
				}
				//iterate though list getting each monster. and information for monsters
			}
		}
		else{
			File.Create (Application.dataPath+ "/Heal_Item_Information.xml");
			print (Application.dataPath);
		}
		return heals;
	}

	public List<Item> PullItem(string type){
		List<Item> available = new List<Item>();
		switch (type) {
		case "HEAL": //Make sure to change this code if Item type labels change at all!
		case "MANA":
		case "DUAL":
			available.AddRange(All_Items [0]); //Healing items are the first item in the list
			break;
		case "DROP":
			available.AddRange( All_Items [1]); //All Drop items are the second item in the list
			available.AddRange (All_Items [2]); //All Magic stones are the third item in the list. Magic stones are also considered a drop
			break;
		case "STONE":
			available.AddRange (All_Items [2]); //All Magic stones are the third item in the list
			break;
		}

		return available;
	}

	public Item GetItem(string name, string type){
		List<Item> of_type = PullItem (type);
		Item i = of_type.Find (delegate(Item obj) {
			return obj._name == name;
		});
		return i;
	}
}
