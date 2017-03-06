using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class RoomFactory : MonoBehaviour{
	public static RoomFactory TheFactory;

	private List<Room> All_Rooms; //make into list of lists

	void Awake(){
		if (TheFactory == null) {
			DontDestroyOnLoad (gameObject);
			TheFactory = this;
			LoadData ();
		} else if (TheFactory != this) {
			Destroy (gameObject);
		}
	}

	private void LoadData(){
		All_Rooms = new List<Room> ();
		if (File.Exists (Application.dataPath + "/RoomInformation.xml")) {
			XmlDocument data = new XmlDocument ();
			data.Load (Application.dataPath + "/RoomInformation.xml");
			foreach (XmlNode room_data in data.DocumentElement.ChildNodes) {
				bool success = true;
				List<List<char>> layout;
				List<List<string[]>> path = new List<List<string[]>>();
				List<coordinate> doors;
				List<int> direction;
				List<string> label;
				List<int> Allowed = new List<int>();
				int[] size = new int[2];
				List<string> tags = new List<string>();
				string id = "";

				XmlNode layout_reader = room_data.SelectSingleNode ("Room_Layout");
				layout = ReadLayout (layout_reader);

				XmlNode pathway_reader = room_data.SelectSingleNode ("Room_Pathway");
				path = ReadPathway (pathway_reader);

				XmlNode door_reader = room_data.SelectSingleNode ("Doors");
				doors = ReadDoors (door_reader, out direction, out label);

				foreach (XmlNode tag_Data in room_data.SelectSingleNode("tags").ChildNodes) {
					tags.Add (tag_Data.InnerText);
				}

				foreach (XmlNode allowed_Data in room_data.SelectSingleNode("allowedfloors").ChildNodes) {
					Allowed.Add (int.Parse (allowed_Data.InnerText));
				}

				id = room_data.SelectSingleNode ("ID").InnerText;

				size[0] = int.Parse (room_data.SelectSingleNode ("sizex").InnerText);
				size[1] = int.Parse (room_data.SelectSingleNode ("sizey").InnerText);

			
				Room r = new Room(layout, path, doors, direction, label, Allowed, size,tags,id);
				if (bool.Parse (room_data.SelectSingleNode ("stairs").InnerText)) {
					r.SetStairs (int.Parse(room_data.SelectSingleNode("stairDir").InnerText));
				}

				All_Rooms.Add(r);

			}
		}
		else{
			File.Create (Application.dataPath+ "/RoomInformation.xml");
			print (Application.dataPath);
		}
	}

	private List<List<char>> ReadLayout(XmlNode layout_data){
		List<List<char>> outList = new List<List<char>> ();
		foreach (XmlNode row in layout_data) {
			List<char> r_list = new List<char> ();
			char[] row_array = row.InnerText.ToCharArray();
			for (int i = 0; i < row_array.Length; i++) {
				r_list.Add (row_array [i]);
			}
			outList.Add (r_list);
		}
		return outList;
	}

	private List<List<string[]>> ReadPathway(XmlNode layout_data){
		string numbers = "-0123456789";
		List<List<string[]>> outList = new List<List<string[]>> ();
		foreach (XmlNode row in layout_data) {
			List<string[]> r_list = new List<string[]> ();
			char[] row_array = row.InnerText.ToCharArray();
			int increase = 0;
			for (int i = 0; i < row_array.Length; i += increase) {
				increase = 0;
				int next = 0;
				string path_type = "";
				string length = "";
				while (next != 1) {
					
					path_type = path_type + row_array [i + increase];
					
					increase++;
					if (numbers.Contains ("" + row_array [i + increase])) {
						next = 1;
					}
				}
				while (next != 2) {
					length = length + row_array [i + increase];
					increase++;
					if (i + increase < row_array.Length) {
						if (!numbers.Contains ("" + row_array [i + increase])) {
							next = 2;
						}
					} else {
						next = 2;
					}
				}
				r_list.Add (new string[]{length, path_type});

			}
			outList.Add (r_list);

		}
		return outList;
	}

	private List<coordinate> ReadDoors(XmlNode door_data, out List<int> direction, out List<string> label){
		direction = new List<int> ();
		label = new List<string> ();
		List<coordinate> out_list = new List<coordinate> ();
		foreach (XmlNode row in door_data) {
			int x = int.Parse (row.SelectSingleNode ("x").InnerText);
			int z = int.Parse (row.SelectSingleNode ("z").InnerText);
			out_list.Add(new coordinate(0, x, z));
			direction.Add (int.Parse (row.SelectSingleNode ("direction").InnerText));
			label.Add (row.SelectSingleNode ("label").InnerText);
		}
		return out_list;
	}

	/// <summary>
	/// Returns a Random Room from all the room available that does not contain the provided tags and is available on the given floor
	/// </summary>
	/// <returns>A randomly selected Room</returns>
	/// <param name="exclude_tags">All the tags that the select room should not have</param>
	/// <param name="floor_number">The floor this room will be added to</param>
	public Room GetRandomRoom(string[] exclude_tags, int floor_number){
		List<Room> fits = new List<Room>();
		foreach (Room r in All_Rooms) {
			bool[] tags = r.has_tag (exclude_tags);
			if (CheckResults(tags, false)) {
				if (r.allowed_on_floor (floor_number)) {
					fits.Add (r);
				}
			}
		}
		Room out_room = new Room (fits [Random.Range (0, fits.Count)]);
		return out_room;
	}

	/// <summary>
	/// Returns a Random Room from all the roomss available that contains the provided tags and is available on the given floor
	/// </summary>
	/// <returns>TA randomly selected Room</returns>
	/// <param name="floor_number">The floor this room will be added to</param>
	/// <param name="explicit_tags">All the tags that the room should have</param>
	public Room GetRandomRoom(int floor_number, string[] explicit_tags){
		List<Room> fits = new List<Room>();
		foreach (Room r in All_Rooms) {
			bool[] tags = r.has_tag (explicit_tags);
			if (CheckResults(tags, true)) {
				if (r.allowed_on_floor (floor_number)) {
					fits.Add (r);
				}
			}
		}
		Room out_room = new Room (fits [Random.Range (0, fits.Count)]);
		return out_room;
	}

	private bool CheckResults(bool[] tester, bool result){
		
		for (int i = 0; i < tester.Length; i++) {
			if (!tester [i] == result) {
				return false;
			}
		}
		return true;
	}
}


