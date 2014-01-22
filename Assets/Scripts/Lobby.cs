using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

public class Lobby : MonoBehaviour 
{
	
	/*********** CONSTANTS ******************/
	
	private const int JOIN_TAB = 0;
	private const int CREATE_TAB = 1;
	private const int FAVOURITES_TAB = 2;
	private const int SETTINGS_TAB = 3;
	
	private const int NICKNAME_SCREEN = 0;
	private const int MAIN_MENU = 1;
	private const int LOBBY = 2;
	
	private const int SPECTATING = 0;
	private const int TEAM_1 = 1;
	private const int TEAM_2 = 2;
	
	private const string GAME_TYPE = "Default";

	private enum lobby_states {team_selection, hero_selection};
	
	/****************************************/
	
	public GameObject settings_prefab;
	public GameObject local_player_prefab;
	public GameObject net_player_prefab;
	public GameObject local_game_prefab;
	public GameObject net_game_prefab;
	public GameObject court_start_position_team_1;
	public GameObject court_start_position_team_2;
	
	public string[] team_colors = new string[] {"Red", "Blue", "Green"};

	private int team_1_color;
	private int team_2_color;

	private bool local_game;
	private bool show_lobby;
	private bool escape_key_pressed;
	
	private List<Player> spectating;
	private List<Player> team_1;
	private List<Player> team_2;
	
	private GameObject settings;
	private Game_Settings game_settings;
	private GameObject game_manager_object;
	
	public GameObject[] heros;
	private int lobby_state;

	private struct Player
	{
		public string name;
		public int team;
		public NetworkPlayer network_player;
		public int controller;
		public bool is_network;

		public int hero;
		public bool ready;
	}
	
	void Awake()
	{
		show_lobby = true;
		escape_key_pressed = false;
		lobby_state = (int)lobby_states.team_selection;
		
		team_1_color = 0;
		team_2_color = 1;
		
		spectating = new List<Player>();
		team_1 = new List<Player>();
		team_2 = new List<Player>();
		
		game_manager_object = null;
		
		settings = GameObject.FindGameObjectWithTag("settings");
		if(settings == null) {
			settings = (GameObject)Instantiate(settings_prefab, settings_prefab.transform.position, settings_prefab.transform.rotation);
			game_settings = settings.GetComponent<Game_Settings>();
			game_settings.local_game = true;
		} else {
			game_settings = settings.GetComponent<Game_Settings>();
			game_settings.local_game = game_settings.IsLocalGame();
		}
		
		if(game_settings.IsLocalGame()) {
			local_game = true;

		} else {
			networkView.group = 1;
			Network.SetLevelPrefix(1);
			
			if(Network.isServer)
				AddNetworkPlayer(Network.player, game_settings.player_name, SPECTATING);
			else{
				Debug.Log("Connect to server");
				ConnectToServer();
			}
		}

		if (local_game) {
			AddLocalPlayer(0, "Keyboard", SPECTATING);
			for (int i = 0; i < Input.GetJoystickNames().Length; i++) {
				AddLocalPlayer(i+1, "Gamepad " + (i+1), SPECTATING);
			}
		}
	}
	
	void Start() {}
	
	Player CreatePlayer(string player_name, int team)
	{		
		Player player = new Player();
		player.name = player_name;
		player.team = team;
		return player;
	}
	
	[RPC]
	void AddNetworkPlayer (NetworkPlayer network_player, string player_name, int team = SPECTATING)
	{
		Player player = CreatePlayer((string)player_name, team);
		player.network_player = network_player;
		player.is_network = true;

		switch(team){
			case SPECTATING:
				spectating.Add(player);
				break;
			case TEAM_1:
				team_1.Add(player);
				break;
			case TEAM_2:
				team_2.Add(player);
				break;
		}
	}
	
	void AddLocalPlayer(int controller, string player_name, int team=0) 
	{
		Player player = CreatePlayer(player_name, team);
		player.controller = controller;
		player.is_network = false;
		switch(team){
		case SPECTATING:
			spectating.Add(player);
			break;
		case TEAM_1:
			team_1.Add(player);
			break;
		case TEAM_2:
			team_2.Add(player);
			break;
		}
	}
	
	void DestroyNetworkPlayer(GameObject[] players, NetworkPlayer network_player)
	{
		foreach(GameObject player_object in players) {
			Network_Player player = player_object.GetComponent<Network_Player>();
			if(player.owner == network_player) {
				Network.Destroy(player_object);
				return;
			}
		}
	}
	[RPC]
	void UpdateNetworkPlayer(Vector3 start_position, NetworkPlayer network_player, int team, string name, int texture_id)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		foreach(GameObject player_object in players) {
			Network_Player player = player_object.GetComponent<Network_Player>();
			if(player.owner == network_player) {
				player.InitializePlayerInfo(network_player, team, name, start_position, texture_id);
				player.Start();
				GameObject gbo = GameObject.FindGameObjectWithTag("GameController");
				Game_Behaviour gb = gbo.GetComponent<Game_Behaviour>();
				if (gb.is_game_going) {
					player.ReleasePlayers();
				} else {
					player.DisableGotoCenter(gb.scored_team);
				}
				//player.ReleasePlayers();
				return;
			}
		}
	}
	
	bool IsNetworkPlayerInstanciated(GameObject[] players, NetworkPlayer network_player)
	{
		foreach(GameObject player_object in players) {
			Network_Player player = player_object.GetComponent<Network_Player>();
			if(player.owner == network_player)
				return true;
		}
		
		return false;
	}
	
	bool IsLocalPlayerInstanciated(GameObject[] players, int controller)
	{
		foreach(GameObject player_object in players) {
			Local_Player player = player_object.GetComponent<Local_Player>();
			if(player.controller == controller)
				return true;
		}
		
		return false;
	}
	
	void DestroyLocalPlayer(GameObject[] players, int controller)
	{
		foreach(GameObject player_object in players) {
			Local_Player player = player_object.GetComponent<Local_Player>();
			if(player.controller == controller) {
				Destroy(player_object);
				return;
			}
		}
	}
	
	void UpdateLocalPlayer(GameObject[] players, Vector3 start_position, int controller, int team, string name, int texture_id)
	{
		foreach(GameObject player_object in players) {
			Local_Player player = player_object.GetComponent<Local_Player>();
			if(player.controller == controller) {
				player.InitializePlayerInfo(team, name, start_position, controller, texture_id);
				player.Start();
				player.ReleasePlayers();
				return;
			}
		}
	}
	
	void InstanciateNewNetworkPlayer(Vector3 start_position, NetworkPlayer network_player, int team, string name, int texture_id) 
	{
		GameObject player = (GameObject)Network.Instantiate(net_player_prefab, start_position, transform.rotation, 0);
						
		Network_Player np = (Network_Player)player.GetComponent<Network_Player>();
		np.InitializePlayerInfo(network_player, team, name, start_position, texture_id);
		np.Start();
		
		Network_Game net_game = game_manager_object.GetComponent<Network_Game>();
		if(net_game.is_game_going) {
			np.ReleasePlayer();
		} else {
			np.UpdateCollisions(net_game.scored_team);
		}
	}
	
	void InstanciateNewLocalPlayer(Vector3 start_position, int team, string name, int controller, int texture_id)
	{
		GameObject player = (GameObject)Instantiate(local_player_prefab, start_position, transform.rotation);
						
		Local_Player lp = (Local_Player)player.GetComponent<Local_Player>();
		lp.InitializePlayerInfo(team, name, start_position, controller, texture_id);
	}
	
	void StartNetworkGame()
	{
		int players_team_1 = team_1.Count;
		int players_team_2 = team_2.Count;
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
		if(game_manager_object == null)
			game_manager_object = (GameObject)Network.Instantiate(net_game_prefab, Vector3.zero, transform.rotation, 0);

		int texture_id = 0;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		for(int i = 0; i < team_1.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			start_position = court_start_position_team_1.transform.position;
			start_position.x = start_position.x + distance_team_1*players_team_1;
			players_team_1--;
			
			if(!IsNetworkPlayerInstanciated(players, team_1[i].network_player))
				InstanciateNewNetworkPlayer(start_position, team_1[i].network_player, team_1[i].team, team_1[i].name, texture_id);
			else
				networkView.RPC("UpdateNetworkPlayer", RPCMode.All, start_position, team_1[i].network_player, team_1[i].team, team_1[i].name, texture_id);
			
			texture_id++;
		}
		
		for(int i = 0; i < team_2.Count; i++){
			Vector3 start_position = new Vector3(0,0,0);
			start_position = court_start_position_team_2.transform.position;
			start_position.x = start_position.x + distance_team_2*players_team_2;
			players_team_2--;
			
			if(!IsNetworkPlayerInstanciated(players, team_2[i].network_player))
				InstanciateNewNetworkPlayer(start_position, team_2[i].network_player, team_2[i].team, team_2[i].name, texture_id);
			else
				networkView.RPC("UpdateNetworkPlayer", RPCMode.All, start_position, team_2[i].network_player, team_2[i].team, team_2[i].name, texture_id);
			
			texture_id++;
		}
		
		for(int i = 0; i < spectating.Count; i++){
			if(IsNetworkPlayerInstanciated(players, spectating[i].network_player))
				DestroyNetworkPlayer(players, spectating[i].network_player);
		}
		/*****************************************************************************************************************/
		
		show_lobby = false;
	}
	
	void StartLocalGame ()
	{
		int players_team_0 = spectating.Count;
		int players_team_1 = team_1.Count;
		int players_team_2 = team_2.Count;
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
		if(game_manager_object == null)
			game_manager_object = (GameObject)Instantiate(local_game_prefab, Vector3.zero, transform.rotation);
		
		int texture_id = 0;
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		for(int i = 0; i < team_1.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			start_position = court_start_position_team_1.transform.position;
			start_position.x = start_position.x + distance_team_1*players_team_1;
			players_team_1--;
			
			if(!IsLocalPlayerInstanciated(players, team_1[i].controller))
				InstanciateNewLocalPlayer(start_position, team_1[i].team, team_1[i].name, team_1[i].controller, texture_id);
			else
				UpdateLocalPlayer(players, start_position, team_1[i].controller, team_1[i].team, team_1[i].name, texture_id);
			
			texture_id++;
		}
		
		for(int i = 0; i < team_2.Count; i++){
			Vector3 start_position = new Vector3(0,0,0);
			start_position = court_start_position_team_2.transform.position;
			start_position.x = start_position.x + distance_team_2*players_team_2;
			players_team_2--;
			
			if(!IsLocalPlayerInstanciated(players, team_2[i].controller))
				InstanciateNewLocalPlayer(start_position, team_2[i].team, team_2[i].name, team_2[i].controller, texture_id);
			else
				UpdateLocalPlayer(players, start_position, team_2[i].controller, team_2[i].team, team_2[i].name, texture_id);
			
			texture_id++;
		}
		
		for(int i = 0; i < spectating.Count; i++){
			if(IsLocalPlayerInstanciated(players, spectating[i].controller))
				DestroyLocalPlayer(players, spectating[i].controller);
		}
		
		show_lobby = false;
	}
	
	void ConnectToServer()
	{
		show_lobby = false;
		Network.Connect(game_settings.connect_to.ip, game_settings.connect_to.port);
	}
	
	void OnConnectedToServer()
	{
		networkView.RPC("TellName", RPCMode.Server, Network.player, game_settings.player_name);
	}
	
	[RPC]
	void TellName(NetworkPlayer network_player, string new_player_name)
	{
		
		/******* Initialize new player lists *********/
		for(int i = 0; i < team_1.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, team_1[i].network_player, team_1[i].name, TEAM_1);
		for(int i = 0; i < team_2.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, team_2[i].network_player, team_2[i].name, TEAM_2);
		for(int i = 0; i < spectating.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, spectating[i].network_player, spectating[i].name, SPECTATING);
		/********************************************/
		
		networkView.RPC("AddNetworkPlayer", RPCMode.All, network_player, new_player_name, SPECTATING);
		networkView.RPC("StartPlayers", network_player);
	}
	
	[RPC]
	void StartPlayers()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach(GameObject player_object in players) {
			Network_Player player = player_object.GetComponent<Network_Player>();
			player.GetPlayerInfo();
			player.Start();
		}
	}
	
	void DrawPlayers(List<Player> players, int team)
	{
		for(int i = 0; i < players.Count; i++) {
			bool change_team = false;
			int new_team = SPECTATING;
			
			GUILayout.BeginHorizontal();
				if((team == TEAM_2 || team == SPECTATING) && (local_game || Network.isServer)) {
					if(GUILayout.Button("<", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_1;
						change_team = true;
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.Label(players[i].name);
				if(!local_game && players[i].network_player != Network.player) {
					GUILayout.Label("" + Network.GetAveragePing(players[i].network_player));
				}
				GUILayout.FlexibleSpace();
				if((team == TEAM_1 || team == SPECTATING) && (local_game || Network.isServer)) {
					if(GUILayout.Button(">", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_2;
						change_team = true;
					}
				}
			GUILayout.EndHorizontal();

			if(change_team) {
				if(local_game)
					ChangeLocalPlayerTeam(players[i].controller, team, new_team);
				else
					networkView.RPC("ChangeNetworkPlayerTeam", RPCMode.All, players[i].network_player, team, new_team);
			}
		}
	}
	
	void ChangeLocalPlayerTeam(int controller, int old_team, int new_team)
	{
		List<Player> old_player_team = spectating;
		
		switch(old_team){
		case SPECTATING:
			old_player_team = spectating;
			break;
		case TEAM_1:
			old_player_team = team_1;
			break;
		case TEAM_2:
			old_player_team = team_2;
			break;
		}
		
		for(int i = 0; i < old_player_team.Count; i++){
			if(old_player_team[i].controller == controller){
				AddLocalPlayer(controller, old_player_team[i].name, new_team);
				old_player_team.RemoveAt(i);
				return;
			}
		}
	}
	
	/* When in a lobby the admin moves the player between teams, we use this function*/
	[RPC]
	void ChangeNetworkPlayerTeam(NetworkPlayer network_player, int old_team, int new_team)
	{
		List<Player> old_player_team = spectating;
		
		switch(old_team){
			case SPECTATING:
				old_player_team = spectating;
				break;
			case TEAM_1:
				old_player_team = team_1;
				break;
			case TEAM_2:
				old_player_team = team_2;
				break;
		}
		
		for(int i = 0; i < old_player_team.Count; i++){
			if(old_player_team[i].network_player == network_player){
				AddNetworkPlayer(network_player, old_player_team[i].name, new_team);
				old_player_team.RemoveAt(i);
				return;
			}
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer network_player)
	{
		Network.RemoveRPCs(network_player);
		Network.DestroyPlayerObjects (network_player);
		
		networkView.RPC("RemovePlayer", RPCMode.All, network_player);

		if(Network.isServer)
			MasterServer.UnregisterHost();
	}
	
	[RPC]
	void RemovePlayer(NetworkPlayer network_player)
	{
		for(int i = 0; i < team_1.Count; i++){
			if(team_1[i].network_player == network_player){
				team_1.RemoveAt(i);
				return;
			}
		}
		for(int i = 0; i < team_2.Count; i++){
			if(team_2[i].network_player == network_player){
				team_2.RemoveAt(i);
				return;
			}
		}
		for(int i = 0; i < spectating.Count; i++){
			if(spectating[i].network_player == network_player){
				spectating.RemoveAt(i);
				return;
			}
		}
	}

	void LobbyScreen()
	{
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUILayout.BeginVertical(GUILayout.MinWidth(0.3f*Screen.width));
					int temp_team_1_color = GUILayout.Toolbar(team_1_color, team_colors);
					if (temp_team_1_color != team_2_color)
						team_1_color = temp_team_1_color;
					GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
						DrawPlayers(team_1, TEAM_1);
					GUILayout.EndVertical();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(GUILayout.MinWidth(0.3f*Screen.width));
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Spectating");
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
						DrawPlayers(spectating, SPECTATING);
					GUILayout.EndVertical();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(GUILayout.MinWidth(0.3f*Screen.width));
					int temp_team_2_color = GUILayout.Toolbar(team_2_color, team_colors);
					if (temp_team_2_color != team_1_color)
						team_2_color = temp_team_2_color;
					GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
						DrawPlayers(team_2, TEAM_2);
					GUILayout.EndVertical();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(!local_game && GUILayout.Button("Disconnect", GUILayout.MinWidth(0.15f*Screen.width))){
					bool is_server = Network.isServer;
					Network.Disconnect();
					if(is_server)
						MasterServer.UnregisterHost();
					BackToMainMenu();
				}
				if (local_game) {
					if(GUILayout.Button("Restart and Refresh", GUILayout.MinWidth(0.15f*Screen.width)))
						Application.LoadLevel("Main_Game");
					GUILayout.FlexibleSpace();
				}
				if(Network.isServer || local_game){
					if(!local_game)
						GUILayout.FlexibleSpace();
					if(GUILayout.Button("Start", GUILayout.MinWidth(0.15f*Screen.width))) {
						if(!local_game)
							StartNetworkGame();
						else
							StartLocalGame();
					}
				}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Application.LoadLevel("Main_Menu");
	}
	
	void BackToMainMenu()
	{
		Application.LoadLevel("Main_Menu");
	}

	void HeroScreen()
	{

	}
	
	void OnGUI()
	{	
		if(show_lobby) {
			GUILayout.BeginArea(new Rect(Screen.width*0.01f, Screen.height*0.01f, Screen.width - Screen.width*0.02f, Screen.height - Screen.height*0.02f));
			switch (lobby_state)
			{
			case (int)lobby_states.team_selection:
				LobbyScreen();
				break;
			case (int)lobby_states.hero_selection:
				HeroScreen();
				break;
			}
			GUILayout.EndArea();
		}
	}
	
	void Update()
	{
		if (Input.GetKey(KeyCode.Escape)) {
			if(!escape_key_pressed) {
				show_lobby = !show_lobby;
				escape_key_pressed = true;
			}
		} else {
			escape_key_pressed = false;
		}
	}
	
}
