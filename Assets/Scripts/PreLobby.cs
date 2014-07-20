using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

public class PreLobby : Lobby 
{
	
	/*********** CONSTANTS ******************/

	private enum lobby_states {team_selection, hero_selection};

	/****************************************/

	public GameObject local_player_prefab;
	public GameObject choose_hero_prefab;
	private List<Camera> heroes_camera_list = new List<Camera>();
	public GameObject other_hero_choices_prefab;
	public GameObject network_loading_prefab;

	public Material team_1_material;
	public Material team_2_material;

	private int lobby_state;
	private int players_ready = 0;

	private Camera[] other_choices_cameras;
	private float native_horizontal_resolution = 1296f;
	private float native_vertical_resolution = 729f;

	bool[] is_pressed_button = new bool[4] {false, false, false, false};

	void Awake()
	{
		base.Awake();
		lobby_state = (int)lobby_states.team_selection;

		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		if(settings == null) {
			settings = (GameObject)Instantiate(settings_prefab, settings_prefab.transform.position, settings_prefab.transform.rotation);
			game_settings = settings.GetComponent<Game_Settings>();
			game_settings.local_game = true;
		} else {
			game_settings = settings.GetComponent<Game_Settings>();
			game_settings.local_game = game_settings.IsLocalGame();
		}
		
		if (game_settings.IsLocalGame()) {
			AddLocalPlayer(0, "Keyboard", SPECTATING);
			for (int i = 0; i < Input.GetJoystickNames().Length; i++) {
				AddLocalPlayer(i+1, "Gamepad " + (i+1), SPECTATING);
			}
		}

		show_lobby_arrows = true;
		
	}

	void HeroScreen()
	{
		if(other_choices_cameras != null && other_choices_cameras.Length != 0) {
			for(int i = 0; i < 4; i++) {
				if(other_choices_cameras[i]) {
					Camera other_hero_camera = other_choices_cameras[i];
					GUI.Box(new Rect(other_hero_camera.pixelRect.x, 
					                 Screen.height - other_hero_camera.pixelRect.yMax, 
					                 other_hero_camera.pixelWidth, 
					                 other_hero_camera.pixelHeight), 
					        "") ;
				}
			}
		}
	}

	[RPC]
	void NetworkHeroSelectScreen()
	{
		show_lobby = false;

		int total_players_team_1 = team_1.Count;
		int total_players_team_2 = team_2.Count;

		int team = 1;
		other_choices_cameras = new Camera[4];

		for(int i = 0; i < 4; i++) {

			GameObject other_hero_choices = (GameObject)Instantiate(other_hero_choices_prefab);
			Vector3 new_position = new Vector3(other_hero_choices.transform.position.x,
			                                   other_hero_choices.transform.position.y + 10*i,
			                                   other_hero_choices.transform.position.z
			                                  );

			other_hero_choices.transform.position = new_position;

			Camera other_hero_camera = other_hero_choices.transform.Find("Camera").GetComponent<Camera>();
			other_hero_camera.camera.rect = new Rect(0.2f + ((i%2) * 0.15f) + ((team - 1) * 0.35f), 0.80f, 0.1f, 0.15f);

			other_choices_cameras[i] = other_hero_camera;

			if(team == TEAM_1 && total_players_team_1 != 0) {

				Transform heroes = other_hero_choices.transform.Find("Heroes");
				foreach(Transform hero in heroes) {
					Transform hero_base = hero.Find("Base");
					hero_base.renderer.material = team_1_material;
				}
				total_players_team_1--;

				Player temp = team_1[i];
				temp.hero_choosen = other_hero_choices;
				team_1[i] = temp;

			} else if(team == TEAM_2 && total_players_team_2 != 0){

				Transform heroes = other_hero_choices.transform.Find("Heroes");
				foreach(Transform hero in heroes) {
					Transform hero_base = hero.Find("Base");
					hero_base.renderer.material = team_2_material;
				}
				total_players_team_2--;

				Player temp = team_2[i%2];
				temp.hero_choosen = other_hero_choices;
				team_2[i%2] = temp;

			} else {
				other_hero_camera.cullingMask = 0;
			}

			team += i % 2;
		}

		GameObject choose_hero = (GameObject)Instantiate(choose_hero_prefab);
		Camera choose_hero_camera = choose_hero.transform.Find("Main Camera").GetComponent<Camera>();
		choose_hero_camera.camera.rect = new Rect(0.2f, 0.1f, 0.6f, 0.6f);

		Hero_Selection hero_script = choose_hero.GetComponent<Hero_Selection>();
		Material team_color;

		if(self_player.team == 1) {
			hero_script.InitializeNetworkPlayer(TEAM_1, self_player.name, 0, Network.player, this);
		} else {
			hero_script.InitializeNetworkPlayer(TEAM_2, self_player.name, 0, Network.player, this);
		}
	}

	public void HeroChanged(int hero_index)
	{
		if(!game_settings.IsLocalGame()){
			networkView.RPC("RPC_HeroChanged", RPCMode.All, hero_index, Network.player);
		}
	}

	[RPC]
	void RPC_HeroChanged(int hero_index, NetworkPlayer network_player)
	{
		List<Player> allPlayers = new List<Player>(team_1);
		allPlayers.AddRange(team_2);

		for(int i = 0; i < allPlayers.Count; i++){
			if(allPlayers[i].network_player == network_player){
				OtherHeroChoices other = allPlayers[i].hero_choosen.GetComponent<OtherHeroChoices>();
				other.ChangeHero(hero_index);
			}
		}
	}

	private void BotActivationGUI()
	{


		for (int i = 0; i < heroes_camera_list.Count; i++) {
			//Debug.Log(screen_to_viewport.x);
			Vector3 add_bot_label_position = (heroes_camera_list[i].ViewportToScreenPoint(new Vector3((Screen.width*0.315f)/748, 0.5f, 0)));
			if(!is_pressed_button[i] && GUI.Button(new Rect(add_bot_label_position.x, -add_bot_label_position.y+Screen.height,100,30), "Add Bot")) {
				Debug.Log(is_pressed_button[i]);
				if (i == 0 || i == 2)
					game_settings.red_team_bots++;
				else
					game_settings.blue_team_bots++;

				is_pressed_button[i] = true;
				Debug.Log(is_pressed_button[i]);
			}
		}
	}

	void LocalHeroSelectScreen()
	{
		int total_players_team_1 = team_1.Count;
		int total_players_team_2 = team_2.Count;

		int player_number = 0;

		for(int i = 0; i < 4; i++) {

			float team = i % 2f;

			GameObject choose_hero = (GameObject)Instantiate(choose_hero_prefab);
			Camera choose_hero_camera = choose_hero.transform.Find("Main Camera").GetComponent<Camera>();
		
			Vector3 new_choose_hero_position = new Vector3(choose_hero.transform.position.x,
			                                               choose_hero.transform.position.y + 30*i,
			                                               choose_hero.transform.position.z
			                                              );
			choose_hero.transform.position = new_choose_hero_position;
			choose_hero_camera.camera.rect = new Rect(0.05f + (0.5f * team), 0.55f - (0.5f * player_number), 0.4f, 0.4f);

			choose_hero.transform.name = "team_" + (team + 1) + "_" + (player_number + 1);

			Hero_Selection hero_script = choose_hero.GetComponent<Hero_Selection>();

			if((team + 1) == 1 && total_players_team_1 != 0) {
				hero_script.InitializeLocalPlayer(TEAM_1, team_1[player_number].name, i, team_1[player_number].controller, this);
				total_players_team_1--;
			} else if((team + 1) == 2 && total_players_team_2 != 0) {
				hero_script.InitializeLocalPlayer(TEAM_2, team_2[player_number].name, i, team_2[player_number].controller, this);
				total_players_team_2--;
			} else {
				heroes_camera_list.Add(choose_hero_camera);
				GameObject lights = choose_hero.transform.Find("Lights").gameObject;
				lights.SetActive(false);
				GameObject halo = choose_hero.transform.Find("ready_led").Find("Halo").gameObject;
				halo.SetActive(false);
				hero_script.SetTeam((int)team + 1);
			//	PrintAddBotOverlay();
			}

			player_number += (i%2);
		}
	}

	public void PlayerReady(Hero_Selection.Player player)
	{
		if(game_settings.IsLocalGame()) {
			players_ready++;
			game_settings.AddPlayer(player);

			if (players_ready == (team_1.Count + team_2.Count))
				Application.LoadLevel("Main_Game");

		} else {
			networkView.RPC("RPC_PlayerReady", RPCMode.All, Network.player, player.texture_id);
		}
	}

	[RPC]
	void RPC_PlayerReady(NetworkPlayer network_player, int texture_id) {

		players_ready++;

		List<Player> allPlayers = new List<Player>(team_1);
		allPlayers.AddRange(team_2);
		
		for(int i = 0; i < allPlayers.Count; i++){
			if(allPlayers[i].network_player == network_player){
				Hero_Selection.Player player = new Hero_Selection.Player();
				OtherHeroChoices other = allPlayers[i].hero_choosen.GetComponent<OtherHeroChoices>();

				player.hero_index = other.hero_index;
				player.player_name = allPlayers[i].name;
				player.team = allPlayers[i].team;
				player.texture_id = texture_id;
				player.network_player = allPlayers[i].network_player;

				if(Network.isServer) {
					game_settings.AddPlayer(player);
				}
				break;
			}
		}
	
		if (Network.isServer && players_ready == (team_1.Count + team_2.Count)) {
			Network.Instantiate(network_loading_prefab, 
			                    network_loading_prefab.transform.position,
			                    network_loading_prefab.transform.rotation,
			                    0);
			networkView.RPC("StartGame", RPCMode.All);
		}

	}

	[RPC]
	void StartGame()
	{
		Application.LoadLevel("Network_Loading");
	}

	protected override void LobbyMenu()
	{

		if(!game_settings.IsLocalGame() && GUILayout.Button("Disconnect", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))){
			bool is_server = Network.isServer;
			Network.Disconnect();
			if(is_server)
				MasterServer.UnregisterHost();
			BackToMainMenu();
		} else if(game_settings.IsLocalGame() && GUILayout.Button("Back", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))) {
			Application.LoadLevel(game_settings.main_menu_scene);
		}
		GUILayout.FlexibleSpace();
		if (game_settings.IsLocalGame()) {
			if(GUILayout.Button("Restart and Refresh", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))) {
				game_settings.players_list.Clear();
				Application.LoadLevel("Pre_Game_Lobby");
			}
			GUILayout.FlexibleSpace();
		}
		if(Network.isServer || game_settings.IsLocalGame()){
			if(!game_settings.IsLocalGame())
				GUILayout.FlexibleSpace();
			if(GUILayout.Button("Start", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))) {
				if(game_settings.IsLocalGame())
					LocalHeroSelectScreen();
				else
					networkView.RPC("NetworkHeroSelectScreen", RPCMode.All);

				lobby_state = (int)lobby_states.hero_selection;
				HeroScreen();
			}
		}
	}

	protected override void LobbyStates()
	{
		switch (lobby_state)
		{
		case (int)lobby_states.team_selection:
			LobbyScreen();
			break;
		case (int)lobby_states.hero_selection:
			BotActivationGUI();
			break;
		}
	}

}
