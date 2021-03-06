/* Combat Action explanation
OUTPUT Float -> The amount of damage / block / dodge skill involved for the hero. Used for stat buffing.
	If the value would be negative, it should be set to 0 and stat (see below) should be set to 0.
	if ACTION is set false this will return the hit of the action the target is taking. 
hero is normally the adventurer involved in this action. Kept in this position for consistancy
	This can be either a monster or an adventurer
TheMon is normally the monster involved in this action. Kept in this position for consistancy
	This can be either a monster or an adventurer
focus is mainly for defense and is used to indication if the one attacking them is their main focus or not
	For attacks that are active focus doesn't matter. For inactive attacks it tells whether you want the hit or the power
	False is hit, True is power
active is used for information gathering on attackers to learn about what the HIT on their action is.
	Calling on defensive actions can be done but will return the percentage of damage that should be taken due to the action
stat is sent out to indicate what stat should be buffed for the hero by the action. (0 = none)(1 = strength)(2 = defense)(3 = dextarity)(4 = agility)(5 = magic)
isResponse is used to indicate if there is a response to the action;
response is used to send any action that the monser or hero might take in reaction to the action that just happened. Such as counters or bonus attacks
defended is used to indicate if the action (OFFENSIVE) was defended against (0 = Not defended)(1 = blocked)(2 = dodged)
defending is used to indicate if the target defended when taking DEFENSIVE action. (0 = Not defended)(1 = blocked)(2 = dodged)

You_are is used to indicate which position the one resolving the action is in. 1 being the hero position and 2 being the monster position
self is used to indicate if the combataction is being called from outside the actiontaker. 
	If it is true the CombatAction may return Codes in the form of NEGATIVE floats Codes are listed below
	//A CombatAction may return a code depending on the action performed. Codes returned regularly are marked in angle brackets <#> 
	-1 = action was an attack
	-2 = action was a counter (COUNTER overrides last attack action.)(Counter is not allowed to happen when casting)
	-3 = action was a spell
	-3.5 = action is casting spell. (This is used ot indicate that the spell has not been completed yet)
	-4 = action was nothing
	-5 = action requires a turn of nothing (ACTIONS CANNOT REQUIRE NOTHING BEFORE THEM. Consider a spell or skill if desired)
	-6 = action was a skill
	-7 = action was using an item
	-8 = action was a dodge
	-9 = action was a block
	-10 = action was deciding defense. If you are getting this something went wrong

	< -56 > = action is an AOE attack and should be handled as such
	< -78 > = action is to call for more monsters of same type and should be handled as such.  
*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterCombatActions
{

	public static MonsterCombatActions MonsterCombat = new MonsterCombatActions();

	private MonsterCombatActions (){/*Nothing happens here. This is a singleton*/}


	/***********************************/
	//COMBAT ACTIONS//		
	/***********************************/
	//Default Outline for a CombatAction. Copy and Paste for each new method to guarantee accuraccy. 
	/*public float METHOD NAME HERE(Unit Position_One, Unit Position_Two, bool focus, bool active, out int stat,
		out bool isResponse, out CombatAction response, int defended, out int defending, bool self, int You_are){
		Unit hero = Position_One;
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One; hero = Position_Two;}
		if (You_are == 2) {	TheMon = (Monster)Position_Two; hero = Position_One;}
		//Temporary out settings
		stat = 0;
		isResponse = false;
		response = NoAction;
		defending = 0;

		if (!active && self) {
			return YOUR_TYPE_CODE_HERE;
		} else if (!active) {
			return YOUR_HIT_CHANCE_HERE;
		}

		//YOUR CODE BELOW HERE

		return 0f;
	}*/

	//No actions Methods

	public float NoActionOffense(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One;}
		if (You_are == 2) {	TheMon = (Monster)Position_Two;}
		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;

		if (!active && self) {
			return -4f;
		} else if (!active) {
			return 0f;
		}

		TheMon.LastOffense = new PastAction(new Attack_Action(), -4f, true, 0f, 0,TheMon);

		return 0f;
	}

	public float NoActionDefense(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One;}
		if (You_are == 2) {	TheMon = (Monster)Position_Two;}
		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;

		if (!active && self) {
			return -4f;
		} else if (!active) {
			return 0f;
		}

		TheMon.LastDefense= new PastAction(NoActionDefense, -4f, true, 0f, 0, TheMon);

		return 0;
	}

	public float NoAction(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){

		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;

		if (!active && self) {
			return -4f;
		} else if (!active) {
			return 0f;
		}

		return 0;
	}

	//Defensive Functions

	private float hitChance; //This is used to prevent an attacks accuracy from changing from the first produced value
	private float hitPower; //This is used to reduce method calls during combat

	public float Block(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Unit hero = Position_One;
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One; hero = Position_Two;}

		//Temporary out settings
		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;
		float retValue = 0f;
		if (!active && self) {
			return -9;
		} else if (!active) {
			return 1f;
		}

		//Blocking Happens

		float blockPower = TheMon.dex + (TheMon.str / 2);
		if (!focus) {
			blockPower *= .5f;
		}

		//blockPower += hero.skillBuff (Skill.type_block); //looks to see if the hero has any evasion skills and adds their buffs to the stat
		blockPower = Mathf.RoundToInt(blockPower);
		float StoppingPower = blockPower - hitPower;
		if (StoppingPower > 150) {
			FullBlock (hero, TheMon, focus, active, out stat, out isResponse, out response, defended, out defending, self, 2);
			retValue = StoppingPower;
		} else if (StoppingPower > 0) {
			HalfBlock (hero, TheMon, focus, active, out stat, out isResponse, out response, defended, out defending, self, 2);
			retValue = StoppingPower;
			//The following is set once again to ensure it is properly set
		}else {
			retValue = 0;
			//The following is set once again to ensure it is properly set
			stat = new int[0];
			defending = 0;
			TheMon.LastDefense = new PastAction(Block, -9, false, 1f, 0, hero);
		}
			
		//DEFAULT VALUES UNTIL SKILLS IMPLEMENTED
		isResponse = false;
		response =  new Attack_Action ();

		return retValue;
	}

	//This function shouldn't be called except from Blocking. Used to indicate that the user blocked the attack fully and takes no damage
	public float FullBlock(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Monster TheMon = (Monster)Position_Two;
		Unit hero = Position_One;
		if (You_are == 1) {	TheMon = (Monster)Position_One; hero = Position_Two;}
		//Temporary out settings
		stat = new int[1]{3};
		isResponse = false;
		response =  new Attack_Action ();
		defending = 1;

		if (!active && self) {
			return -9f;
		} else if (!active) {
			return 0;
		}

		TheMon.LastDefense = new PastAction(FullBlock, -9, defending > 0, defending > 0 ? 0 : 1f, 0, hero);

		return 0f;
	}

	public float HalfBlock(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Monster TheMon = (Monster)Position_Two;
		Unit hero = Position_One;
		if (You_are == 1) {	TheMon = (Monster)Position_One; hero = Position_Two;}
		//Temporary out settings
		stat = new int[1]{3};
		isResponse = false;
		response =  new Attack_Action ();
		defending = 1;

		if (!active && self) {
			return -9f;
		} else if (!active) {
			return .5f;
		}

		TheMon.LastDefense = new PastAction(HalfBlock, -9, defending > 0, defending > 0 ? .5f : 1f, 0, hero);

		return 0f;
	}

	public float Dodge(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Monster TheMon = (Monster)Position_Two;
		Unit hero = Position_One;
		if (You_are == 1) {	TheMon = (Monster)Position_One; hero = Position_Two;}
		//temporary out settings
		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;
		float retValue = 0f;

		//The following rules out if this is an active action or not
		if (!active && self) {
			return -8;
		} else if (!active) {
			return 0;
		}

		//Act of dodging

		float dodgeSkill =	TheMon.agi;
		if (!focus) {
			dodgeSkill *= .5f;
		}
		dodgeSkill = Mathf.RoundToInt(dodgeSkill);
		float selection = Random.Range (0, dodgeSkill + hitChance);
		if (selection > hitChance) {
			defending = 2; //set defending to the dodge code
			stat = new int[1]{4}; //code set to agility.
		} else {
			//The following is set once again to ensure it is properly set
			defending = 0; //set defending as the none code
			stat = new int[0]; //code set to none
		}
		retValue = dodgeSkill - hitChance;
		if (retValue < 0) {
			retValue = 0;
		}
		//hero.FindSkill (Skill.type_counter,out isResponse, out response, defending); //searches for any counter skill to use
		//DEFAULT VALUES UNTIL SKILLS IMPLEMENTED
		isResponse = false;
		response =  new Attack_Action ();

		TheMon.LastDefense = new PastAction(Dodge, -8, defending > 0, defending > 0 ? 0f : 1f, 0, hero);
		//TheMon.LastDefense = Dodge;

		return retValue;
	}

	public float Defend(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Unit hero = Position_One;
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One; hero = Position_Two;}
		if (You_are == 2) {	TheMon = (Monster)Position_Two; hero = Position_One;}
		//temporary out settings
		stat = new int[0];
		isResponse = false;
		response =  new Attack_Action ();
		defending = 0;

		if (!active && self) {
			return -10;
		} else if (!active) {
			return 0;
		}

		//Gather Information to make decision
		float lastAct = TheMon.LastOffense.code; /*(hero, TheMon, focus, false, out stat, out isResponse, out response, 0, out defending, true, 2);*/
		float blockPower = TheMon.dex + (TheMon.str / 2);
		float dodgeSkill = TheMon.agi;
		if (!focus) {
			blockPower *= .5f;
			dodgeSkill *= .5f;
		}
		hitChance = hero.Offensive_Action.hit (hero);/* (hero, TheMon, focus, false, out stat, 
			out isResponse, out response, 0, out defending, false, 1); */
		hitPower = hero.Offensive_Action.damage (hero);/* (hero, TheMon, true, false, out stat, 
			out isResponse, out response, 0, out defending, false, 1); */


		float outValue = 0f;

		//The choice is made
		//somehow, maybe personality, hero may decide to do nothing
		if(lastAct == -3.5f || Random.value < .5){ //Hero's personality may/will affect this later
			//Dodge
			outValue = Dodge(hero, TheMon, focus, active, out stat, out isResponse, out response, defended, out defending, self, You_are);
		}
		else{
			//Block
			outValue = Block(hero, TheMon, focus, active, out stat, out isResponse, out response, defended, out defending, self, You_are);
		}

		return outValue;
	}

	//Offensive Functions

	public float Attack(Unit Position_One, Unit Position_Two, bool focus, bool active, out int[] stat,
		out bool isResponse, out Attack_Action response, int defended, out int defending, bool self, int You_are){
		Unit hero = Position_One;
		Monster TheMon = (Monster)Position_Two;
		if (You_are == 1) { TheMon = (Monster)Position_One; hero = Position_Two;}
		if (You_are == 2) {	TheMon = (Monster)Position_Two; hero = Position_One;}
		//Temporary out settings
		stat = new int[1]{2};
		isResponse = false;
		response =  new Attack_Action ();
		defending = defended;

		//Stat buff is used here because the buff needs to be present when enemy is figuring out if they can dodge or not
		//It is not relevant to whether or not the attack will hit. When this is active, the attack was sucessful. 

		float hit = TheMon.Offensive_Action.hit (TheMon);
		float power = TheMon.Offensive_Action.damage (TheMon);

		if (!active && self) {
			return -1;
		} else if (!active && !focus) {
			return hit;
		} else if (!active && focus) {
			return power;
		}
			
		float damage = Mathf.RoundToInt(power) - Mathf.RoundToInt(hero.def_attack);
		if (defending == 1) {
			damage *= hero.LastDefense._damage;/* (hero, TheMon, focus, false, out stat, out isResponse,out response, defended,out defending, false, 1); */
		}
		//Default Value for current testign
		isResponse = false;
		response =  new Attack_Action ();

		if (damage <= 0) {
			damage = 0;
		} else {
			hero.TakeDamage (damage, TheMon.Offensive_Action.elements);
		}

		TheMon.LastOffense = new PastAction(TheMon.Offensive_Action, TheMon.Offensive_Action.code, damage > 0, damage, TheMon.Offensive_Action.volume, hero);

		return damage;
	}
}


