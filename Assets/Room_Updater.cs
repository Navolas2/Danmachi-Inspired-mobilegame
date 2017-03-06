using UnityEngine;
using System.Collections;

public class Room_Updater : MonoBehaviour
{
	public Room attached;

	// Update is called once per frame
	void Update ()
	{
		attached.Update ();
	}
}

