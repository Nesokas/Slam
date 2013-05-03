using UnityEngine;
using System.Collections;

public class Score_Text_Behaviour : MonoBehaviour {
	
	public string team_1_name;
	public string team_2_name;
	public float fade_time = 200;
	
	private string text;
	private bool fade = false;
	private float time = 0;
	
	public void TeamScored(int team)
	{
		text = " team scored";
		if(team == 1)
			text = "Red" + text;
		else
			text = "Blue" + text;
		
		fade = true;
		time = 0;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(fade) {
			if(time > fade_time)
				guiTexture.color.a = 
		}
	
	}
}
