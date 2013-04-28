using UnityEngine;
using System.Collections;

public class Menu_Behaviour : MonoBehaviour {

	float scale = 0.0025f;

	private float buttons_scale_width;
	
	void Start()
	{
		buttons_scale_width = Screen.height * scale;
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width/2 - 35*buttons_scale_width, Screen.height/2 - 20*buttons_scale_width, 70*buttons_scale_width, 25*buttons_scale_width), "Start"))
			Application.LoadLevel(1);
	
		if(GUI.Button(new Rect(Screen.width/2 - 35*buttons_scale_width, Screen.height/2 + 50*buttons_scale_width, 70*buttons_scale_width, 25*buttons_scale_width), "Exit"))
			Application.Quit();
	}
}
