using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;


public class AttackFactory : MonoBehaviour{
	
	public static AttackFactory Attack_Factory;

	private List<Attack_Action> All_Actions; //make into list of lists

	void Awake(){
		if (Attack_Factory == null) {
			DontDestroyOnLoad (gameObject);
			Attack_Factory = this;
			LoadData ();
		} else if (Attack_Factory != this) {
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
		All_Actions = new List<Attack_Action> ();
		if (File.Exists (Application.dataPath + "/AttackInformation.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/AttackInformation.xml");
			foreach (XmlNode attack_data in data.DocumentElement.ChildNodes) {
				bool success = true;
				string name = attack_data.SelectSingleNode ("name").InnerText;
				string message = attack_data.SelectSingleNode ("message").InnerText;
				List<float> hit = new List<float> ();
				List<float> damage = new List<float> ();
				List<string> affinity = new List<string> ();
				float code = 0f;
				bool s_tar = true;
				bool has_status = false;
				int pause = 0;
				float cost = 0f;
				int volume = 0;
				List<int> stats = new List<int>();

				success = success && float.TryParse (attack_data.SelectSingleNode ("code").InnerText, out code);
				success = success && bool.TryParse (attack_data.SelectSingleNode ("single_target").InnerText, out s_tar);
				success = success && bool.TryParse (attack_data.SelectSingleNode ("has_status").InnerText, out has_status);
				success = success && int.TryParse (attack_data.SelectSingleNode ("delay").InnerText, out pause);
				success = success && float.TryParse (attack_data.SelectSingleNode ("cost").InnerText, out cost);
				success = success && int.TryParse (attack_data.SelectSingleNode ("volume").InnerText, out volume);

				foreach (XmlNode hitData in attack_data.SelectSingleNode("hit").ChildNodes) {
					float add_float = 0f;
					success = success && float.TryParse (hitData.InnerText, out add_float);
					hit.Add (add_float);
				}

				foreach (XmlNode damageData in attack_data.SelectSingleNode("damage").ChildNodes) {
					float add_float = 0f;
					success = success && float.TryParse (damageData.InnerText, out add_float);
					damage.Add (add_float);
				}

				foreach (XmlNode elemData in attack_data.SelectSingleNode("affinity").ChildNodes) {
					affinity.Add (elemData.InnerText);
				}

				foreach (XmlNode elemData in attack_data.SelectSingleNode("stats").ChildNodes) {
					int value = 0;
					int.TryParse (elemData.InnerText, out value);
					stats.Add (value);
				}

				if (success) {
					//MULTIPLE LISTS BASED ON RANK
					Attack_Action a = new Attack_Action (name, hit, damage, code, s_tar, affinity, pause, cost, message, has_status, volume);
					if (has_status) {
						a.Attach_Status(Status.parseNode(attack_data.SelectSingleNode("STATUS")));
					}
					a.StatGains = stats.ToArray ();
					All_Actions.Add (a);
				}
				//iterate though list getting each attack action. and information for each attack
			}
		}
		else{
			File.Create (Application.dataPath + "/AttackInformation.xml");
			print (Application.dataPath);
		}
	}

	public Attack_Action parseNode(XmlNode attack_data, string name){
		string message = attack_data.SelectSingleNode ("message").InnerText;
		bool success = true;
		List<float> hit = new List<float> ();
		List<float> damage = new List<float> ();
		List<string> affinity = new List<string> ();
		float code = 0f;
		bool s_tar = true;
		int pause = 0;
		float cost = 0f;
		int volume = 0;
		bool status = false;

		success = success && float.TryParse (attack_data.SelectSingleNode ("code").InnerText, out code);
		success = success && bool.TryParse (attack_data.SelectSingleNode ("single_target").InnerText, out s_tar);
		success = success && int.TryParse (attack_data.SelectSingleNode ("delay").InnerText, out pause);
		success = success && float.TryParse (attack_data.SelectSingleNode ("cost").InnerText, out cost);
		success = success && bool.TryParse (attack_data.SelectSingleNode ("has_status").InnerText, out status);
		success = success && int.TryParse (attack_data.SelectSingleNode ("volume").InnerText, out volume);

		foreach (XmlNode hitData in attack_data.SelectSingleNode("hit").ChildNodes) {
			float add_float = 0f;
			success = success && float.TryParse (hitData.InnerText, out add_float);
			hit.Add (add_float);
		}

		foreach (XmlNode damageData in attack_data.SelectSingleNode("damage").ChildNodes) {
			float add_float = 0f;
			success = success && float.TryParse (damageData.InnerText, out add_float);
			damage.Add (add_float);
		}

		foreach (XmlNode elemData in attack_data.SelectSingleNode("affinity").ChildNodes) {
			affinity.Add (elemData.InnerText);
		}

		Attack_Action a_out;
		if (success) {
			//MULTIPLE LISTS BASED ON RANK
			a_out = new Attack_Action(name, hit, damage, code, s_tar, affinity, pause, cost, message, status, volume);
			if (status) {
				a_out.Attach_Status(Status.parseNode(attack_data.SelectSingleNode("STATUS")));
			}
			return a_out;
		}
		return null;
	}

	public Attack_Action LearnAction(int level){
		List<Attack_Action> available = new List<Attack_Action> ();
		//Iterate through list adding all monsters fitting requirements to list
		//randomly select a monster
		//have monster adjust stats a bit
		//return monster
		//Spell spawner = available[0];
		return new Attack_Action(All_Actions[level]);
	}
}


