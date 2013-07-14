using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SPC1: MonoBehaviour {
	
	public TriggerBox trigger_box;
	
	private GUIManager gui_manager;
	
	private bool is_legal_goal;
	private bool ball_in_trigger_box;
	private bool player_in_trigger_box;

	public GameObject sphere;
		
	public GUIStyle SPC;
	
	public void Awake()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
//		gui_manager = new GUIManager("SPC1");
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
		
	}
	
	void OnGUI()
	{
		gui_manager.DrawChallengeTitle("Challenge 1");
	}
}