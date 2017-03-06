using System;


public abstract class Item
{
	public static string Item_Type_Heal = "HEAL";
	public static string Item_Type_Mana = "MANA";
	public static string Item_Type_Dual = "DUAL";
	public static string Item_Type_Buff = "BUFF";
	public static string Item_Type_Drop = "DROP";
	public static string Item_Type_Stone = "STONE";
	public static string Item_Type_Equip = "EQUIPMENT";

	protected string Type;
	protected float _value;
	protected string name;
	protected float _cost;

	public Item ()
	{
	}

	public abstract void use (Unit U);

	public abstract float value();

	public abstract bool usable ();

	public abstract float value(Unit U);

	public float cost {
		get { return _cost; }
	}

	public string _type{
		get{ return Type; }
	}

	public string _name{
		get{ return name; }
	}
}


