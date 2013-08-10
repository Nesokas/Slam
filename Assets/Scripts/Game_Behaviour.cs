using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Behaviour : MonoBehaviour {

	public Vector3 ball_position = new Vector3(0, 0.06968546f, 0);

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

	private List<GameObject> players_team_1 = new List<GameObject>();
	private List<GameObject> players_team_2 = new List<GameObject>();
	private GameObject ball;

	private float players_distance = 1.5f;
	private int gamepad_num = 1;
	private bool keyboard_selected = false;
	
	private GameObject settings;
	private Game_Settings game_settings;
	private bool trigger_timer;
	private bool finish_game = false;
	public float timer_value;
	
	private int scored_team = 0;
	
	private int score_team_1 = 0;
	private int score_team_2 = 0;
	
	public GameObject guiManager;
	private GUIManager gui_manager;
	public GUIStyle main_game_manager;
	private bool is_celebrating = false;
	
	private int team_celebrating;
	public AudioClip goal_cheer;
	
	private bool is_goal = false;
	private int team_scored = 0;
	private float DEFAULT_TEAM_SCORED_MESSAGE_XPOS = -400f;
	private float TEAM_SCORED_MESSAGE_SPEED_MULTIPLIER = 400f;
	private float team_scored_message_xpos;
	
	
	public GameObject spawn_team_1;
	public GameObject spawn_team_2;
	
	private int spawn_team = 0;
	
	public void ScoreTeam(int team)
	{
		Crowd team_1_crowd = crowd_team_1.GetComponent<Crowd>();
		Crowd team_2_crowd = crowd_team_2.GetComponent<Crowd>();
		
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
		for(int i = 0; i < players_team_1.Count; i++) {
			Kickoff_Player pb = players_team_1[i].GetComponent<Kickoff_Player>();
			pb.EnableGotoCenter();
		}
		
		for(int i = 0; i < players_team_2.Count; i++) {
			Kickoff_Player pb = players_team_2[i].GetComponent<Kickoff_Player>();
			pb.EnableGotoCenter();
		}
			
	}
	
	void StartGameAgain()
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
	
	void TeamReaction(int team, string reaction)
	{
		List<GameObject> players_team;
		if(team == 1)
			players_team = players_team_1;
		else
			players_team = players_team_2;
		
		for(int i = 0; i < players_team.Count; i++) {
			Player_Behaviour player_behaviour = players_team[i].GetComponent<Player_Behaviour>();
			player_behaviour.ChangeAnimation(reaction);
		}
	}

	bool IsOdd(int num)
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

	string GetPlayerNum(string player_name)
	{
		return player_name.Replace("Player ", "");
	}

	void AddPlayerToTeam(int team, GameObject player) 
	{	
		if(team == 1)
			players_team_1.Add(player);
		else
			players_team_2.Add(player);
	}

	string GetPlayer(int team, int position_in_team, Game_Settings game_settings)
	{
		if(team == 1)
			return game_settings.players_team_1[position_in_team];
		else
			return game_settings.players_team_2[position_in_team];
	}

	void InstantiatePlayer(int team, Vector3 position, int team_position, Game_Settings game_settings) 
	{
		GameObject player = (GameObject)Instantiate(player_prefab, position, player_prefab.transform.rotation);
		AddPlayerToTeam(team, player);

		string player_name = GetPlayer(team, team_position, game_settings);
		int player_num = System.Convert.ToInt32(GetPlayerNum(player_name));
		Kickoff_Player player_component = player.GetComponent<Kickoff_Player>();
		
		player_component.Start();
		player_component.InitializePlayerInfo(player_num, team, m_camera);
		player_component.ChangeAnimation("Idle");
		
		player_component.DisableGotoCenter(scored_team);

	}
	
	void SpawnPlayer(int team, Vector3 pos)
	{
		GameObject player = (GameObject)Network.Instantiate(player_prefab, pos, player_prefab.transform.rotation, 0);
		AddPlayerToTeam(team, player);

		string player_name = "Player";
		
		Kickoff_Player player_component = player.GetComponent<Kickoff_Player>();
		
		player_component.Start();
		player_component.InitializePlayerInfo(0, team, m_camera);
		player_component.ChangeAnimation("Idle");
		
		player_component.DisableGotoCenter(scored_team);
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server intitialized and ready");
		if (spawn_team + 1 == 1)
			SpawnPlayer(1, spawn_team_1.transform.position);
		
		else {
			SpawnPlayer(2, spawn_team_2.transform.position);
		}
		
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server intitialized and ready");
		if (spawn_team + 1 == 1)
			SpawnPlayer(1, spawn_team_1.transform.position);
		
		else {
			SpawnPlayer(2, spawn_team_2.transform.position);
		}
		
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
	}
		
	void MovePlayersToStartPositions()
	{
		Vector3 position;
		int i;
		Vector3 odd_position;

		gamepad_num = 1;
		keyboard_selected = false;

		DestroyAllPlayers();

		ball.transform.position = ball_position;
		ball.transform.rigidbody.velocity = Vector3.zero;
		if (scored_team != 0) {
			Ball_Behaviour bb = ball.GetComponent<Ball_Behaviour>();
			bb.GameHasRestarted();
		}

		num_team_1_players = game_settings.players_team_1.Count;
		num_team_2_players = game_settings.players_team_2.Count;

//		if (IsOdd(num_team_1_players)) {
//
//			position = new Vector3(0, 0.1012379f, team_1_inicial_z_position);
//
//			for(i = 0; i < num_team_1_players; i++) {
//				if(IsOdd(i)) {
//					odd_position = new Vector3(-position.x, position.y, position.z);
//					SpawnPlayer(1, odd_position, i, game_settings);
//				} else {
//					SpawnPlayer(1, position, i, game_settings);
//					position.x += players_distance;
//				}
//			}
//		} else {
//
//			position = new Vector3(0, 0.1012379f, team_1_inicial_z_position);
//
//			for(i = 0; i < num_team_1_players; i++) {
//				if(IsOdd(i)) {
//					odd_position = new Vector3(-position.x, position.y, position.z);
//					SpawnPlayer(1, odd_position, i, game_settings);
//				} else {
//					position.x += players_distance;
//					SpawnPlayer(1, position, i, game_settings);
//				}
//			}
//		}
//
//		if (IsOdd(num_team_2_players)) {
//
//			position = new Vector3(0, 0.1012379f, team_2_inicial_z_position);
//
//			for(i = 0; i < num_team_2_players; i++) {
//				if(IsOdd(i)) {
//					odd_position = new Vector3(-position.x, position.y, position.z);
//					SpawnPlayer(2, odd_position, i, game_settings);
//				} else {
//					SpawnPlayer(2, position, i, game_settings);
//					position.x += players_distance;
//				}
//			}
//		} else {
//			position = new Vector3(0, 0.1012379f, team_2_inicial_z_position);
//
//			for(i = 0; i < num_team_2_players; i++) {
//				if(IsOdd(i)) {
//					odd_position = new Vector3(-position.x, position.y, position.z);
//					SpawnPlayer(2, odd_position, i, game_settings);
//				} else {
//					position.x += players_distance;
//					SpawnPlayer(2, position, i, game_settings);
//				}
//			}
//		}
	}
	
	// variable used for testing (so we don't need to go always to the lobby screen)
	private bool skiped_lobby = false;
	
	void AddTestPlayers()
	{
		game_settings = settings.GetComponent<Game_Settings>();
		game_settings.AddNewPlayer(1, "Player 0");
		game_settings.AddNewPlayer(1, "Player 1");
		game_settings.AddNewPlayer(2, "Player 2");
		game_settings.AddNewPlayer(2, "Player 3");
	}

	void Awake()
	{
		ball = (GameObject)Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation);
		ball.transform.name = "Ball";
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
	}
	void Start () 
	{
		settings = GameObject.FindGameObjectWithTag("settings");
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
		gui_manager = guiManager.GetComponent<GUIManager>();

		if(settings == null) {
			settings = (GameObject)Instantiate(settings_prefab, new Vector3(0,0,0), settings_prefab.transform.rotation);
			AddTestPlayers();
			skiped_lobby = true;
		} else {
			game_settings = settings.GetComponent<Game_Settings>();
		}
		
		MovePlayersToStartPositions();
	}

	// Update is called once per frame
	void Update () {
		if(skiped_lobby) {
			AddTestPlayers();
			skiped_lobby = false;
		}
		
		if(trigger_timer){
			if(timer_value > timer && !finish_game)
				StartGameAgain();
			else if (timer_value > timer && finish_game)
				FinishGame();
			else timer_value++;
		}
		if(is_celebrating) {
			team_scored_message_xpos += (Time.deltaTime * TEAM_SCORED_MESSAGE_SPEED_MULTIPLIER);
		}
		
		if (Input.GetKey(KeyCode.Escape))
	    {
	        Application.LoadLevel(1);
	    }
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
	
	void OnGoal(NotificationCenter.Notification notification)
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
	
	void OnGUI()
	{	
		if(is_celebrating)
			gui_manager.DrawGoalScored(team_scored);
		 else
			gui_manager.DrawScore(score_team_1, score_team_2);
			
	}
}