using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Personality
{
	private static float TRAIT_MAX = 100f;
	private static float TRAIT_MIN = -100f;
	private static float  ANGER_MIN = 0f;
	private static float PERSONALITY_POINTS_MAX = 75f;

	//Personality Traits
	private List<List<float>> trait_lists;


	//Emotional Traits
	private List<List<float>> emotion_lists;

	private float spentPoints = 0f;

	public Personality ()
	{
		List<float> careful;
		List<float> seriousness;
		List<float> focus;
		List<float> kindness;
		List<float> wisdom;
		List<float> creativity;
		careful = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		seriousness  = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		focus  = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		kindness  = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		wisdom = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		creativity = new List<float> (){TRAIT_MIN, 0f, TRAIT_MAX, 0f};
		trait_lists = new List<List<float>> (){careful, seriousness, focus, kindness, wisdom, creativity};

		List<float> anger; //calm and angry
		List<float> happiness; //happy and sad
		List<float> affection; //love and hate
		anger = new List<float> (){ANGER_MIN, 0f, TRAIT_MAX};
		happiness = new List<float> (){ TRAIT_MIN, 0f, TRAIT_MAX };
		affection = new List<float> (){ TRAIT_MIN, 0f, TRAIT_MAX };
		emotion_lists = new List<List<float>> (){anger, happiness, affection};
	}

	public void adjustPersonalityStats(float s1, float s2, float s3, float s4, float s5, float s6)
	{
		trait_lists[0][1] = s1;
		trait_lists[1][1] = s2;
		trait_lists[2][1] = s3;
		trait_lists[3][1] = s4;
		trait_lists[4][1] = s5;
		trait_lists[5][1] = s6;
	}

	public void reset(){
		trait_lists[0][1] = 0;
		trait_lists[1][1] = 0;
		trait_lists[2][1] = 0;
		trait_lists[3][1] = 0;
		trait_lists[4][1] = 0;
		trait_lists[5][1] = 0;
	}

	public void UpdateTrait(float value, int trait){
		if (trait_lists [trait] [3] == 0) {
			float difference = 0f;
			float adjustable = 0f;
			float point_change = value - trait_lists [trait] [1];
			if (spentPoints + point_change > PERSONALITY_POINTS_MAX) {
				spentPoints += point_change;
				difference = PERSONALITY_POINTS_MAX - spentPoints;
				spentPoints = PERSONALITY_POINTS_MAX;
				adjustable = Count_Adjustable (trait);
				if (adjustable != 0) {
					difference = difference / adjustable;
				} else {
					difference = 0f;
					point_change = 0f;
				}
			} else {
				spentPoints += point_change;
			}
			bool finished = true;
			float extra_points = 0f;
			do {
				for (int i = 0; i < trait_lists.Count; i++) {
					if (i != trait && trait_lists [i] [3] == 0 && 
						trait_lists [i] [1] != trait_lists [i] [0]) {

						trait_lists [i] [1] += difference;

						if (trait_lists [i] [1] < trait_lists [i] [0]) {
							
							extra_points += trait_lists [i] [1] - trait_lists [i] [0];
							trait_lists [i] [1] = trait_lists [i] [0];
						}
					}
					else if(i == trait) {
						trait_lists [i] [1] += point_change;

						if (trait_lists [i] [1] > trait_lists [i] [2]) {
							extra_points += trait_lists [i] [1] - trait_lists [i] [2];
							trait_lists [i] [1] = trait_lists [i] [2];
						} else if (trait_lists [i] [1] < trait_lists [i] [0]) {
							trait_lists [i] [1] = trait_lists [i] [0];
						}
					}
				}
				if (extra_points != 0) {
					adjustable = Count_Adjustable (trait);
					if (adjustable != 0) {
						difference = extra_points / adjustable;
						extra_points = 0f;
						point_change = 0f;
						finished = false;
					} else {
						point_change = extra_points;
						spentPoints += point_change;
						trait_lists[trait][1] += extra_points;
						difference = 0f;
						finished = true;
					}
				} else {
					finished = true;
				}
			} while(!finished);
		}
	}

	private float Count_Adjustable(int trait){
		int count = 0;
		for (int i = 0; i < trait_lists.Count; i++) {
			if (trait_lists[i] [3] == 0 && i != trait && trait_lists[i][1] != trait_lists[i][0]) {
				count++;
			}
		}
		return (float)count;
	}

	public float AdjustPercent(float trait, float max_percent){
		if (max_percent > 1f) {
			max_percent = max_percent / 100f;
		}
		return (trait * max_percent) / 100f;
	}

	public void ToggleLock(int trait){
		if (trait_lists [trait] [3] == 0) {
			trait_lists [trait] [3] = 1;
		}
		else if (trait_lists [trait] [3] == 1) {
			trait_lists [trait] [3] = 0;
		}
	}

	public List<string> Calc_Affinity(){
		int lowest = 0;
		int highest = 0;
		float lowValue = 100;
		float highValue = -100;
		for(int i = 0; i < trait_lists.Count; i++){
			if (trait_lists [i] [1] > highValue) {
				highest = i;
				highValue = trait_lists [i] [1];
			}
			if (trait_lists [i] [1] < lowValue) {
				lowest = i;
				lowValue = trait_lists [i] [1];
			}
		}
		List<string> affinities = new List<string> ();
		affinities.Add (SwitchAffinity (lowest));
		affinities.Add (SwitchAffinity (highest));
		affinities.Add(SwitchAffinity(Random.Range(-15, 5)));

		return affinities;
	}

	private string SwitchAffinity(int i){
		switch (i) {
		case 2:
			return Spell.Affinity_Earth;
		case 0:
			return Spell.Affinity_Elec;
		case 1:
			return Spell.Affinity_Fire;
		case 3:
			return Spell.Affinity_Water;
		case 4:
			return Spell.Affinity_Wind;
		case 5:
			if(trait_lists[5][1] > 0)
				return Spell.Affinity_Light;
			else
				return Spell.Affinity_Shadow;
		}
		return Spell.Affinity_None;
	}

	List<float> careful;
	List<float> seriousness;
	List<float> focus;
	List<float> kindness;
	List<float> wisdom;
	List<float> creativity;

	public override string ToString ()
	{
		return string.Format ("[Personality: _careful={0}, _seriousness={1}, _focus={2}, _kindness={3}, _wisdom={4}, _creativity={5}, _anger={6}, _happyness={7}, _affection={8}]", _careful, _seriousness, _focus, _kindness, _wisdom, _creativity, _anger, _happyness, _affection);
	}

	public float _careful{
		get{return trait_lists[0][1]; }
	}

	public float _seriousness{
		get{return trait_lists[1][1]; }
	}

	public float _focus{
		get{return trait_lists[2][1];}
	}

	public float _kindness{
		get{return trait_lists[3][1]; }
	}

	public float _wisdom{
		get {return trait_lists[4][1]; }
	}

	public float _creativity{
		get{return trait_lists[5][1]; }
	}


	public float _anger{
		get{return emotion_lists[0][1]; }
	}

	public float _happyness {
		get{ return emotion_lists[1] [1]; }
	}

	public float _affection{
		get{ return emotion_lists[2] [1]; }
	}

	public List<List<float>> traitList{
		get{ return trait_lists; }
	}
}


