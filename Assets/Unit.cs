using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate float CombatAction(Unit hero, Unit TheMon, bool focus, bool active, out int[] stat,
	out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are); 
/* Combat Action explanation is in MonsterCombatActions and HeroCombatActions*/

public class Unit/* : MonoBehaviour*/ {
	
	protected DungeonUnit my_explorer;

	protected string _name;
	protected string type;
	protected int level;
	protected float strength;
	protected float defense;
	protected float dextarity;
	protected float agility;
	protected float magic;
	protected float mana;
	protected float health;
	protected float Max_Health;
	protected float Max_Mana;
	public int delay = 0;
	public CombatAction Defensive_Action;
	public Attack_Action Offensive_Action;
	protected PastAction Last_Defense;
	protected PastAction Last_Offense;
	protected List<PastAction> actions_offense;
	protected List<PastAction> actions_defense;
	protected List<Attack_Action> Known_Offense;
	protected List<Status> buffs;

	protected List<string> Weak_Element; //Elements are located in the Spell
	protected List<string> Strong_Element; //Elements are located in the Spell
	//protected List<Attack_Action> Known_Defense;



	public Unit(){
		//DEFAULT VALUES
		type = "none";
		_name = "TESTER";
		level = 1;
		strength = defense = dextarity = agility = magic = 10;
		health = 100;
		Max_Health = health;
		mana = 50;
		Max_Mana = mana;
		//DEFAULT VALUES
		new DungeonUnit(this);
		buffs = new List<Status> ();
		Last_Defense = new PastAction();
		Last_Offense = new PastAction();
		actions_offense = new List<PastAction> ();
		actions_defense = new List<PastAction> ();
		Weak_Element = new List<string> (){Spell.Affinity_None};
		Strong_Element = new List<string> (){Spell.Affinity_None};
		Known_Offense = new List<Attack_Action> ();
	}

	public float GetStatTotal(){
		float total = Mathf.RoundToInt (str);
		total += Mathf.RoundToInt (def);
		total += Mathf.RoundToInt (dex);
		total += Mathf.RoundToInt (agi);
		total += Mathf.RoundToInt (mag);
		return total;
	}

	public void attachAction(Attack_Action action, float code, bool worked, float damage, Unit target)
	{
		int type = HeroCombatActions.HeroCombat.ActionType (code);
		PastAction n_action = new PastAction(action, code, worked, damage, action.volume, target);
		if (type == 1) { //Offense
			LastOffense = n_action;
			actions_offense.Add(n_action);
		} else if (type == 2) { //Defense
			LastDefense = n_action;
			actions_defense.Add(n_action);
		}
	}

	public void attachAction(PastAction past){
		if (past.action != null) {
			actions_offense.Add (past);
		} else {
			actions_defense.Add (past);
		}
	}

	public void addStatus(Status n){
		buffs.Add (new Status(n));
	}

	public void RemoveEndedStatus()
	{
		foreach (Status it_stat in buffs) {
			it_stat.decrament ();
			if (it_stat.isFinished ()) {
				buffs.Remove (it_stat);
			}
		}
	}

	public void StatusAilment(){
		foreach (Status it_stat in buffs) {
			health = it_stat.changeHP (health, Max_Health);
		}
	}

	public void SetDungeonUnit (DungeonUnit d)
	{
		my_explorer = d;
	}

	protected float ApplyStatus(float input, int stat){
		float boost = 1f;
		foreach (Status it_stat in buffs){
			float this_boost = it_stat.statChanges [stat];
			this_boost -= 1f;

			boost += this_boost;
		}
		return input * boost;//TODO have all status gets and sets call this method
	}

	public virtual void DetermineAttack(DungeonUnit target, List<DungeonUnit> enemies, List<DungeonUnit> allies, out DungeonUnit n_target){
		//THIS DOES NOTHING! IT IS HERE JUST TO BE OVERRIDDEN
		n_target = target;
	}

	public virtual void Encounter (DungeonUnit du){
		//nothing happens with a generic unit //TODO Implement proper response in Adventurer and Monster
	}

	public virtual void HearSound (DungeonReactable sound_source){
		//nothing happens with a generic unit //TODO Implement proper response in Adventurer and Monster
	}

	public virtual void TakeDamage (float damage, List<string> Elements)
	{
		float multiplier = 1f;
		foreach (string elem in Elements) {
			if (Weak_Element.Contains (elem) && !elem.Equals (Spell.Affinity_None)) {
				multiplier += .5f;
			}
		}
		foreach (string elem in Elements) {
			if (Strong_Element.Contains (elem) && !elem.Equals (Spell.Affinity_None)) {
				if (multiplier > 1) {
					multiplier -= .5f;
				} else {
					multiplier *= .7f;
				}
			}
		}
		health -= damage;
	}

	public string name
	{
		get { return _name; }
	}

	public virtual float dex
	{
		get {return dextarity;}
		set { dextarity = value; }
	}

	public virtual float str
	{
		get {return strength;}
	}

	public virtual float def
	{
		get {return defense;}
	}

	public virtual float def_attack
	{
		get {return defense;}
	}

	public virtual float agi
	{
		get {return agility;}
	}

	public virtual float mag
	{
		get {return magic;}
	}

	public virtual float hp
	{
		get {return health;}
		set{ health = value; }
	}

	public virtual float hp_max
	{
		get{ return Max_Health; }
	}

	public virtual float mp
	{
		get{ return mana; }
		set{ mana = value; }
	}

	public virtual float mana_max
	{
		get{ return Max_Mana; }
	}

	public virtual int _level{
		get{ return level; }
	}

	public string _type
	{
		get{ return type; }
	}

	public PastAction LastOffense{
		get { return Last_Offense; }
		set { Last_Offense = value; }
	}

	public PastAction LastDefense{
		get { return Last_Defense; }
		set { Last_Defense = value; }
	}

	public List<Attack_Action> offenses{
		get { return Known_Offense; }
	}

	public DungeonUnit explorer{
		get { return my_explorer; }
	}
	/*public List<Attack_Action> defenses{
		get { return Known_Defense; }
	}*/
}
