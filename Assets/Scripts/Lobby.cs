using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Lobby : MonoBehaviour {
	
	/************ CONSTANTS *****************/
	
	private const int INITIAL_SCREEN = 1;
	private const int NETWORK_GAME = 2;
	private const int LOCAL_GAME = 3;
	private const int CREATE_SERVER = 4;
	private const int JOIN_SERVER = 5;
	private const int LOBBY_NETWORK = 6;
	private const int LOBBY_LOCAL = 7;
	
	private const int LOCAL = 1;
	private const int NETWORK = 2;
	
	private const string GAME_TYPE = "Default";
	
	/**************************************/
	
	private string player_name = "";
	private string ip = "127.0.0.1";
	private string port = "8000";
	private string room_name = "";
		
	private int state;
	
	private Rect initial_box;
	private Rect nickname_field;
	private Rect ip_field;
	private Rect port_field;
	private Rect room_name_field;
	private Rect label;
	
	private GUIStyle label_style;
	
	private float w;
	private float h;
	
	private bool connect_pressed = false;
	
	private int buttons_width = 90;
	private int buttons_height = 25;
	
	private ArrayList available_rooms;
	
	// position from witch the court all beggins
	// for start placing the players
	public GameObject court_start_position_team_1;
	public GameObject court_start_position_team_2;
	
	public GameObject settings_prefab;
	
	[Serializable]
	private struct Player
	{
		public string name;
		public int team;
	}
	
	private struct ServerPlayer
	{
		public Player player;
		public NetworkPlayer network_player;
	}
	
	private struct Player_Local
	{
		public Player player;
		public int controller;
	}
	
	private List<Player> players;
	private List<Player_Local> local_players;
	private List<ServerPlayer> server_players;
	
	// Use this for initialization
	void Start () 
	{		
		
		state = INITIAL_SCREEN;
		players = new List<Player>();
		server_players = new List<ServerPlayer>();
		local_players = new List<Player_Local>();
		
		w = 300;
		h = 200;
		
		//Initial Box

		initial_box.width = w;
	  	initial_box.height = h;
		initial_box.x = Screen.width/2 - w/2;
	  	initial_box.y = Screen.height/2 - h/2;
		
		//Nickname Field
		nickname_field.width = w*0.94f - 70;
	  	nickname_field.height = 21;
		
		
		//Labels
		label = new Rect(0,0,0,0);
		label.width = w*0.7f;
		label.height = h*0.95f;
		
		//Ip Field
		ip_field.width = w*0.95f - 25;
	  	ip_field.height = 21;
		
		//port field
		port_field.width = w*0.95f - 35;
	  	port_field.height = 21;
		
		//room name field
		room_name_field.width = w*0.75f - 35;
	  	room_name_field.height = 21;
		
		InitializeMSF();
		
		available_rooms = new ArrayList();
	}
	
	// Function to initialize the master server and the facilitator IPs
	// and initialize the server room list
	void InitializeMSF()
	{
		MasterServer.ipAddress = "85.240.134.57";
		MasterServer.port = 23466;
		
		Network.natFacilitatorIP = "85.240.134.57";
		Network.natFacilitatorPort = 50005;
		
		MasterServer.ClearHostList();
	    MasterServer.RequestHostList(GAME_TYPE);
		MasterServer.updateRate = 2;
	}
	
	void AddPlayer(NetworkPlayer network_player, string player_name)
	{
		Player player = new Player();
		
		player.name = player_name;
		player.team = 0;
		
		ServerPlayer server_player = new ServerPlayer();
		server_player.player = player;
		server_player.network_player = network_player;
		server_players.Add(server_player);
	}

	void NetworkGame ()
	{
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f;
		GUI.Label(label, "Nickname");
		nickname_field.x = initial_box.x + 70;
		nickname_field.y = initial_box.y + h*0.33f;
		player_name = GUI.TextField(nickname_field, player_name);
		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
							   initial_box.y + h*0.6f,
							   buttons_width,
						       buttons_height),
		              "Create Room")) {
			state = CREATE_SERVER;
		}
		if(GUI.Button(new Rect(initial_box.x + w*0.9f - buttons_width, 
							   initial_box.y + h*0.6f,
							   buttons_width,
						       buttons_height),
		              "Join Room")) {
			state = JOIN_SERVER;
		}
		if(GUI.Button(new Rect(initial_box.x + initial_box.width/2 - buttons_width/2, 
							   initial_box.y + h*0.8f,
							   buttons_width,
						       buttons_height),
		              "Back")) {
			state = INITIAL_SCREEN;
		}
	}

	void CreateServer ()
	{
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f;
		GUI.Label(label, "Room Name");
		room_name_field.x = initial_box.x + 90f;
		room_name_field.y = initial_box.y + h*0.33f;
		room_name = GUI.TextField(room_name_field, room_name);
		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "Back")) {
			room_name = "";
			connect_pressed = false;
			state = INITIAL_SCREEN;
		}
		if(GUI.Button(new Rect(initial_box.x + w*0.9f - buttons_width, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "Create")) {
			if(room_name == ""){
				connect_pressed = true;
			} else {
				bool useNat = !Network.HavePublicAddress();
				Network.InitializeServer(32, 25002, useNat);
				
				// For now the game type will be "Default"
				MasterServer.RegisterHost(GAME_TYPE, room_name);
				
				state = LOBBY_NETWORK;
				AddPlayer(Network.player, player_name);
			}
		}
		if(connect_pressed){
			label.x = initial_box.x + w*0.33f;
			label.y = initial_box.y + h*0.6f;
			GUI.Label(label, "Insert a room name");
		}
	}
	
	private Vector2 scrollPosition = Vector2.zero;
	
	void JoinServer ()
	{
		
		GUILayout.BeginArea(new Rect(Screen.width*0.05f, Screen.height*0.05f, Screen.width*0.9f, Screen.height*0.9f));
		if (MasterServer.PollHostList().Length != 0) {
            HostData[] hostData = MasterServer.PollHostList();
			available_rooms = new ArrayList();
            for (int i = 0; i < hostData.Length; i++) {
				available_rooms.Add(hostData[i]);
            }
            MasterServer.ClearHostList();
        }
		
		GUILayout.Label("Join Room");
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width*0.9f), GUILayout.Height(Screen.height*0.8f));
		GUILayout.BeginVertical("box");
		for(int i = 0; i < available_rooms.Count; i++) {
			if(GUILayout.Button(((HostData)available_rooms[i]).gameName)) {
				Network.Connect(((HostData)available_rooms[i]).ip, ((HostData)available_rooms[i]).port);
				state = LOBBY_NETWORK;
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
//		label.x = initial_box.x + w*0.03f;
//		label.y = initial_box.y + h*0.33f;
//		GUI.Label(label, "IP");
//		ip_field.x = initial_box.x + 25;
//		ip_field.y = initial_box.y + h*0.33f;
//		ip = GUI.TextField(ip_field, ip);
//		label.x = initial_box.x + w*0.03f;
//		label.y = initial_box.y + h*0.33f*1.5f;
//		GUI.Label(label, "Port");
//		port_field.x = initial_box.x + 35;
//		port_field.y = initial_box.y + h*0.33f*1.5f;
//		port = GUI.TextField(port_field, port);
//		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
//							   initial_box.y + h*0.71f,
//							   85,
//						       25),
//		              "Back")) {
//			port = "";
//			ip = "";
//			connect_pressed = false;
//			state = INITIAL_SCREEN;
//		}
//		if(GUI.Button(new Rect(initial_box.x + w*0.9f - 80, 
//							   initial_box.y + h*0.71f,
//							   85,
//						       25),
//		              "Join")) {
//			if(ip == "" || port == ""){
//				connect_pressed = true;
//			} else {
//				Network.Connect(ip, int.Parse(port));
//				state = LOBBY_NETWORK;
//			}
//		}
//		if(connect_pressed){
//			label.x = initial_box.x + w*0.2f;
//			label.y = initial_box.y + h*0.6f;
//			GUI.Label(label, "Insert a valid ip address and port");
//		}
	}
	
	void LobbyNetwork(Vector2 team_0, Vector2 team_1, Vector2 team_2)
	{
		int num_players;
		if(Network.isServer)
			num_players = server_players.Count;
		else
			num_players = players.Count;
		
		for(int i = 0; i < num_players; i++){
			Player player;
			if(Network.isServer)
				player = server_players[i].player;
			else
				player = players[i];
			
			Vector2 position = new Vector2(0,0);
			
			switch(player.team){
			case 0:
				position = team_0;
				team_0.y += 35;
				break;
			case 1:
				position = team_1;
				team_1.y += 35;
				break;
			case 2:
				position = team_2;
				team_2.y += 35;
				break;
			}
			
			GUI.Box(new Rect(position.x - 30, position.y, 60, 30), player.name);
			
			if(Network.isServer) {
				if(player.team != 1) {
					if(GUI.Button(new Rect(position.x - 55, position.y + 5, 20, 20), "<")) {
						if (player.team == 0) {
							ChangeTeamPlayer(NETWORK, i, 1);
						} else {
							ChangeTeamPlayer(NETWORK, i, 0);
						}
					}
				}
				
				if(player.team != 2){
					if(GUI.Button(new Rect(position.x + 35, position.y + 5, 20, 20), ">")) {
						if (player.team == 0) {
							ChangeTeamPlayer(NETWORK, i, 2);
						} else {
							ChangeTeamPlayer(NETWORK, i, 0);
						}
					}
				}
			}
		}
		
		if(Network.isServer) {
			if(GUI.Button(new Rect(Screen.width/2 - 85, 6*Screen.height/7, 80, 25), "Start")){
				StartNetworkGame();
			}
			if(GUI.Button(new Rect(Screen.width/2 + 5, 6*Screen.height/7, 80, 25), "Disconnect")){
				Network.Disconnect();
				state = INITIAL_SCREEN;
			}
		} else {
			if(GUI.Button(new Rect(Screen.width/2 - 35, 6*Screen.height/7, 70, 25), "Disconnect")){
				Network.Disconnect();
			}
		}
	}
	
	void ChangePlayerName(string new_name, int i)
	{
		Player_Local temp_local_player = local_players[i];
		temp_local_player.player.name = new_name;
		local_players[i] = temp_local_player;
	}
	
	void LobbyLocal(Vector2 team_0, Vector2 team_1, Vector2 team_2)
	{
		/********************** For the keyboard player **********************/
		
		Player_Local local_player = local_players[0];
		Vector2 position = new Vector2(0,0);
			
		switch(local_player.player.team){
		case 0:
			position = team_0;
			team_0.y += 35;
			break;
		case 1:
			position = team_1;
			team_1.y += 35;
			break;
		case 2:
			position = team_2;
			team_2.y += 35;
			break;
		}
		
		string new_name = GUI.TextField(new Rect(position.x - 30, position.y, 60, 30), local_player.player.name, 7);
		
		ChangePlayerName(new_name, 0);
		
		if(local_player.player.team != 1) {
			if(GUI.Button(new Rect(position.x - 55, position.y + 5, 20, 20), "<")) {
				if (local_player.player.team == 0) {
					ChangeTeamPlayer(LOCAL, 0, 1);
				} else {
					ChangeTeamPlayer(LOCAL, 0, 0);
				}
			}
		}
		
		if(local_player.player.team != 2){
			if(GUI.Button(new Rect(position.x + 35, position.y + 5, 20, 20), ">")) {
				if (local_player.player.team == 0) {
					ChangeTeamPlayer(LOCAL, 0, 2);
				} else {
					ChangeTeamPlayer(LOCAL, 0, 0);
				}
			}
		}
		
		/******************************************************************/
		
		for(int i = 1; i < local_players.Count; i++){
			local_player = local_players[i];
			position = new Vector2(0,0);
				
			switch(local_player.player.team){
			case 0:
				position = team_0;
				team_0.y += 35;
				break;
			case 1:
				position = team_1;
				team_1.y += 35;
				break;
			case 2:
				position = team_2;
				team_2.y += 35;
				break;
			}
			
			new_name = GUI.TextField(new Rect(position.x - 30, position.y, 60, 30), local_player.player.name, 7);
			
			ChangePlayerName(new_name, i);
				
			if(local_player.player.team != 1) {
				if(GUI.Button(new Rect(position.x - 55, position.y + 5, 20, 20), "<")) {
					if (local_player.player.team == 0) {
						ChangeTeamPlayer(LOCAL, i, 1);
					} else {
						ChangeTeamPlayer(LOCAL, i, 0);
					}
				}
			}
			
			if(local_player.player.team != 2){
				if(GUI.Button(new Rect(position.x + 35, position.y + 5, 20, 20), ">")) {
					if (local_player.player.team == 0) {
						ChangeTeamPlayer(LOCAL, i, 2);
					} else {
						ChangeTeamPlayer(LOCAL, i, 0);
					}
				}
			}
		}
		
		if(local_players.Count < Input.GetJoystickNames().Length + 1) {
			for(int i = local_players.Count, j = local_players.Count - 1; i < Input.GetJoystickNames().Length + 1; i++, j++) {
				Player_Local player_local = new Player_Local();
				player_local.controller = i;
				player_local.player.name = (j + 1).ToString();
				player_local.player.team = 0;
				
				local_players.Add(player_local);
			}
		} else if(local_players.Count > Input.GetJoystickNames().Length + 1) {
			for(int i = 1, j = 0; i < Input.GetJoystickNames().Length; i++, j++) {
				Player_Local player_local = new Player_Local();
				player_local.controller = i;
				player_local.player.name = (j + 1).ToString();
				player_local.player.team = 0;
				
				local_players[i] = player_local;
			}
			for(int i = Input.GetJoystickNames().Length + 1; i < local_players.Count; i++)
				local_players.RemoveAt(i);
		}

		if(GUI.Button(new Rect(Screen.width/2 - 85, 6*Screen.height/7, 80, 25), "Start")){
			StartLocalGame();
		}
		if(GUI.Button(new Rect(Screen.width/2 + 5, 6*Screen.height/7, 80, 25), "Back")){
			state = INITIAL_SCREEN;
		}
	}

	void StartLocalGame ()
	{
		int players_team_0 = 0;
		int players_team_1 = 0;
		int players_team_2 = 0;
		
		for (int i = 0; i < local_players.Count; i++) {
			if(local_players[i].player.team == 0)
				players_team_0++;
			else if (local_players[i].player.team == 1)
				players_team_1++;
			else
				players_team_2++;
		}
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
		GameObject settings = (GameObject)Instantiate(settings_prefab);
		Game_Settings game_settings = settings.GetComponent<Game_Settings>();
	
		for(int i = 0; i < local_players.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			Player player = local_players[i].player;
			
			if(player.team == 1) {
				start_position = court_start_position_team_1.transform.position;
				Debug.Log(start_position);
				start_position.x = start_position.x + distance_team_1*players_team_1;
				Debug.Log(distance_team_1*players_team_1);
				Debug.Log(start_position);
				players_team_1--;
			} else if(player.team == 2) {
				start_position = court_start_position_team_2.transform.position;
				Debug.Log(start_position);
				start_position.x = start_position.x + distance_team_2*players_team_2;
				Debug.Log(distance_team_2*players_team_2);
				Debug.Log(start_position);
				players_team_2--;
			}
			Debug.Log("---------------------------" + player.name);
			game_settings.AddLocalPlayer(player.team, player.name, start_position, local_players[i].controller);
		}
		game_settings.local_game = true;
		Application.LoadLevel("Main_Game");
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
        networkView.RPC("AskName", player);
    }
	
	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		for(int i = 0; i < server_players.Count; i++) 
		{
			if(player == server_players[i].network_player)
				server_players.RemoveAt(i);
		}
		
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		string data = SerializePlayers();
		networkView.RPC("UpdatePlayers", RPCMode.Others, data);
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if(Network.isClient){
			Network.RemoveRPCs(Network.player);
			Network.DestroyPlayerObjects(Network.player);
			state = INITIAL_SCREEN;
		}
	}
	
	string SerializePlayers()
	{
		MemoryStream o = new MemoryStream(); //Create something to hold the data
 
    	BinaryFormatter bf = new BinaryFormatter(); //Create a formatter
		
		List<Player> new_players_list = new List<Player>();
		for(int i = 0; i < server_players.Count; i++)
			new_players_list.Add(server_players[i].player);
		
    	bf.Serialize(o, new_players_list); //Save the list
    	return Convert.ToBase64String(o.GetBuffer()); //Convert the data to a string
	}

	List<Player> DeserializePlayers (string data)
	{
		//Reading it back in
	   	MemoryStream ins = new MemoryStream(Convert.FromBase64String(data)); //Create an input stream from the string
	   	//Read back the data
		BinaryFormatter bf = new BinaryFormatter();
		return (List<Player>)bf.Deserialize(ins);
	}
	
	void ChangeTeamPlayer(int type, int i, int team) 
	{
		Player player;
		
		if(type == NETWORK){
			player = server_players[i].player;
		} else {
			player = local_players[i].player;
		}
		player.team = team;
		
		if(type == NETWORK) {
			ServerPlayer server_player = server_players[i];
			server_player.player = player;
			server_players[i] = server_player;
			string data = SerializePlayers();
			networkView.RPC("UpdatePlayers", RPCMode.Others, data);
		} else {
			Player_Local local_player = local_players[i];
			local_player.player = player;
			local_players[i] = local_player;
		}
	}
	
	
	[RPC]
	void AskName()
	{
		networkView.RPC("TellName", RPCMode.Server, player_name, Network.player);
	}
	
	[RPC]
	void TellName(string new_player_name, NetworkPlayer network_player)
	{
		AddPlayer(network_player, new_player_name);
		string data = SerializePlayers();
		
		networkView.RPC("UpdatePlayers", RPCMode.Others, data);
	}
	
	[RPC]
	void UpdatePlayers(string data)
	{
		List<Player> new_players = DeserializePlayers (data);
		players = new_players;
	}
	
	void InitialScreen()
	{
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 30, 100, 25), "Local Game")){
			if(local_players.Count != 0)
				local_players = new List<Player_Local>();
			
			Player_Local player_local = new Player_Local();
			player_local.controller = 0;
			player_local.player.team = 0;
			player_local.player.name = "0";
			
			local_players.Add(player_local);
			
			state = LOBBY_LOCAL;
		}
		if(GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 + 30, 100, 25), "Network Game")){
			state = NETWORK_GAME;
		}
	}
	
	void OnGUI()
	{
		if(state != LOBBY_NETWORK && state != LOBBY_LOCAL && state != JOIN_SERVER) {
			GUI.Box(initial_box, "Welcome");
			
			switch(state) {
				case INITIAL_SCREEN:
					InitialScreen();
					break;
				case NETWORK_GAME:
					NetworkGame();
					break;
				case CREATE_SERVER:
					CreateServer ();
					break;
				
			}
			
		} else if (state == JOIN_SERVER) {
			JoinServer();
		} else {
			
			GUI.Box(new Rect(10, 10, Screen.width-20, Screen.height-20), "Lobby"); 
			GUI.Box(new Rect(Screen.width/4 - Screen.width/6, 40, Screen.width/4, Screen.height-60), "Team 1");
			GUI.Box(new Rect(3*Screen.width/4 - Screen.width/12, 40, Screen.width/4, Screen.height-60), "Team 2");
			
			Vector2 team_0 = new Vector2(Screen.width/2,Screen.height/6);
			Vector2 team_1 = new Vector2(Screen.width/6 + Screen.width/25,Screen.height/6);
			Vector2 team_2 = new Vector2(5*Screen.width/6 - Screen.width/25,Screen.height/6);
			
			switch(state) {
				case LOBBY_NETWORK:
					LobbyNetwork(team_0, team_1, team_2);
					break;
				case LOBBY_LOCAL:
					LobbyLocal(team_0, team_1, team_2);
					break;
			}
		}
	}
	
	void StartNetworkGame()
	{
		int players_team_0 = 0;
		int players_team_1 = 0;
		int players_team_2 = 0;
		
		for (int i = 0; i < server_players.Count; i++) {
			if(server_players[i].player.team == 0)
				players_team_0++;
			else if (server_players[i].player.team == 1)
				players_team_1++;
			else
				players_team_2++;
		}
		
		float court_lenght = court_start_position_team_1.transform.position.x*(-2);
		float distance_team_1 = court_lenght/(players_team_1+1);
		float distance_team_2 = court_lenght/(players_team_2+1);
		
		networkView.RPC("LoadSettings", RPCMode.Others);
		
		GameObject settings = (GameObject)Instantiate(settings_prefab);
		Game_Settings game_settings = settings.GetComponent<Game_Settings>();
	
		for(int i = 0; i < server_players.Count; i++) {
			Vector3 start_position = new Vector3(0,0,0);
			Player player = server_players[i].player;
			
			if(player.team == 1) {
				start_position = court_start_position_team_1.transform.position;
				Debug.Log(start_position);
				start_position.x = start_position.x + distance_team_1*players_team_1;
				Debug.Log(distance_team_1*players_team_1);
				Debug.Log(start_position);
				players_team_1--;
			} else if(player.team == 2) {
				start_position = court_start_position_team_2.transform.position;
				Debug.Log(start_position);
				start_position.x = start_position.x + distance_team_2*players_team_2;
				Debug.Log(distance_team_2*players_team_2);
				Debug.Log(start_position);
				players_team_2--;
			}
			
			game_settings.AddNetworkPlayer(player.team, player.name, start_position, server_players[i].network_player);
		}
		game_settings.local_game = false;
		networkView.RPC("LoadGame", RPCMode.All);
	}
	
	[RPC]
	void LoadSettings()
	{
		GameObject settings = (GameObject)Instantiate(settings_prefab);
		Game_Settings game_settings = settings.GetComponent<Game_Settings>();
		game_settings.local_game = false;
	}
	
	[RPC]
	void LoadGame()
	{
		Application.LoadLevel("Main_Game");
	}
}
