using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SPC1: MonoBehaviour {
	
	public TriggerBox trigger_box;
	public Font font;
	public Material red;
	public Material blue;
	
	private bool is_legal_goal;
	private bool ball_in_trigger_box;
	private bool player_in_trigger_box;

	public GameObject sphere;
		
	public GUIStyle SPC;
	
	public void Awake()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
	}
	
	public void OnGoal()
	{
		if(is_legal_goal)
			Debug.Log("GGGGGGGOOOOOOOOOOOOAAAAAAAAAAAAAALLLLLLLLLL");
	}
	
	public void enterTriggerBox(Collider collider, GameObject trigger_box)
	{
		if (collider.tag == "ball") {
			ball_in_trigger_box = true;
		}
		
		if (collider.tag == "player_collider") {
			player_in_trigger_box = true;
		}
	}
	
	public void stayTriggerBox(Collider collider, GameObject trigger_box)
	{
		if (collider.tag == "ball") {
			ball_in_trigger_box = true;
		}
		
		if (collider.tag == "player_collider") {
			player_in_trigger_box = true;
		}
	}
	
	public void playerTouchedBall()
	{
		ball_in_trigger_box = false;
		player_in_trigger_box = false;
	}
	
	
	void Update()
	{
		if(ball_in_trigger_box && player_in_trigger_box)
			is_legal_goal = true;
		else
			is_legal_goal = false;
	}
	void Start() 
	{
		
		SPC = new GUIStyle();
		SPC.font = font;
		SPC.fontSize = 40;
		SPC.normal.textColor = Color.white;
		SPC.alignment = TextAnchor.MiddleCenter;
	}
	
	public void DrawOutlinedText(Rect pos, string str)
	{
		SPC.normal.textColor = Color.black;
		pos.x--;
		GUI.Label(pos, str, SPC);
		pos.x += 4;
		GUI.Label(pos, str, SPC);
		pos.x -= 2;
		pos.y -= 2;
		GUI.Label(pos, str, SPC);
		pos.y +=4;
		GUI.Label(pos, str, SPC);
		pos.y--;
		SPC.normal.textColor = Color.white;
		GUI.Label(pos, str, SPC);
	}
	
	void OnGUI()
	{
		DrawOutlinedText(new Rect(Screen.width/2 - 10, 30, 10 , 10), "Challenge Nr. 1");
	}
}