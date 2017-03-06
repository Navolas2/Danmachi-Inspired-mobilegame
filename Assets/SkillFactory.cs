using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class SkillFactory : MonoBehaviour{

	public static SkillFactory Skill_Factory;

	private List<Skill> All_Skills; //make into list of lists

	void Awake(){
		if (Skill_Factory == null) {
			DontDestroyOnLoad (gameObject);
			Skill_Factory = this;

		} else if (Skill_Factory != this) {
			Destroy (gameObject);
		}
	}

	void Start(){
		LoadData ();
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
		All_Skills = new List<Skill> ();
		if (File.Exists (Application.dataPath + "/SkillInformation.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/SkillInformation.xml");
			foreach (XmlNode attack_data in data.DocumentElement.ChildNodes) {
				Skill out_Skill = new Skill();
				bool success = true;
				string name = attack_data.SelectSingleNode ("name").InnerText;
				string descrip = attack_data.SelectSingleNode ("description").InnerText;
				float proc = 0f;
				int value = -1;
				bool action = false;
				bool link = false;
				bool status = false;
				List<string> type = new List<string>();
				List<float>stat = new List<float>();
				List<string>skill = new List<string>();


				success = success && float.TryParse (attack_data.SelectSingleNode ("proc").InnerText, out proc);
				success = success && int.TryParse (attack_data.SelectSingleNode ("value").InnerText, out value);
				success = success && bool.TryParse (attack_data.SelectSingleNode ("hasAction").InnerText, out action);
				success = success && bool.TryParse (attack_data.SelectSingleNode ("hasLink").InnerText, out link);
				success = success && bool.TryParse (attack_data.SelectSingleNode ("hasBuff").InnerText, out status);

				XmlNode requireData = attack_data.SelectSingleNode ("Requirements");
				foreach (XmlNode statReq in requireData.SelectNodes("stats")) {
					float add_float = 0f;
					success = success && float.TryParse (statReq.InnerText, out add_float);
					stat.Add (add_float);
				}

				foreach (XmlNode skillReq in requireData.SelectNodes("skills")) {
					skill.Add (skillReq.InnerText);
				}

				foreach (XmlNode typeData in attack_data.SelectNodes("type")) {
					type.Add (typeData.InnerText);
				}

				if (success) {
					//MULTIPLE LISTS BASED ON RANK
					out_Skill = new Skill(proc, value, type, name, action, link, stat, skill, status, descrip);
				
					if (action) {
						Attack_Action a = AttackFactory.Attack_Factory.parseNode (attack_data.SelectSingleNode ("Action_info"), name);
						out_Skill.setAction (a);
					}

					if (link) {
						List<string> link_skills = new List<string> ();
						foreach (XmlNode link_skill in requireData.SelectSingleNode("link").ChildNodes) {
							link_skills.Add (link_skill.InnerText);
						}
						out_Skill.LinkedNames = link_skills;
					}

					if (status) {
						Status a = Status.parseNode (attack_data.SelectSingleNode ("Buff"));
						out_Skill.effects = a;
					}
					All_Skills.Add (out_Skill);
				}
			}
		}
		else{
			File.Create (Application.dataPath + "/SkillInformation.xml");
			print (Application.dataPath);
		}
	}

	public Skill LearnAction(int level){
		List<Skill> available = new List<Skill> ();
		//Iterate through list adding all monsters fitting requirements to list
		//randomly select a monster
		//have monster adjust stats a bit
		//return monster
		//Spell spawner = available[0];
		return All_Skills[0];
	}
}


