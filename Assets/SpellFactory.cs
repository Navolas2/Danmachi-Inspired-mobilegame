using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SpellFactory : MonoBehaviour{
	public static SpellFactory Spell_Factory;

	private List<Spell> All_Spells; //make into list of lists

	void Awake(){
		if (Spell_Factory == null) {
			DontDestroyOnLoad (gameObject);
			Spell_Factory = this;
			LoadData ();
		} else if (Spell_Factory != this) {
			Destroy (gameObject);
		}
	}

	//FORM FOR SPELL XML
	/*
		<SPELL>
			<name></name>
			<speed></speed>
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
		All_Spells = new List<Spell> ();
		if (File.Exists (Application.dataPath + "/SpellInformation.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/SpellInformation.xml");
			foreach (XmlNode spell_data in data.DocumentElement.ChildNodes) {
				bool success = true;
				string _name = spell_data.SelectSingleNode ("name").InnerText;
				int speed = 0;
				float cost = 0f;
				float hit = 0f;
				float damage = 0f;
				float focus = 0f;
				int level = 0;
				string affinity = spell_data.SelectSingleNode ("affinity").InnerText;
				string type = spell_data.SelectSingleNode ("type").InnerText;
				string effect = spell_data.SelectSingleNode ("effect").InnerText;
				bool self = false;
				success = success && int.TryParse (spell_data.SelectSingleNode ("speed").InnerText, out speed);
				success = success && float.TryParse (spell_data.SelectSingleNode ("hit").InnerText, out hit);
				success = success && float.TryParse (spell_data.SelectSingleNode ("damage").InnerText, out damage);
				success = success && float.TryParse (spell_data.SelectSingleNode ("focus").InnerText, out focus);
				success = success && float.TryParse (spell_data.SelectSingleNode ("cost").InnerText, out cost);
				success = success && int.TryParse (spell_data.SelectSingleNode ("level").InnerText, out level);
				success = success && bool.TryParse (spell_data.SelectSingleNode ("self").InnerText, out self);
				if (success) {
					//MULTIPLE LISTS BASED ON RANK
					All_Spells.Add(new Spell(_name, speed, hit, damage, focus, effect, level,cost, affinity, type, self));
				}
				//iterate though list getting each monster. and information for monsters
			}
		}
		else{
			File.Create (Application.dataPath+ "/SpellInformation.xml");
			print (Application.dataPath);
		}
	}

	public Spell LearnSpell(int level){
		List<Spell> available = new List<Spell> ();
		//Iterate through list adding all monsters fitting requirements to list
		//randomly select a monster
		//have monster adjust stats a bit
		//return monster
		//Spell spawner = available[0];
		return All_Spells[0];
	}
}


