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
	
	/****************************************/
	
	public GameObject settings_prefab;
	public GameObject court_start_position_team_1;
	public GameObject court_start_position_team_2;
	
	public string[] tabs = new string[] {"Join", "Create", "Favorites", "Settings"};
	public string[] sortable_columns = new string[] {"Room Name", "Court", "Players", "Ping", "Country"};
	public string[] team_colors = new string[] {"Red", "Blue", "Green"};
	
	private int tab_selected;
	private int room_selected;
	private int total_players_connected;
	private int menu_state;	
	private int team_1_color;
	private int team_2_color;
	
    private bool[] toogle_columns;
	private bool[] room_selection;
	private bool offline_game;
	
	private string room_name;
	private string password;
	private string player_name;
	private string search;
	
	private Vector2 rooms_scroll_position;
	
	private ArrayList available_rooms;
	
	private List<Player> spectating;
	private List<Player> team_1;
	private List<Player> team_2;
	
	private GameObject settings;
	private Game_Settings game_settings;
	public Game_Behaviour game_behaviour;
	
	private struct SimplePlayer
	{
		public string name;
		public int team;
	}

	private struct Player
	{
		public SimplePlayer player;
		public NetworkPlayer network_player;
		public int controller;
		public bool is_network;
	}
	
	void InitializeMSF()
	{
		IPHostEntry host;
		
		host = Dns.GetHostEntry("magicbit.no-ip.biz");
		string ip = host.AddressList[0].ToString();
		
		MasterServer.ipAddress = ip;
		MasterServer.port = 23466;
		
		Network.natFacilitatorIP = ip;
		Network.natFacilitatorPort = 50005;
		
		MasterServer.ClearHostList();
	    MasterServer.RequestHostList(GAME_TYPE);
		MasterServer.updateRate = 2;
		
		HostData[] hostData = MasterServer.PollHostList();
		room_selection = new bool[hostData.Length];
		for(int i = 0; i < room_selection.Length; i++)
			room_selection[i] = false;
	}
	
	void Awake()
	{
		tab_selected = 0;
		room_selected = 0;
		available_rooms = new ArrayList();
		offline_game = false;
		
		room_name = "";
		password = "";
		player_name = "";
		search = "";
		
		team_1_color = 0;
		team_2_color = 1;
		
		menu_state = NICKNAME_SCREEN;
		
		toogle_columns = new bool[sortable_columns.Length];
		for(int i = 0; i < toogle_columns.Length; i++)
			toogle_columns[i] = false;
		
		InitializeMSF(); //MSF -> Master Server Facilitator
		
		spectating = new List<Player>();
		team_1 = new List<Player>();
		team_2 = new List<Player>();
	}
	
	void Start()
	{
		settings = GameObject.FindGameObjectWithTag("settings");
		if (settings != null) {
			game_settings = settings.GetComponent<Game_Settings>();
			List<Game_Settings.Player> player_list = game_settings.players;
			
			foreach(Game_Settings.Player player in player_list) {
				Player new_player = new Player();
				
				if (player.network_player != null)
					new_player.network_player = player.network_player;
				else
					new_player.controller = player.controller;
				
				new_player.player.name = player.name;
				new_player.player.team = player.team;
				
				if (player.team == 1)
					team_1.Add(new_player);
				else
					team_2.Add(new_player);
			}
			menu_state = LOBBY;
		}
	}

	void JoinRoom ()
	{
		// Draw sortable columns
		GUILayout.BeginHorizontal();
			toogle_columns[0] = GUILayout.Toggle(toogle_columns[0], sortable_columns[0], GUILayout.MinWidth(Screen.width*0.38f));
			for(int i = 1; i < sortable_columns.Length; i++)
				toogle_columns[i] = GUILayout.Toggle(toogle_columns[i], sortable_columns[i], GUILayout.MaxWidth(Screen.width*0.07f));
		GUILayout.EndHorizontal();
		
		// Draw existing rooms
		rooms_scroll_position = GUILayout.BeginScrollView(rooms_scroll_position);
			
			if (MasterServer.PollHostList().Length != 0) {
	            HostData[] hostData = MasterServer.PollHostList();
				available_rooms = new ArrayList();
	            for (int i = 0; i < hostData.Length; i++) {
					available_rooms.Add(hostData[i]);
	            }
	            MasterServer.ClearHostList();
	        }
			
			GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true));
				GUILayout.BeginVertical();
					string[] room_names = new string[available_rooms.Count];
					for(int i = 0; i < available_rooms.Count; i++) {
						room_names[i] = ((HostData)available_rooms[i]).gameName;
					}
					room_selected = GUILayout.SelectionGrid(room_selected, room_names, 1, GUILayout.MinWidth(Screen.width*0.38f), GUILayout.ExpandWidth(true));
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
				total_players_connected = 0;
				for(int i = 0; i < room_names.Length; i++) {
					GUILayout.BeginHorizontal();
					GUILayout.Label(((HostData)available_rooms[i]).gameType, GUILayout.MaxWidth(Screen.width*0.07f), GUILayout.Height(22));
					GUILayout.Label(((HostData)available_rooms[i]).connectedPlayers + "/" + ((HostData)available_rooms[i]).playerLimit, GUILayout.MaxWidth(Screen.width*0.07f), GUILayout.Height(22));
					Ping player_ping = new Ping(((HostData)available_rooms[i]).ip.ToString());
					GUILayout.Label(player_ping.time.ToString(), GUILayout.MaxWidth(Screen.width*0.07f), GUILayout.Height(22));
					GUILayout.Label("Country" + i, GUILayout.MaxWidth(Screen.width*0.07f), GUILayout.Height(22));
					GUILayout.EndHorizontal();
					total_players_connected = total_players_connected + 1 + ((HostData)available_rooms[i]).connectedPlayers;
				}
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
		
		//Connect and refresh buttons 
		GUILayout.BeginHorizontal();
			if(GUILayout.Button("Connect")) {
				Network.Connect(((HostData)available_rooms[room_selected]).ip, ((HostData)available_rooms[room_selected]).port);
				menu_state = LOBBY;
			}
			if(GUILayout.Button ("Refresh")) {
				MasterServer.ClearHostList();
        		MasterServer.RequestHostList(GAME_TYPE);
			}
			GUILayout.FlexibleSpace();
			search = GUILayout.TextField(search, GUILayout.MinWidth(100));
			GUILayout.Button("Search");
		GUILayout.EndHorizontal();
	}
	
	Player CreatePlayer(string player_name, int team)
	{
		SimplePlayer simple_player = new SimplePlayer();
		simple_player.name = player_name;
		simple_player.team = team;
		
		Player player = new Player();
		player.player = simple_player;
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
	
	void CreateRoom()
	{
		GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
			offline_game = GUILayout.Toggle(offline_game, "Offline Game");
			if (!offline_game){
				GUILayout.BeginHorizontal();
					GUILayout.Label("Room Name:", GUILayout.MaxWidth(Screen.width*0.2f));
					room_name = GUILayout.TextField(room_name);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Password:", GUILayout.MaxWidth(Screen.width*0.2f));
					password = GUILayout.PasswordField(password, '*');
				GUILayout.EndHorizontal();
			}
			if(GUILayout.Button("Create", GUILayout.ExpandWidth(false))){
				if(!offline_game){
					bool useNat = !Network.HavePublicAddress();
					Network.InitializeServer(32, 25002, useNat);
					
					// For now the game type will be "Default"
					MasterServer.RegisterHost(GAME_TYPE, room_name);
					
					AddNetworkPlayer(Network.player, player_name);
				} else {
					AddLocalPlayer(0, "Keyboard");
					string[] joysticks = Input.GetJoystickNames();
					for(int i = 0; i < joysticks.Length; i++) {
						AddLocalPlayer(i+1, joysticks[i]);
					}
				}
				menu_state = LOBBY;
			}
		GUILayout.EndVertical();
		
	}
	
	void StartNetworkGame()
	{
		int players_team_1 = team_1.Count;
		int players_team_2 = team_2.Count;
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
//		networkView.RPC("LoadSettings", RPCMode.Others);
		
		if(settings == null)
			networkView.RPC("LoadSettings", RPCMode.All);
		
		/* Preenche a lista de players (nao os do Game_Behaviour, mas sim outra estrutura a parte) do Game_Settings */
		for(int i = 0; i < team_1.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			SimplePlayer player = team_1[i].player;
			
			start_position = court_start_position_team_1.transform.position;
			start_position.x = start_position.x + distance_team_1*players_team_1;
			players_team_1--;
			game_settings.AddNetworkPlayer(player.team, player.name, start_position, team_1[i].network_player);
		}
		
		for(int i = 0; i < team_2.Count; i++){
			Vector3 start_position = new Vector3(0,0,0);
			SimplePlayer player = team_2[i].player;
			
			start_position = court_start_position_team_2.transform.position;
			start_position.x = start_position.x + distance_team_2*players_team_2;
			players_team_2--;
			game_settings.AddNetworkPlayer(player.team, player.name, start_position, team_2[i].network_player);
		}
		/*****************************************************************************************************************/
		game_settings.local_game = false;
		
		
		/* If we click start in the lobby and the game is already running, we'll want to keep the current running game so we can't simply load the scene again */
		/* instead, we destroy the lobby so we get back in the game */
		if(game_settings.is_game_running == true) {
			Debug.Log("GAME_IS_RUNNING");
			Network.RemoveRPCs(gameObject.networkView.viewID);
			Destroy(gameObject);
		}
		else {
			Debug.Log("NOT_RUNNING");
			game_settings.is_game_running = true;
			networkView.RPC("LoadGame", RPCMode.All);
		}
		/********************************************************************************************************************************************************/
	}
	
	void StartLocalGame ()
	{
		int players_team_0 = spectating.Count;
		int players_team_1 = team_1.Count;
		int players_team_2 = team_2.Count;
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
//		GameObject settings = (GameObject)Instantiate(settings_prefab);
//		Game_Settings game_settings = settings.GetComponent<Game_Settings>();
	
		for(int i = 0; i < team_1.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			SimplePlayer player = team_1[i].player;
			
			start_position = court_start_position_team_1.transform.position;
			start_position.x = start_position.x + distance_team_1*players_team_1;
			players_team_1--;
			
			game_settings.AddLocalPlayer(player.team, player.name, start_position, team_1[i].controller);
		}
		
		for(int i = 0; i < team_2.Count; i++){
			Vector3 start_position = new Vector3(0,0,0);
			SimplePlayer player = team_2[i].player;

			start_position = court_start_position_team_2.transform.position;
			start_position.x = start_position.x + distance_team_2*players_team_2;
			players_team_2--;
			
			game_settings.AddLocalPlayer(player.team, player.name, start_position, team_2[i].controller);
		}
		
		game_settings.local_game = true;
		Application.LoadLevel("Main_Game");
	}
	
	
	/* Instantiates settings_prefab (Game_Settings.cs) on clients and server*/
	[RPC]
	void LoadSettings()
	{
		settings = (GameObject)Instantiate(settings_prefab);
		game_settings = settings.GetComponent<Game_Settings>();
		game_settings.local_game = false;
	}
	
	[RPC]
	void LoadGame()
	{
		Application.LoadLevel("Main_Game");
	}
	
	/* When a new player connects to a lobby, this will handle the logic */
	void OnPlayerConnected(NetworkPlayer network_player)
	{
		Debug.Log("New Player Connected");
		networkView.RPC("AskName", network_player);
	}
	
	[RPC]
	void AskName()
	{
		networkView.RPC("TellName", RPCMode.Server, Network.player, player_name);
	}
	
	[RPC]
	void TellName(NetworkPlayer network_player, string new_player_name)
	{
		
		/******* Initialize new player lists *********/
		for(int i = 0; i < team_1.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, team_1[i].network_player, team_1[i].player.name, TEAM_1);
		for(int i = 0; i < team_2.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, team_2[i].network_player, team_2[i].player.name, TEAM_2);
		for(int i = 0; i < spectating.Count; i++)
			networkView.RPC("AddNetworkPlayer", network_player, spectating[i].network_player, spectating[i].player.name, SPECTATING);
		/********************************************/
		
		networkView.RPC("AddNetworkPlayer", RPCMode.All, network_player, new_player_name, SPECTATING);
	}
	/************************************************************************************/
	
	
	void DrawPlayers(List<Player> players, int team)
	{
		for(int i = 0; i < players.Count; i++) {
			bool change_team = false;
			int new_team = SPECTATING;
			
			GUILayout.BeginHorizontal();
				if((team == TEAM_2 || team == SPECTATING) && (offline_game || Network.isServer))
					if(GUILayout.Button("<", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_1;
						change_team = true;
					}
				GUILayout.FlexibleSpace();
				GUILayout.Label(players[i].player.name);
				if(players[i].network_player != Network.player) {
					GUILayout.Label("" + Network.GetAveragePing(players[i].network_player));
				}
				GUILayout.FlexibleSpace();
				if((team == TEAM_1 || team == SPECTATING) && (offline_game || Network.isServer)) {
					if(GUILayout.Button(">", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_2;
						change_team = true;
					}
				}
			GUILayout.EndHorizontal();
			if(change_team) {
				if(offline_game)
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
				AddLocalPlayer(controller, old_player_team[i].player.name, new_team);
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
				AddNetworkPlayer(network_player, old_player_team[i].player.name, new_team);
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

	void LobbyScreen ()
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
				if(!offline_game && GUILayout.Button("Disconnect", GUILayout.MinWidth(0.15f*Screen.width))){
					Network.Disconnect();
					ClearPlayersLists();
					menu_state = MAIN_MENU;
				}
				if(Network.isServer || offline_game){
					if(!offline_game)
						GUILayout.FlexibleSpace();
					if(GUILayout.Button("Start", GUILayout.MinWidth(0.15f*Screen.width))) {
						GameObject gb = GameObject.FindGameObjectWithTag("GameController");
						
						if(gb != null){
							game_behaviour = (Game_Behaviour)gb.GetComponent<Game_Behaviour>();
							game_behaviour.isOnLobbyScreen = false;
						}
						if(!offline_game)
							StartNetworkGame();
						else
							StartLocalGame();
					}
				}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	void OnDisconnectedFromServer()
	{
		ClearPlayersLists();
		menu_state = MAIN_MENU;
	}

	void NicknameScreen()
	{
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical("box", GUILayout.MaxHeight(0.1f*Screen.height), GUILayout.MaxWidth(0.26f*Screen.width));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Nickname:", GUILayout.Width(67));
						player_name = GUILayout.TextField(player_name, GUILayout.MinWidth(0.2f*Screen.width));
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("Exit", GUILayout.MinWidth(0.1f*Screen.width)))
							Application.Quit();
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("Start", GUILayout.MinWidth(0.1f*Screen.width)))
							menu_state = MAIN_MENU;
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
	
	void ClearPlayersLists()
	{
		spectating = new List<Player>();
		team_1 = new List<Player>();
		team_2 = new List<Player>();
	}

	void MainMenuScreen ()
	{
		GUILayout.BeginArea(new Rect(0,0, Screen.width*0.7f, Screen.height - Screen.height*0.02f));
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					// Draw tabs
					tab_selected = GUILayout.Toolbar(tab_selected, tabs);
					
					switch(tab_selected){
					case JOIN_TAB:
						JoinRoom ();
						break;
					case CREATE_TAB:
						CreateRoom();
						break;
					}
		
					//Version
					GUILayout.BeginHorizontal();
						if(GUILayout.Button("Back"))
							menu_state = NICKNAME_SCREEN;
						GUILayout.FlexibleSpace();
						GUILayout.Label("Version 1.0; " + total_players_connected + " online");
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void OnGUI()
	{	
		
		GUILayout.BeginArea(new Rect(Screen.width*0.01f, Screen.height*0.01f, Screen.width - Screen.width*0.02f, Screen.height - Screen.height*0.02f));
		switch(menu_state){
		case NICKNAME_SCREEN:
			NicknameScreen();
			break;
		case MAIN_MENU:
			MainMenuScreen();
			break;
		case LOBBY:
			LobbyScreen();
			break;
		}
		GUILayout.EndArea();
	}
	
}
