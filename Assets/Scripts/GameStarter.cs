using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStarter : MonoBehaviour {

	Game_Settings game_settings;
	int team_1_count;
	int team_2_count;
	float distance_team_1;
	float distance_team_2;

	public GameObject court_start_position_team_1;
	public GameObject court_start_position_team_2;

	public GameObject local_player_prefab;
	public GameObject local_game_prefab;

	public GameObject settings_prefab;

	// Use this for initialization
	void Start () {
		GameObject settings = GameObject.Find("Settings(Clone)");

		if(settings == null)
			settings = (GameObject)Instantiate(settings_prefab);

		game_settings = settings.GetComponent<Game_Settings>();

		team_1_count = game_settings.team_1_count;
		team_2_count = game_settings.team_2_count;

		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		distance_team_1 = court_lenght/(team_1_count+1);
		distance_team_2 = court_lenght/(team_2_count+1);

		StartLocalGame();

	}

	private void StartLocalGame()
	{
		Instantiate(local_game_prefab, Vector3.zero, transform.rotation);
		
		foreach (Hero_Selection.Player player in game_settings.players_list) {
			InstantiateNewLocalPlayer(CalculatePosition(player.team), player.team, player.player_name, player.controller, player.texture_id);
		}
	}

	private Vector3 CalculatePosition(int team)
	{
		Vector3 start_position = new Vector3(0,0,0);
		GameObject court_start_position;
		float distance_team;
		int team_count;

		if(team == 1) {
			court_start_position = court_start_position_team_1;
			distance_team = distance_team_1;
			team_count = team_1_count;
			team_1_count--;
		} else {
			court_start_position = court_start_position_team_2;
			distance_team = distance_team_2;
			team_count = team_2_count;
			team_2_count--;
		}

		start_position = court_start_position.transform.position;
		start_position.x = start_position.x + distance_team*team_count;

		return start_position;
	}

	private void InstantiateNewLocalPlayer(Vector3 start_position, int team, string name, int controller, int texture_id)
	{
		GameObject player = (GameObject)Instantiate(local_player_prefab, start_position, transform.rotation);
		
		Local_Player lp = (Local_Player)player.GetComponent<Local_Player>();
		lp.InitializePlayerInfo(team, name, start_position, controller, texture_id);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
