using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TextNode : MonoBehaviour, IDragHandler {

	private UnityEngine.UI.Text myText;
	private Information_Scrolling parent;

	// Use this for initialization
	void Start () {
		if (myText == null) {
			SetUp ("Defaulting");
		}
	}

	public void SetUp(string data){
		myText = GetComponent<UnityEngine.UI.Text> ();
		myText.text = data;
	}

	// Update is called once per frame
	void Update () {
	
	}

	public Information_Scrolling _parent{
		set{parent = value;}
	}

	public void OnDrag(PointerEventData eventData){
		//print ("Dragging");
		GameObject obj = eventData.pointerDrag;
		Vector2 pointPos = eventData.position;
		Vector3 myloc = obj.transform.position;
		//myloc.y =  pointPos.y;
		float move = pointPos.y - myloc.y;
		parent.MoveNodes (move);
	}

	public Vector3 position{
		get{ return transform.position; }
		set{ transform.position = value; }
	}

	public bool rendering{
		get{ return myText.enabled; }
		set{ myText.enabled = value; }
	}
}
