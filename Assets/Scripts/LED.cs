using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LED : MonoBehaviour {
	
	public bool is_red;
	private Material[] numbers;

	// Use this for initialization
	void Awake () {
		string resource_folder = "Materials/Numbers/";

		if(is_red)
			resource_folder += "Red/";
		else
			resource_folder += "Green/";

		numbers = new Material[10];
		for(int i = 0; i < 10; i++) {
			numbers[i] = Resources.Load<Material>(resource_folder + "Number_" + i);
		}

		gameObject.renderer.material = numbers[0];
	}
	
	public void SetCurrentNumber(int number)
	{
		gameObject.renderer.material = numbers[number];
	}
	
	
}
