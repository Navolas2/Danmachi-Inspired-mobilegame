using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
 
public delegate void ClickAction();

public class Menu_Button : MonoBehaviour, IPointerClickHandler {

	private MenuManager parent;
	public int Menu_option = 0;
	protected ClickAction clickresponse = NoAction;
	// Use this for initialization
	void Start () {
		parent = GameObject.FindGameObjectWithTag ("Globals").GetComponent<MenuManager> ();
		UnityEngine.UI.Text mytext = GetComponentsInChildren<UnityEngine.UI.Text> () [0];
		string tx = "";
		clickresponse = parent.GetAction (Menu_option, out tx);
		mytext.text = tx;
		parent.AddSelf (this);
	}

	public static void NoAction(){
		//nothing happens
	}
		

	public void OnPointerClick(PointerEventData eventData){
		
		clickresponse ();
		
	}


	public Vector3 position{
		get{ return transform.position; }
		set{ transform.position = value; }
	}

	public bool rendering{
		get{ return gameObject.activeSelf; }
		set{ gameObject.SetActive(value); }
	}
}
