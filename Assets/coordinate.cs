using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class coordinate
{
	private float x;
	private float y; //Vertical Position
	private float z;

	private int layer;

	int room_index;

	private int floor;
	private char note = ' ';

	public coordinate ()
	{
		x = 0f;
		y = 0f;
		z = 0f;
		floor = 0;
		room_index = -1;
		layer = 0;
	}

	public coordinate(int floor_target)
	{
		floor = floor_target;
		x = -1; //No position will ever be negative
		y = -1;
		z = -1;
		room_index = -1;
		layer = -1;
	}

	public coordinate(int floor_target, char note)
	{
		floor = floor_target;
		x = -1; //No position will ever be negative
		y = -1;
		z = -1;
		room_index = -1;
		layer = -1;
		this.note = note;
	}

	public coordinate(int floor_target, int room)
	{
		floor = floor_target;
		x = -1; //No position will ever be negative
		y = -1;
		z = -1;
		room_index = room;
		layer = -1;
	}

	public coordinate(coordinate coor){
		this.x = coor.x;
		this.y = coor.y;
		this.z = coor.z;
		this.floor = coor.floor;
		this.room_index = coor.room_index;
		this.layer = coor.layer;
	}

	public coordinate(int layer, int x, int z)
	{
		this.x = x;
		y = 0;
		this.z = z;
		this.layer = layer;
		floor = 0;
		room_index = -1;
	}

	public coordinate(int x, int z, int layer, int flr)
	{
		this.x = x;
		y = 0;
		this.z = z;
		this.layer = layer;
		floor = flr;
		room_index = -1;
	}

	public void Randomize(int layers, int x, int z){
		this.x = Random.Range (1, (float)x - 1);
		this.z = Random.Range (1, (float)z) - 1;
		this.layer = layers;
	}

	public void RandomShift(){
		this.x = (float)Mathf.RoundToInt (x);
		this.y = (float)Mathf.RoundToInt (z);
		this.z = (float)Mathf.RoundToInt (z);
		x += (Random.value - .5f);
		z += (Random.value - .5f);
	}

	/// <summary>
	/// Determines if two locations are the same
	/// </summary>
	/// <returns><c>true</c> if this instance is location the specified location otherwise, <c>false</c>.</returns>
	/// <param name="coor">This is the coordinate that is non-static</param>
	public bool IsLocation(coordinate coor){
		bool same = true;
		if (note == ' ') {
			same = same && this.floor == coor.floor;
		} else if (note == '+') {
			same = same && this.floor <= coor.floor;
		} else if (note == '-') {
			same = same && this.floor >= coor.floor;
		}
		if (this.room_index != -1 && coor.room_index != -1) {
			same = same && this.room_index == coor.room_index;
		}
		if (this.x != -1 && coor.x != -1) {
			same = same && this.x == coor.x;
			same = same && this.y == coor.y;
			same = same && this.z == coor.z;
		}
		return same;
	}

	public void AdjustPosition(int layer, int floor, int index){
		this.layer = layer;
		this.floor = floor;
		this.room_index = index;
	}

	public void ChangePosition(coordinate loc){
		this.x = loc.x;
		this.y = loc.y;
		this.z = loc.z;
		this.floor = loc.floor;
		this.room_index = loc.room_index;
	}

	/// <summary>
	/// Distance between two coordinates on a flat plan. Y is ignored
	/// </summary>
	/// <param name="coor">The coordinate that is being used to look for the distance</param>
	public int Distance(coordinate coor){
		int dist = 0;
		dist += Mathf.Abs (this._x_int - coor._x_int);
		dist += Mathf.Abs (this._z_int - coor._z_int);
		return dist;
	}

	public int GeneralDirection(coordinate coor){
		//Given a location. Calculates the general direction of travel needed to reach the given location
		//1 forward, 2 right, 3 back, 4 left, 5 up a layer, 6 down a layer, 7 up a floor, 8 down a floor
		if (this.floor != coor.floor) {
			if (this.floor > coor.floor) {
				return 8;
			} else {
				return 7;
			}
		}
		float x_dif = Mathf.Abs(this.x - coor.x);
		float y_dif = Mathf.Abs(this.y - coor.y);
		float z_dif = Mathf.Abs(this.z - coor.z);
		if (x_dif > y_dif && x_dif > z_dif) {
			return (this.x - coor.x) > 0 ? 4 : 2;
		} else if (y_dif > x_dif && y_dif > z_dif) {
			return (this.y - coor.y) > 0 ? 6 : 5;
		} else if (z_dif > x_dif && z_dif > y_dif) {
			return (this.z - coor.z) > 0 ? 3 : 1;
		} else {
			return 0;
		}
	}

	public void Move(int dir){
		switch (dir) {
		case 1:
			z += 1;
			break;
		case 2:
			x += 1;
			break;
		case 3:
			z -= 1;
			break;
		case 4:
			x -= 1;
			break;
		case 5:
			y += 1;
			break;
		case 6:
			y -= 1;
			break;
		}
	}

	public bool FloorOnly(){
		return this.x == -1;
	}

	public int _floor {
		get{return floor; }
	}

	public int _layer{
		get{ return layer; }
	}

	public float _x {
		get { return x; }
		set { x = value; }
	}

	public float _y {
		get {return y; }
		set { y = value; }
	}

	public float _z{
		get {return z; }
		set { z = value; }
	}

	public int _x_int {
		get { return Mathf.RoundToInt(x); }
	}

	public int _y_int {
		get {return Mathf.RoundToInt(y); }
	}

	public int _z_int {
		get {return Mathf.RoundToInt(z); }
	}

	public int _room{
		get { return room_index; }
	}

	public override string ToString ()
	{
		return string.Format ("[coordinate: _floor={0}, _room={4}, _x={1}, _y{2} , _z={3}]", _floor, _x, _y, _z, room_index);
	}
}


