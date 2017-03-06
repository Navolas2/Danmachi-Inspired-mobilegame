using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class AdjustPersonality : MonoBehaviour, IDragHandler, IEndDragHandler {

	public int trait = 0;
	public CreateCharacter main;
	private UnityEngine.UI.Slider slide;
	private bool update = true;

	// Use this for initialization
	void Start () {
		slide = GetComponent<UnityEngine.UI.Slider> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (update) {
			slide.value = main.personality.traitList [trait] [1];
		} 
	}

	public void ValueChange(float value){
		main.personality.UpdateTrait (value, trait);
	}

	public void OnDrag(PointerEventData eventData){
		update = false;
	}

	public void OnEndDrag (PointerEventData eventData){
		update = true;
	}

	public void ToggleLock(){
		main.personality.ToggleLock (trait);
	}

}
