using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop_Child : MonoBehaviour, IDropHandler {

	private IDropHandler parent;
	// Use this for initialization
	void Start () {
		parent = GetComponentsInParent<IDropHandler> ()[1];
	}

	public void OnDrop(PointerEventData data){
		parent.OnDrop (data);
	}
}
