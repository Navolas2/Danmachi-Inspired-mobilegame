using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Connection
{
	int index;
	string label;
	coordinate location;
	coordinate grid_location;
	int orientation;
	int original_orientation;
	Connection pair;
	Room connected_room;

	public Connection (coordinate coor, int direction, string tag, Room r)
	{
		location = coor;
		orientation = direction;
		original_orientation = orientation;
		connected_room = r;
		label = tag;
		SetGridLocation ();
	}

	public Connection (Connection c, Room r){
		this.location = new coordinate(c.location);
		this.orientation = c.orientation;
		original_orientation = orientation;
		this.label = c.label;
		this.grid_location = new coordinate(c.grid_location);
		this.connected_room = r;
	}

	private void SetGridLocation(){
		switch (orientation) {
		case 1:
			grid_location = new coordinate (location._x_int, location._z_int - 1, location._layer, location._floor);
			break;
		case 2:
			grid_location = new coordinate (location._x_int - 1, location._z_int, location._layer, location._floor);
			break;
		case 3:
			grid_location = new coordinate (location._x_int, location._z_int + 1, location._layer, location._floor);
			break;
		case 4:
			grid_location = new coordinate (location._x_int + 1, location._z_int, location._layer, location._floor);
			break;
		}

	}

	public void ConvertToEntrance(){
		this.orientation = 7;
	}

	public void ConvertToExit(){
		this.orientation = 8;
	}

	public void ConvertToStairs(int dir){
		this.orientation = dir;
	}

	public bool is_open(){
		if (orientation != 7 && orientation != 8) {
			return (pair == null);
		}
		else {
			return false;
		}
	}
		
	public static bool CompleteConnection(Connection one, Connection two){
		one.pair = two;
		two.pair = one;
		return true;
	}



	public void AdjustPosition(int layer, int floor, int index){
		this.index = index;
		location.AdjustPosition (layer, floor, index);
		grid_location.AdjustPosition (layer, floor, index);
	}

	public static Connection FindMatchingConnection(List<Connection> conns, Connection matcher){
		foreach (Connection c in conns) {
			if (CompatibleDirection (c._Direction, matcher._Direction)) {
				if (!c._room.Equal(matcher._room)) {
					return c;
				}
			}
		}
		return null;
	}

	public static List<Connection> FindMatchingConnection(List<Connection> conns_one, List<Connection> conns_two){
		foreach (Connection c in conns_one) {
			Connection res = FindMatchingConnection (conns_two, c);
			if (res != null) {
				return new List<Connection>(){c, res};
			}
		}
		return null;

	}

	public bool AtConnection(coordinate loc){
		return location.IsLocation (loc);
		/*switch (orientation) {
		case 1:
			return loc.IsLocation(new coordinate(location._x_int + 1, location._y_int, location._z_int, location._floor));
		case 2:
			return loc.IsLocation(new coordinate(location._x_int, location._y_int, location._z_int - 1, location._floor));
		case 3:
			return loc.IsLocation(new coordinate(location._x_int - 1, location._y_int, location._z_int, location._floor));
		case 4:
			return loc.IsLocation(new coordinate(location._x_int, location._y_int, location._z_int + 1, location._floor));
		}
		*/
		//return false;
	}

	private static bool CompatibleDirection(int dir_1, int dir_2){
		int sum = dir_1 + dir_2;
		if (sum == 4 || sum == 11) {
			return true;
		} else if (sum == 6) {
			if (dir_1 == 1 || dir_2 == 1) {
				return false;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}

	public int _Direction {
		get{ return orientation; }
	}

	public int _Orig_Direction{
		get{ return original_orientation; }
	}
		
	public coordinate _location{
		get{ return location; }
	}

	public coordinate _grid_location{
		get{ return grid_location; }
	}

	public int _index{
		get{ return index; }
	}

	public Room _room{
		get{ return connected_room; }
	}

	public Room _pair_room{
		get{ return pair._room; }
	}

	public Connection _pair{
		get{ return pair; }
	}

	public string _label{
		get{ return label; }
	}

	public int _pair_index {
		get {
			if (pair == null) {
				return -1;
			} else {
				return pair._index;
			}
		}
	}
			
}


