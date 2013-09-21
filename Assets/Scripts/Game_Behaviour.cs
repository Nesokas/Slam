using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Behaviour : MonoBehaviour {

	public Vector3 ball_position = new Vector3(0, -0.04788643f, 0);

	public float team_1_inicial_z_position = 7.812522f;
	public float team_2_inicial_z_position = -7.812522f;

	public int num_team_1_players = 1;
	public int num_team_2_players = 1;

	public GameObject player_prefab;
	public GameObject ball_prefab;

	public Camera m_camera;
	public GameObject settings_prefab;
	
	public GameObject crowd_team_1;
	public GameObject crowd_team_2;
	
	public float timer = 200;

	protected List<GameObject> players_team_1 = new List<GameObject>();
	protected List<GameObject> players_team_2 = new List<GameObject>();
	protected GameObject ball;
	
	protected bool trigger_timer;
	protected bool finish_game = false;
	public float timer_value;
	
	protected int scored_team = 0;
	
	protected int score_team_1 = 0;
	protected int score_team_2 = 0;
	
	protected GUIManager gui_manager;
	public GUIStyle main_game_manager;
	protected bool is_celebrating = false;
	
	protected int team_celebrating;
	public AudioClip goal_cheer;
	
	protected int team_scored = 0;
	protected float DEFAULT_TEAM_SCORED_MESSAGE_XPOS = -400f;
	protected float TEAM_SCORED_MESSAGE_SPEED_MULTIPLIER = 400f;
	protected float team_scored_message_xpos;
	
	
	public GameObject spawn_team_1;
	public GameObject spawn_team_2;
	public GameObject player_controller;
	
	public void ScoreTeam(int team)
	{		
		if(team == 1) {
			TeamReaction(1, "Celebrate");
			TeamReaction(2, "Sad");
			scored_team = 1;
		} else {
			TeamReaction(2, "Celebrate");
			TeamReaction(1, "Sad");
			scored_team = 2;
		}	
		
		timer_value = 0f;
		trigger_timer = true;
	}
	
	public void FinishGame()
	{
		Application.LoadLevel(1);
	}
	
	public void ReleasePlayers()
	{
		NotificationCenter.DefaultCenter.PostNotification(this, "EnableGotoCenter");	
	}
	
	protected void MovePlayersToStartPositions(){}
	
	protected void StartGameAgain()
	{
		Debug.Log("start game again");
		int winning_team = StopCelebration();
		Crowd team_1_crowd = crowd_team_1.GetComponent<Crowd>();
		Crowd team_2_crowd = crowd_team_2.GetComponent<Crowd>();
		
		if(winning_team == 0) {
			MovePlayersToStartPositions();
			trigger_timer = false;
		} else {
			finish_game = true;
			timer_value = 0f;
			trigger_timer = true;
			
			if(winning_team == 1) {
				TeamReaction(1, "Celebrate");
				team_1_crowd.Celebrate();
				TeamReaction(2, "Sad");
				team_2_crowd.Sad();
			} else {
				TeamReaction(2, "Celebrate");
				team_2_crowd.Celebrate();
				TeamReaction(1, "Sad");
				team_1_crowd.Sad();
			}
		}
	}
	
	protected void TeamReaction(int team, string reaction)
	{
	
		Hashtable data = new Hashtable();
		data["team"] = team;
		data["reaction"] = reaction;
		
		NotificationCenter.DefaultCenter.PostNotification(this, "ChangeReaction", data);
	}

	protected bool IsOdd(int num)
	{
		return (num % 2) != 0;
	}

	void DestroyAllPlayers()
	{
		for(int i = 0; i < players_team_1.Count; i++)
			Destroy(players_team_1[i]);
		players_team_1.Clear();

		for(int i = 0; i < players_team_2.Count; i++)
			Destroy (players_team_2[i]);
		players_team_2.Clear();
	}

	protected string GetPlayerNum(string player_name)
	{
		return player_name.Replace("Player ", "");
	}

	protected void AddPlayerToTeam(int team, GameObject player) 
	{	
		if(team == 1)
			players_team_1.Add(player);
		else
			players_team_2.Add(player);
	}
	
	protected void Start() 
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
		GameObject guiManager = GameObject.FindGameObjectWithTag("GuiManager");
		gui_manager = guiManager.GetComponent<GUIManager>();
		
		m_camera = GameObject.FindGameObjectWithTag("MainCamera").camera;
		
		MovePlayersToStartPositions();
	}

	// Update is called once per frame
	protected void Update () 
	{
		
		if(trigger_timer){
			if(timer_value > timer && !finish_game)
				StartGameAgain();
			else if (timer_value > timer && finish_game)
				FinishGame();
			else timer_value++;
		}
		
		if(is_celebrating)
			team_scored_message_xpos += (Time.deltaTime * TEAM_SCORED_MESSAGE_SPEED_MULTIPLIER);
		
		if (Input.GetKey(KeyCode.Escape))
	        Application.LoadLevel(0);
	}
	
	public int StopCelebration()
	{
		is_celebrating = false;
		
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		if(score_team_1 == 5) {
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(goal_cheer, Vector3.zero);
			return 1;
		} else if(score_team_2 == 5) {
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(goal_cheer, Vector3.zero);
			return 2;
		}
		
		if (!is_celebrating)
			NotificationCenter.DefaultCenter.PostNotification(this, "StopCelebration");
		
		return 0;
	}
	
	protected void OnGoal(NotificationCenter.Notification notification)
	{
		if(!is_celebrating){
			if((int)notification.data["team"] == 1) {
				team_scored = 1;
				score_team_1++;
				ScoreTeam(1);
				team_celebrating = 1;
			}
			else {
				team_scored = 2;
				score_team_2++;
				ScoreTeam(2);
				team_celebrating = 2;
			}
			AudioSource.PlayClipAtPoint(goal_cheer, Vector3.zero);
			is_celebrating = true;
		}
	}
	
	protected void OnGUI()
	{	
		if(is_celebrating)
			gui_manager.DrawGoalScored(team_scored);
		 else
			gui_manager.DrawScore(score_team_1, score_team_2);
			
	}
}