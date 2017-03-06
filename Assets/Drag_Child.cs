using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag_Child : MonoBehaviour, IDragHandler {

	private IDragHandler parent;
	// Use this for initialization
	void Start () {
		parent = GetComponentsInParent<IDragHandler> ()[1];
	}

	public void OnDrag(PointerEventData data){
		parent.OnDrag (data);
	}
}
