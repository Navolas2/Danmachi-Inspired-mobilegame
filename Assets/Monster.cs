using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Unit
{
	public static string type_Monster = "Monster";

	private int minRange;
	private int maxRange;
	private bool wise;
	private int legs;
	private bool breath;
	private bool group;
	private bool aggressive;
	private float rate;
	private List<Item> drops;
	private List<string> drop_names;

	public Monster(){
		minRange = 1;
		maxRange = 1;
		//everything else is default
	}

	public Monster (int min, int max, string name_, float str_, float def_, float dex_, float agi_, float res_, float health_, int rank_, bool _wise, bool _group, bool _aggressive, bool _breath, int _legs,List<string> affinities,List<string> weakness, List<string> _drops)
	{
		type = type_Monster;
		minRange = min;
		maxRange = max;
		_name = name_;
		strength = str_;
		defense = def_;
		dextarity = dex_;
		agility = agi_;
		magic = res_;
		health = health_;
		level = rank_;

		wise = _wise;
		group = _group;
		aggressive = _aggressive;
		breath = _breath;
		legs = _legs;
		drop_names = _drops;

		Weak_Element = weakness;
		Strong_Element = affinities;

		Defensive_Action = MonsterCombatActions.MonsterCombat.Defend;
		Offensive_Action = new Attack_Action ();

		explorer.SetFloorTarget(Random.Range(minRange, maxRange + 1));
	}

	public Monster (Monster orig){
		type = type_Monster;
		minRange = orig.min;
		maxRange = orig.max;
		_name = orig.name;
		strength = orig.str;
		defense = orig.def;
		dextarity = orig.dex;
		agility = orig.agi;
		magic = orig.mag;
		health = orig.health;
		level = orig.level;
		Defensive_Action = orig.Defensive_Action;
		Offensive_Action = orig.Offensive_Action;

		explorer.SetFloorTarget(Random.Range(minRange, maxRange + 1));
	}

	private void adjustStats(){
		strength += Random.Range (-7, 7);
		defense += Random.Range (-7, 7);
		dextarity += Random.Range (-7, 7);
		agility += Random.Range (-7, 7);
		magic += Random.Range (-7, 7);
		health += Random.Range (-15, 15);
	}

	public void SetUpMonster(){
		SetActions ();
		adjustStats ();
		SetDropItems ();
	}

	private void SetActions(){
		Attack_Action a = AttackFactory.Attack_Factory.LearnAction (level);
		a.Attached_to_whom (2);
		Known_Offense.Add (a);
	}

	private void SetDropItems(){
		drops = new List<Item>();
		foreach(string i in drop_names){
			drops.Add(Item_Factory.The_Item_Factory.GetItem(i, Item.Item_Type_Drop));
		}
	}

	/***********************************/
	//Attack Determination//
	/***********************************/

	private void RandomAttack(){
		bool actionPicked = false;
		do {
			int selectAction = (int)Mathf.RoundToInt (Random.Range (0, Known_Offense.Count-1));
			Attack_Action picked = null;
			picked = Known_Offense [selectAction];
			if (picked.useable) {
				Offensive_Action = picked;
				actionPicked = true;
			}
		} while(!actionPicked);
	}

	public override void DetermineAttack(DungeonUnit target, List<DungeonUnit> allies, List<DungeonUnit> enemies, out DungeonUnit n_target){
		//CURRENTLY TESTING
		PastAction LastAction = new PastAction();
		n_target = target;
		if (target == null) { //currently no target
			target = enemies[0];
			n_target = target;
		}
		if (actions_offense.Count > 0) {
			LastAction = actions_offense [actions_offense.Count - 1];
		}
		//Check to see if casting or did an action that requires pausing 
		else if (delay != 0) {
			Offensive_Action = new Attack_Action ("none");
			delay--;
		}
		//if not. proceed normally
		else {
			//Check to see if any enemies attacking allies
			enemies.Sort (delegate(DungeonUnit x, DungeonUnit y) {
				return (int) Mathf.RoundToInt(x.attached_unit.hp - y.attached_unit.hp);
			});
			n_target = enemies [0];
			target = n_target;

			//depending on personality change to help ally
			//randomly change enemy if unfocused
			//ROLE CHECK! WILL BE ADDED LATER IN BATTLE PARTIES UPDATE. 
			List<PastAction> against_target = actions_offense.FindAll (delegate (PastAction a) {
				return a.target.Equals (target.attached_unit.name);
			});
			List<PastAction> actions = new List<PastAction> ();
			List<float> avg_damage = new List<float> ();
			List<int> counts = new List<int> ();
			//Check to see if any actions have been used against the enemy
			if (against_target.Count == 0) {
				//RNG from all actions
				RandomAttack();
			} 
			//If there has been actions tried against the target
			else {
				//build the actions, avg_damage and counts lists so that there are no repeats.
				for (int i = 0; i < against_target.Count; i++) {
					if (actions.Exists(delegate(PastAction obj) {
						return obj.action == against_target[i].action;
					})) {
						int index = actions.FindIndex (delegate(PastAction obj) {
							return obj.action == against_target [i].action;
						});
						avg_damage [index] += against_target [i]._damage;
						counts [index]++;
					} 
					else {
						actions.Add (against_target [i]);
						avg_damage.Add( against_target [i]._damage);
						counts.Add(1);
					}
				}
				for (int i = 0; i < avg_damage.Count; i++) {
					avg_damage [i] = avg_damage [i] / counts [i];
				}
					
				//select all actions that worked against the target
				List<PastAction> successes = actions.FindAll (delegate (PastAction a) {
					return a.worked;
				});
				List<Attack_Action> unused = new List<Attack_Action> ();
				//build list of actions that have not been used against the target
				foreach(Attack_Action a_act in Known_Offense){
					if (a_act.useable && !actions.Exists(delegate(PastAction obj) {
						return obj.action == a_act;	})) 
					{
						unused.Add (a_act);
					}
				}
				//If nothing is successful
				if (successes.Count == 0) {
					//If nothing hasn't been use. Randomly atttack or buff. //TODO Add switching weapons
					if (unused.Count == 0)
					{
						RandomAttack ();
					} 
					//Decide whether to retry an old action, butt or Attack. //TODO Add switching weapons
					else {
						float new_action = .5f;

						float choice = Random.value;
						if (choice < new_action) {
							//New action!
							int action_choice = (int)Mathf.RoundToInt (Random.Range (0, unused.Count - 1));
							Offensive_Action = unused [action_choice];
						} else{
							//retry an action

							int action_choice = (int)Mathf.RoundToInt (Random.Range (0, actions.Count - 1));
							Offensive_Action = actions [action_choice].action;
						} 
					}
				} 
				//If there were successful actions
				else {
					//Remove all unsuccessful actions from actions, avg_damage and counts
					for (int i = 0; i < actions.Count; i++){
						if(!actions[i].worked){
							actions.RemoveAt (i);
							avg_damage.RemoveAt (i);
							counts.RemoveAt (i);
							i--;
						}
					}

					//Build chances for deciding whether or not to do a new actions or not
					float new_action = .5f;
					//if nothing is unused, value is set so will ALWAYS retry
					if (unused.Count == 0) {
						new_action = -1f;
					}
					float choice = Random.value;
					//If new action
					if (choice < new_action) {
						//New action!
						int action_choice = (int)Mathf.RoundToInt(Random.Range(0, unused.Count - 1));
						Offensive_Action = unused [action_choice];
					} 
					//If retry an action
					else {
						//If unwise, just randomly select something that worked
						if (wise) {
							int action_choice = (int)Mathf.RoundToInt (Random.Range (0, actions.Count - 1));
							Offensive_Action = actions [action_choice].action;
						} 
						//If Wise. Select something that deals the most damage
						else {
							float range = 0f;
							foreach (float n in avg_damage) {
								range += n;
							}
							float selection = Random.Range(0, range);
							int i = -1;
							while(selection > 0){
								i++;
								selection -= avg_damage[i];
							}
							Offensive_Action = actions[i].action;
						}
					}
				}
			}
		
		}
	}

	public int min
	{
		get{ return minRange; }
	}

	public int max
	{
		get{ return maxRange; }
	}

	public bool _group{
		get{ return group; }
	}

	public bool _aggressive{
		get{ return aggressive; }
	}

	public float _spawn_rate{
		get{ /*return rate;*/ return 1; }
		set{ rate = value; }
	}
}


