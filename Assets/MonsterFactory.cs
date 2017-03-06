using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class MonsterFactory : MonoBehaviour{
	public static MonsterFactory Monster_Factory;

	private List<Monster> Monsters; //make into list of lists

	void Awake(){
		if (Monster_Factory == null) {
			DontDestroyOnLoad (gameObject);
			Monster_Factory = this;
			LoadData ();
		} else if (Monster_Factory != this) {
			Destroy (gameObject);
		}
	}

	//FORM FOR MONSTER XML
	/*
		<MONSTER>
			<name></name>
			<minRange></minRange>
			<maxRange></maxRange>
			<strength></strength>
			<defense></defense>
			<dextarity></dextarity>
			<agility></agility>
			<magic></magic>
			<health></health>
			<rank></rank>
		<MONSTER>
				*/

	private void LoadData(){
		Monsters = new List<Monster> ();
		if (File.Exists (Application.dataPath + "/MonsterInformation.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/MonsterInformation.xml");
			foreach (XmlNode monster in data.DocumentElement.ChildNodes) {
				bool success = true;
				string _name = monster.SelectSingleNode ("name").InnerText;
				int _minRange = 0;
				int _maxRange = 0;
				float _strength = 0;
				float _defense = 0;
				float _dextarity = 0;
				float _agility = 0;
				float _resistance = 0;
				float _health = 0;
				int _rank = 0;

				bool _wise = false;
				bool _group = true;
				bool _aggressive = false;
				bool _breath = false;
				int _legs = 4;
				List<string> affinities = new List<string> ();
				List<string> weakness = new List<string> ();
				List<string> drops = new List<string> ();

				success = success && int.TryParse (monster.SelectSingleNode ("minRange").InnerText, out _minRange);
				success = success && int.TryParse (monster.SelectSingleNode ("maxRange").InnerText, out _maxRange);
				success = success && float.TryParse (monster.SelectSingleNode ("strength").InnerText, out _strength);
				success = success && float.TryParse (monster.SelectSingleNode ("defense").InnerText, out _defense);
				success = success && float.TryParse (monster.SelectSingleNode ("dextarity").InnerText, out _dextarity);
				success = success && float.TryParse (monster.SelectSingleNode ("agility").InnerText, out _agility);
				success = success && float.TryParse (monster.SelectSingleNode ("magic").InnerText, out _resistance);
				success = success && float.TryParse (monster.SelectSingleNode ("health").InnerText, out _health);
				success = success && int.TryParse (monster.SelectSingleNode ("rank").InnerText, out _rank);
				success = success && bool.TryParse (monster.SelectSingleNode ("wise").InnerText, out _wise);
				success = success && bool.TryParse (monster.SelectSingleNode ("breath").InnerText, out _breath);
				success = success && bool.TryParse (monster.SelectSingleNode ("group").InnerText, out _group);
				success = success && bool.TryParse (monster.SelectSingleNode ("aggressive").InnerText, out _aggressive);
				success = success && int.TryParse (monster.SelectSingleNode ("legs").InnerText, out _legs);

				foreach (XmlNode elemData in monster.SelectSingleNode("affinity").ChildNodes) {
					affinities.Add (elemData.InnerText);
				}

				foreach (XmlNode elemData in monster.SelectSingleNode("weakness").ChildNodes) {
					weakness.Add (elemData.InnerText);
				}

				if (success) {
					//MULTIPLE LISTS BASED ON RANK
					Monster m = new Monster (_minRange, _maxRange, _name, _strength,
						_defense, _dextarity, _agility, _resistance, _health, _rank, _wise, _group, _aggressive, _breath, _legs, affinities, weakness, drops);
					Monsters.Add (m);
				}
				//iterate though list getting each monster. and information for monsters
			}
		}
		else{
			File.Create (Application.dataPath + "/MonsterInformation.xml");
			print (Application.dataPath);
		}
	}

	public Monster SpawnMonster(int level, int rank){
		List<Monster> available = new List<Monster> ();
		//Iterate through list adding all monsters fitting requirements to list
		//randomly select a monster
		//have monster adjust stats a bit
		//return monster
		Monster spawner = new Monster(Monsters[0]);
		spawner.SetUpMonster ();
		return spawner;
	}

	public Monster SpawnMonster(string name){
		Monster outMon = new Monster(Monsters [0]);
		outMon.SetUpMonster ();

		return outMon;
	}

	public List<Monster> GetSpawnable(int level){
		List<Monster> available = new List<Monster> ();
		foreach (Monster m in Monsters) {
			if (m.min <= level && level <= m.max) {
				available.Add (m);
			}
		}
		return available;
	}
}


