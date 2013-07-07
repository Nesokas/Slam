using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_Behaviour : MonoBehaviour {

	public Vector3 ball_position = new Vector3(0, 0.06968546f, 0);

	public float team_1_inicial_z_position = 7.812522f;
	public float team_2_inicial_z_position = -7.812522f;

	public int num_team_1_players = 1;
	public int num_team_2_players = 1;

	public GameObject team_1_prefab;
	public GameObject team_2_prefab;
	public GameObject player_prefab;
	public GameObject ball_prefab;

	public Camera m_camera;
	public GameObject settings_prefab;
	
	public GameObject crowd_team_1;
	public GameObject crowd_team_2;
	
	public GameObject screen_text;
	private Screen_Text_Behaviour screen_text_behaviour;
	
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
	
	private GUIManager gui_manager;
	public GUIStyle main_game_manager;
	private bool is_celebrating = false;
	
	private int team_celebrating;
	public AudioClip winning;
	
	public void ScoreTeam(int team)
	{
		Debug.Log("score team");
		Crowd team_1_crowd = crowd_team_1.GetComponent<Crowd>();
		Crowd team_2_crowd = crowd_team_2.GetComponent<Crowd>();
		
		if(team == 1) {
			TeamReaction(1, "Celebrate");
			team_1_crowd.Celebrate();
			TeamReaction(2, "Sad");
			team_2_crowd.Sad();
			scored_team = 1;
		} else {
			TeamReaction(2, "Celebrate");
			team_2_crowd.Celebrate();
			TeamReaction(1, "Sad");
			team_1_crowd.Sad();
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
		int winning_team = StopCelebration();
		Crowd team_1_crowd = crowd_team_1.GetComponent<Crowd>();
		Crowd team_2_crowd = crowd_team_2.GetComponent<Crowd>();
		
		if(winning_team == 0) {
			MovePlayersToStartPositions();
			trigger_timer = false;
			team_1_crowd.Idle();
			team_2_crowd.Idle();
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

	void InstanciatePlayer(int team, Vector3 position, int team_position, Game_Settings game_settings) 
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

	void MovePlayersToStartPositions()
	{
		Vector3 position;
		int i;
		Vector3 odd_position;

		gamepad_num = 1;
		keyboard_selected = false;

		DestroyAllPlayers();

		//Destroy(ball);
		//ball = (GameObject)Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation);
		//ball.transform.name = "Ball";
		ball.transform.position = ball_position;
		ball.transform.rigidbody.velocity = Vector3.zero;
		if (scored_team != 0) {
			Ball_Behaviour bb = ball.GetComponent<Ball_Behaviour>();
			bb.GameHasRestarted();
		}

		num_team_1_players = game_settings.players_team_1.Count;
		num_team_2_players = game_settings.players_team_2.Count;

		if (IsOdd(num_team_1_players)) {

			position = new Vector3(0, 0.1012379f, team_1_inicial_z_position);

			for(i = 0; i < num_team_1_players; i++) {
				if(IsOdd(i)) {
					odd_position = new Vector3(-position.x, position.y, position.z);
					InstanciatePlayer(1, odd_position, i, game_settings);
				} else {
					InstanciatePlayer(1, position, i, game_settings);
					position.x += players_distance;
				}
			}
		} else {

			position = new Vector3(0, 0.1012379f, team_1_inicial_z_position);

			for(i = 0; i < num_team_1_players; i++) {
				if(IsOdd(i)) {
					odd_position = new Vector3(-position.x, position.y, position.z);
					InstanciatePlayer(1, odd_position, i, game_settings);
				} else {
					position.x += players_distance;
					InstanciatePlayer(1, position, i, game_settings);
				}
			}
		}

		if (IsOdd(num_team_2_players)) {

			position = new Vector3(0, 0.1012379f, team_2_inicial_z_position);

			for(i = 0; i < num_team_2_players; i++) {
				if(IsOdd(i)) {
					odd_position = new Vector3(-position.x, position.y, position.z);
					InstanciatePlayer(2, odd_position, i, game_settings);
				} else {
					InstanciatePlayer(2, position, i, game_settings);
					position.x += players_distance;
				}
			}
		} else {
			position = new Vector3(0, 0.1012379f, team_2_inicial_z_position);

			for(i = 0; i < num_team_2_players; i++) {
				if(IsOdd(i)) {
					odd_position = new Vector3(-position.x, position.y, position.z);
					InstanciatePlayer(2, odd_position, i, game_settings);
				} else {
					position.x += players_distance;
					InstanciatePlayer(2, position, i, game_settings);
				}
			}
		}
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
		gui_manager = new GUIManager("MainGame");
		ball = (GameObject)Instantiate(ball_prefab, ball_position, ball_prefab.transform.rotation);
		ball.transform.name = "Ball";
	}
	void Start () 
	{
		settings = GameObject.FindGameObjectWithTag("settings");
		screen_text_behaviour = screen_text.GetComponent<Screen_Text_Behaviour>();
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");

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
	}
	
	public int StopCelebration()
	{
		is_celebrating = false;
		if(score_team_1 == 5) {
		//	ChangeScoreText("Red Team WINS", team1_color.color);
		//	time_to_stop = 0;
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(winning, Vector3.zero);
			return 1;
		} else if(score_team_2 == 5) {
		//	ChangeScoreText("Blue Team WINS", team2_color.color);
			//time_to_stop = 0;
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(winning, Vector3.zero);
			return 2;
		}
		
		return 0;
	}
	
	void OnGoal(NotificationCenter.Notification notification)
	{
		if(!is_celebrating){
			if((int)notification.data["team"] == 1) {
				score_team_2++;
				ScoreTeam(2);
				team_celebrating = 2;
				gui_manager.DrawGoalScored(2);
			}
			else {
				score_team_1++;
				ScoreTeam(1);
				team_celebrating = 1;
				gui_manager.DrawGoalScored(1);
			}
			is_celebrating = true;
		}
	}
	
	void OnGUI()
	{
		gui_manager.DrawScore(score_team_1, score_team_2);
	}
}