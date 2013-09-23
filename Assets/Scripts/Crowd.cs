using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crowd : MonoBehaviour {
	
	private List<GameObject> all_fans;
	
	void Start()
	{
		all_fans = new List<GameObject>();
			
		foreach(Transform child in transform) {
			if(child.parent == transform) {
				all_fans.Add(child.gameObject);
			}
		}
	}
	
	void Update() 
	{
		// randomly chooce a fan to cheer for his team
		if(Random.Range(0,20) == 0) {
			GameObject fan = all_fans[Random.Range(0, all_fans.Count)];
			
			Fan_Behaviour fan_behaviour = fan.GetComponent<Fan_Behaviour>();
			StartCoroutine(fan_behaviour.Celebrate());
		}
	}
}
