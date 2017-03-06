using System;


public class HealingItem : Item
{
	private int heal_type;
	private float heal_amount;
	private int mana_type;
	private float mana_amount;


	public HealingItem (string nm, int type, float amount, float price)
	{
		name = nm;
		Type = Item.Item_Type_Heal;
		heal_type = type;
		heal_amount = amount;
		mana_type = 0;
		mana_amount = 0f;
		_cost = price;
	}

	public HealingItem (string nm, int type, float amount, float price, string nothing)
	{
		name = nm;
		Type = Item.Item_Type_Mana;
		heal_type = 0;
		heal_amount = 0f;
		mana_type = type;
		mana_amount = amount;
		_cost = price;
	}

	public HealingItem (string nm, int hptype, float hpamount, int mptype, float mpamount, float price)
	{
		name = nm;
		Type = Item.Item_Type_Dual;
		heal_type = hptype;
		heal_amount = hpamount;
		mana_type = mptype;
		mana_amount = mpamount;
		_cost = price;
	}

	override public void use(Unit U){
		if (heal_type == 1) {
			U.hp = U.hp + (U.hp_max * heal_amount);
		}
		else if (heal_type == 2) {
			U.hp = U.hp + heal_amount;
		}
		if (mana_type == 1) {
			U.mp = U.mp + (U.mana_max * mana_amount);
		}
		else if (mana_type == 2) {
			U.mp = U.mp + mana_amount;
		}
	}

	override public float value(){
		return 0f;
	}

	override public float value(Unit U){
		float restore = 0f;
		if (heal_type == 1) {
			restore = restore + (U.hp_max * heal_amount);
		}
		else if (heal_type == 2) {
			restore = restore + heal_amount;
		}
		if (mana_type == 1) {
			restore = restore + (U.mana_max * mana_amount);
		}
		else if (mana_type == 2) {
			restore = restore + mana_amount;
		}
		return restore;
	}

	public override bool usable ()
	{
		return true;
	}
}


