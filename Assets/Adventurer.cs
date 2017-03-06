using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adventurer : Unit
{
	public static string Type_Adventurer = "Adventurer";
	private DungeonLog data_log;


	protected float experience;
	protected List<List<float>> hiddenStats;
	protected List<Item> useables = new List<Item> ();
	protected List<Skill> skills = new List<Skill>();
	protected List<Spell> spells = new List<Spell>();
	private Guild_Manager guild;
	protected float excelia;
	protected List<int> entered_floors; //a floor will be entered only when the hero first steps onto the floor
	public delegate float StatModification(float stat, string WhatStat);
	protected StatModification myModification;
	private CastingSpell Current_Cast;
	private Personality traits;
	private List<string> Magic_Affinity;

	public Adventurer ()
	{
		type = Type_Adventurer;
		_name = "Adventure Man";
		data_log = new DungeonLog (name);
		strength = defense = dextarity = agility = magic = 15;
		experience = 10f;
		hiddenStats = new List<List<float>> ();
		traits = new Personality ();
		myModification = NoMod;
		Defensive_Action = HeroCombatActions.HeroCombat.Defend;
		//Offensive_Action = HeroCombatActions.HeroCombat.Attack;
		spells.Add(SpellFactory.Spell_Factory.LearnSpell(level));
		Current_Cast = new CastingSpell (spells [0], mag);

		my_explorer = new DungeonAdventurer (this);
		for(int i = 0; i < 7; i++)
		{
			Attack_Action a = AttackFactory.Attack_Factory.LearnAction (0); //add the first attack
			a.Attached_to_whom(1);
			a.ActionMessage = "Test " + i;
			Known_Offense.Add (a);
		}
	}
		
	public Adventurer (string nm, Personality p, List<float> stats, Guild_Manager my_guild) //will have modification in the future, and affinity
	{
		type = Type_Adventurer;
		_name = nm;
		data_log = new DungeonLog (name);
		strength = stats[0];
		defense = stats[1];
		dextarity = stats[2];
		agility = stats[3];
		magic = stats[4];
		experience = stats[5];
		guild = my_guild;

		hiddenStats = new List<List<float>> ();
		traits = p;
		Magic_Affinity = p.Calc_Affinity ();
		myModification = NoMod;
		Defensive_Action = HeroCombatActions.HeroCombat.Defend;
		//Offensive_Action = HeroCombatActions.HeroCombat.Attack;
		//spells.Add(SpellFactory.Spell_Factory.LearnSpell(level));
		//Current_Cast = new CastingSpell (spells [0], mag);

		my_explorer = new DungeonAdventurer (this);

			Attack_Action a = AttackFactory.Attack_Factory.LearnAction (0); //add the first attack
			a.Attached_to_whom(1);
			Known_Offense.Add (a);


		entered_floors = new List<int> ();
	}

	/***********************************/
	//STAT MODIFICATION & LEVELING//		
	/***********************************/

	public float NoMod(float stat, string WhatStat){
		return stat;
	}

	public float statBuff()
	{
		float buff_amount =	Random.Range (experience / 4, experience);
		float sign = Random.Range (-1 * (experience / 4), experience);
		if (sign < 0) {
			buff_amount *= -1;
		}
		return buff_amount;
	}

	public void levelUp()
	{
		List<float> oldStats = new List<float> ();
		oldStats.Add (strength);
		oldStats.Add (defense);
		oldStats.Add (dextarity);
		oldStats.Add (agility);
		oldStats.Add (magic);
		hiddenStats.Add (oldStats);
		level++;
		strength = 10f;
		defense = 10f;
		dextarity = 10f;
		agility = 10f;
		if (magic > 0) {
			magic = 10f;
		}
	}

	public void UpdateStats(){
		DungeonAdventurer a = (DungeonAdventurer)my_explorer;
		List<float> statgains = a.getIncrease ();
		float skill = statgains [5];
		float excel = statgains [6];
		statgains.RemoveAt (6);
		statgains.RemoveAt (5);
		WriteStats (statgains, skill, excel);
	}

	private void WriteStats(List<float> statBoosts, float exp_, float exce_)
	{
		strength += myModification (statBoosts[0], "strength");
		defense += myModification (statBoosts[1], "defense");
		dextarity += myModification (statBoosts[2], "dextarity");
		agility += myModification (statBoosts[3], "agility");
		magic += myModification (statBoosts[4], "magic");
		experience += myModification (exp_, "skill");
		excelia += myModification (exce_, "excelia");
	}

	public void FullHeal(){
		health = Max_Health;
	}

	/***********************************/
		//Attack Determination//
	/***********************************/

	private void RandomAttack(){
		bool actionPicked = false;
		do {
			int selectAction = (int)Mathf.RoundToInt (Random.Range (0, Known_Offense.Count + spells.Count - 2));
			Attack_Action picked = null;
			if (selectAction > Known_Offense.Count) {
				selectAction -= Known_Offense.Count;
				Current_Cast = new CastingSpell (this.spells [selectAction], mag);
				Offensive_Action = Current_Cast;
				actionPicked = true;
			} else {
				picked = Known_Offense [selectAction];
				if (picked.useable) {
					Offensive_Action = picked;
					actionPicked = true;
				}
			}
		} while(!actionPicked);
	}

	public override void DetermineAttack(DungeonUnit target, List<DungeonUnit> allies, List<DungeonUnit> enemies, out DungeonUnit n_target){
		//CURRENTLY TESTING
		n_target = target;
		if (target == null) { //currently no target
			target = enemies[0];
			n_target = target;
		}
		PastAction LastAction = new PastAction();
		if (actions_offense.Count > 0) {
			LastAction = actions_offense [actions_offense.Count - 1];
		}
		//Check to see if casting or did an action that requires pausing
		if (LastAction.code == -3.5f) {
			Offensive_Action = Current_Cast;

		} 
		else if (delay != 0) {
			Offensive_Action = new Attack_Action ("none");
			delay--;
		}
		//if not. proceed normally
		else {
			bool attack = true;
			//Decide whether or not to heal based on how low the health is
			if (health < (Max_Health * (.25f + traits.AdjustPercent (traits._careful, .15f) -
			   (traits._affection < 0 ? traits.AdjustPercent (traits._affection, .05f) : 0f) +
				(traits._happyness < 0 ? traits.AdjustPercent (traits._happyness, .1f) : 0f)))) 
			{
				attack = false;
				List<Spell> heals = spells.FindAll(delegate(Spell a) {
					if(a._type == Spell.Spell_Type_Heal){
						return true;
					}
					else{ return false;}
				});
				//Sort healing spells from lowest cast time to longest
				heals.Sort (delegate(Spell x, Spell y) {
					return (int)(Mathf.RoundToInt (x._speed) - Mathf.RoundToInt (y._speed));
				});
				List<Item> heal_list;
				//Check to see if Mana is below a certain amount so that dual healing items could be useful
				if (mana < Max_Mana * .15f) {
					heal_list = useables.FindAll (delegate(Item obj) {
						return obj._type == Item.Item_Type_Heal || obj._type == Item.Item_Type_Dual;
					});
				}
				//If mana isn't low enough, just use healing items
				else {
					heal_list = useables.FindAll (delegate(Item obj) {
						return obj._type == Item.Item_Type_Heal;
					});
				}
				//Check to see if there are healing spells and item
				if (heals.Count == 0) {
					if (heal_list.Count == 0) {
						attack = true;
					} 
					//If there are no healing spells, but healing items. Pick item to use. Using the best first
					else {
						heal_list.Sort (delegate(Item x, Item y) {
							return (int)(Mathf.RoundToInt (y.value (this)) - Mathf.RoundToInt (x.value (this)));
						});
						Item use = heal_list [0];
						useables.Remove (use);
						use.use (this);
						Offensive_Action = new Attack_Action ("none");
						data_log.AddEntry (DungeonLog.ENTRY_TYPE_ITEM, name + " used " + use._name, name, "self");
					}
				}
				//If there are healing spells that can be used
				else {
					//if there are healing items. decide whether to use spells or items
					if (heal_list.Count > 0) {
						heal_list.Sort (delegate(Item x, Item y) {
							return (int)(Mathf.RoundToInt (y.value (this)) - Mathf.RoundToInt (x.value (this)));
						});
						float selection = Random.value;
						//Choice is to use a healing item
						if (selection > .25) {
							Item use = heal_list [0];
							useables.Remove (use);
							use.use (this);
							Offensive_Action = new Attack_Action ("none");
						} 
						//choice is to use a healing spell
						else {
							Current_Cast = new CastingSpell (heals [0], mag);
							Offensive_Action = Current_Cast;
						}
					} 
					//If no healing items. use a spell. aiming for lowest cast time first
					else {
						Current_Cast = new CastingSpell (heals [0], mag);
						Offensive_Action = Current_Cast;
					}
				}
			} 
			//decide whether or not to use a mana healing item
			else if (mana < Max_Mana * .15f) {
				attack = false;
				List<Item> heal_list;
				//Check to see if health is lower than 15%. Look for dual potions
				if (mana < Max_Health * .15f) {
					heal_list = useables.FindAll (delegate(Item obj) {
						return obj._type == Item.Item_Type_Mana || obj._type == Item.Item_Type_Dual;
					});
				} 
				//if health is higher, use a regular mana potion
				else {
					heal_list = useables.FindAll (delegate(Item obj) {
						return obj._type == Item.Item_Type_Mana;
					});
				}
				//check to make sure there are items. if Not, just attack
				if (heal_list.Count == 0){
					attack = true;
				}
				//use a healing item. using the best one first
				else {
					heal_list.Sort (delegate(Item x, Item y) {
						return (int)(Mathf.RoundToInt (y.value (this)) - Mathf.RoundToInt (x.value (this)));
					});
					Item use = heal_list [0];
					useables.Remove (use);
					use.use (this);
					Offensive_Action = new Attack_Action ("none");
					data_log.AddEntry (DungeonLog.ENTRY_TYPE_ITEM, name + " used " + use._name, name, "self");
				}
			} 
			//If the attack boolean is still true, Attack the target
			if (attack){
				//Check to see if any enemies attacking allies
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
						//Building the buff lists
						#region
						List<Item> buff_list;
						//Check to see if health is lower than 15%. Look for dual potions
						buff_list = useables.FindAll (delegate(Item obj) {
							return obj._type == Item.Item_Type_Buff;
						});
						List<Spell> buff_spells;
						buff_spells = spells.FindAll (delegate(Spell obj) {
							return obj._type == Spell.Spell_Type_Buff;
						});
						List<object> buffOptions = new List<object>();
						foreach (Item i in buff_list){
							buffOptions.Add((object)i);
						}
						foreach (Spell i in buff_spells){
							buffOptions.Add((object)i);
						}
						#endregion
						//If nothing hasn't been use. Randomly atttack or buff. //TODO Add switching weapons
						if (unused.Count == 0)
						{
							//Chance to use a buff or randomly attack or change weapon
							float decide = Random.value;
							if (decide > .4f || buffOptions.Count == 0) {
								RandomAttack ();
							} 
							else {
								int choice =(int)Mathf.RoundToInt( Random.Range (0, buffOptions.Count - 1));
								if (choice > buff_list.Count - 1) {
									//use an item to buff
									Item select_item = buff_list[choice];
									select_item.use (this);
									useables.Remove (select_item);
									Offensive_Action = new Attack_Action ("none");
								} else {
									//Use a spelll to buff
									choice = choice - (buff_list.Count - 1);
									Spell select_spell = buff_spells [choice];
									Current_Cast = new CastingSpell (select_spell, mag);
									Offensive_Action = Current_Cast;
								}
							}
						} 
						//Decide whether to retry an old action, butt or Attack. //TODO Add switching weapons
						else {
							float new_action = .5f - traits.AdjustPercent (traits._creativity, .15f) - traits.AdjustPercent (traits._careful, .15f) + (.02f * unused.Count);
							new_action += .15f; //adds in a space for buff actions
							float retry_action = 1f - new_action;
							new_action -= .15f; //reverts new_action to its normal ammount so it doesn't fill the buff action void
					
							float choice = Random.value;
							if (choice < new_action) {
								//New action!
								int action_choice = (int)Mathf.RoundToInt (Random.Range (0, unused.Count - 1));
								Offensive_Action = unused [action_choice];
							} else if (choice > retry_action) {
								//retry an action
								int action_choice = (int)Mathf.RoundToInt (Random.Range (0, actions.Count - 1));
								Offensive_Action = actions [action_choice].action;
							} else {
								//do a buff
								int select =(int)Mathf.RoundToInt( Random.Range (0, buffOptions.Count - 1));
								if (select > buff_list.Count - 1) {
									//use an item to buff
									Item select_item = buff_list[select];
									select_item.use (this);
									useables.Remove (select_item);
									Offensive_Action = new Attack_Action ("none");
								} else {
									//Use a spelll to buff
									select = select - (buff_list.Count - 1);
									Spell select_spell = buff_spells [select];
									Current_Cast = new CastingSpell (select_spell, mag);
									Offensive_Action = Current_Cast;
								}
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
						float new_action = .5f + traits.AdjustPercent (traits._creativity, .15f) - traits.AdjustPercent (traits._careful, .15f) - (.02f * successes.Count);
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
							if (traits._wisdom < -25) {
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
	}

	/***********************************/
		//Public Functions//
	/**********************************/

	public override void TakeDamage (float damage, List<string> Elements)
	{
		if (Current_Cast != null) {
			Current_Cast.damageFocus = damage;
		}
		base.TakeDamage (damage, Elements);
	}

	public void AddEnteredFloor(int floor_number){
		if (!entered_floors.Contains (floor_number)) {
			entered_floors.Add (floor_number);
		}
	}

	public bool ClearedFloor(int floor){
		return entered_floors.Contains (floor + 1); //
	}

	/***********************************/
	//ITEM MANAGEMENT//		
	/***********************************/

	public virtual void AddItem(Item i){
		useables.Add (i);
	}

	/***********************************/
		//GETTERS AND SETTERS//		
	/***********************************/

	public void FloorExploringInformation (out List<int> entered){
		entered = entered_floors;
	}

	override public float str
	{
		get {float outVal = strength;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [0];
			}
			return ApplyStatus( outVal, 0);}
	}

	public float str_base
	{
		get {float outVal = strength;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [0];
			}
			return outVal;}
	}

	public float str_shown{
		get{ return (float)Mathf.FloorToInt( strength); }
	}

	override public float def
	{
		get {float outVal = defense;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [1];
			}
			return ApplyStatus( outVal, 1);}
	}

	public float def_base
	{
		get {float outVal = defense;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [1];
			}
			return outVal;}
	}

	override public float def_attack
	{
		get {float outVal = defense;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [1];
			}
			return ApplyStatus( outVal, 1) + statBuff();}
	}

	public float def_shown{
		get{ return (float)Mathf.FloorToInt( defense); }
	}

	override public float dex
	{
		get {float outVal = dextarity;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [2];
			}
			return ApplyStatus( outVal, 2);}
		set { dextarity = value; }
	}

	public float dex_base
	{
		get {float outVal = dextarity;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [2];
			}
			return outVal;}
	}

	public float dex_shown {
		get{ return (float)Mathf.FloorToInt( dextarity); }
	}

	override public float agi
	{
		get {float outVal = agility;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [3];
			}
			return ApplyStatus( outVal, 3);}
	}

	public float agi_base
	{
		get {float outVal = agility;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [3];
			}
			return outVal;}
	}

	public float agi_shown{
		get{ return (float)Mathf.FloorToInt( agility); }
	}

	override public float mag
	{
		get {float outVal = magic;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [4];
			}
			return ApplyStatus( outVal, 4);}
	}

	public float mag_base
	{
		get {float outVal = magic;
			for (int i = 0; i < hiddenStats.Count; i++) {
				outVal += hiddenStats [i] [4];
			}
			return ApplyStatus( outVal, 4);}
	}

	public float mag_shown {
		get{ return (float)Mathf.FloorToInt( magic); }
	}

	public override float hp
	{
		get {return health;}
	}

	public float exp
	{
		get{ return experience; }
	}

	public DungeonLog diary{
		get{ return data_log; }
	}

	public float _mana {
		get{ return this.mana; }
		set{ mana = value; }
	}

	public Personality personality{
		get{ return traits; }
	}

	public Guild_Manager _guild{
		get{ return guild; }
	}
}


