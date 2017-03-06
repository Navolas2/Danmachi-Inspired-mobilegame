using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer_Child : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
	private IPointerUpHandler parent_up;
	private IPointerDownHandler parent_down;
	public int Parent_index;
	// Use this for initialization
	void Start () {
		parent_up = GetComponentsInParent<IPointerUpHandler> ()[Parent_index];
		parent_down = GetComponentsInParent<IPointerDownHandler> () [Parent_index];
	}

	public void OnPointerDown(PointerEventData data){
		parent_down.OnPointerDown (data);
	}

	public void OnPointerUp(PointerEventData data){
		parent_up.OnPointerUp (data);
	}
}


