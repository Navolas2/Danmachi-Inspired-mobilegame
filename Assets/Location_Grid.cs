using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Location_Grid
{
	private List<List<List<List<DungeonReactable>>>> The_Grid;

	public Location_Grid (int y, int z, int x)
	{
		The_Grid = new List<List<List<List<DungeonReactable>>>>();
		for(int i = 0; i < y; i++){
			The_Grid.Add (new List<List<List<DungeonReactable>>> ());
			for (int j = 0; j < z; j++) {
				The_Grid [i].Add (new List<List<DungeonReactable>> ());
				for (int k = 0; k < x; k++) {
					The_Grid [i] [j].Add (new List<DungeonReactable> ());
				}
			}
		}
	}

	public Location_Grid(int z, int x)
	{
		new Location_Grid (1, z, x);
	}

	public List<DungeonReactable> MoveItem(DungeonReactable mover, coordinate old_loc, coordinate new_loc){
		List<DungeonReactable> items_at_location = The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int];
		The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int].Add (mover);
		The_Grid [old_loc._y_int] [old_loc._z_int] [old_loc._x_int].Remove (mover);

		return items_at_location;
	}

	public List<DungeonReactable> MoveItem(List<DungeonReactable> moving_objects, coordinate old_loc, coordinate new_loc){
		List<DungeonReactable> items_at_location = The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int];
		foreach(DungeonReactable mover in moving_objects){
			The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int].Add (mover);
			The_Grid [old_loc._y_int] [old_loc._z_int] [old_loc._x_int].Remove (mover);
		}

		return items_at_location;
	}

	public List<DungeonReactable> AddItem(DungeonReactable mover, coordinate new_loc){
		List<DungeonReactable> items_at_location = The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int];
		The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int].Add (mover);

		return items_at_location;
	}

	public List<DungeonReactable> AddItem(List<DungeonReactable> moving_objects, coordinate new_loc){
		List<DungeonReactable> items_at_location = The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int];
		foreach(DungeonReactable mover in moving_objects){
			The_Grid [new_loc._y_int] [new_loc._z_int] [new_loc._x_int].Add (mover);
		}

		return items_at_location;
	}

	public void RemoveItem(DungeonReactable mover, coordinate old_loc){
		The_Grid [old_loc._y_int] [old_loc._z_int] [old_loc._x_int].Remove (mover);
	}

	public void RemoveItem(List<DungeonReactable> moving_objects, coordinate old_loc){
		foreach(DungeonReactable mover in moving_objects){
			The_Grid [old_loc._y_int] [old_loc._z_int] [old_loc._x_int].Remove (mover);
		}
	}
}


