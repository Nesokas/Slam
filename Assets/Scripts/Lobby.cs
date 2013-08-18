using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Lobby : MonoBehaviour {

	private const int INITIAL_SCREEN = 1;
	private const int CREATE_SERVER = 2;
	private const int JOIN_SERVER = 3;
	private const int LOBBY = 4;
	
	private string player_name = "";
	private string ip = "127.0.0.1";
	private string port = "8000";
		
	private int state;
	
	private Rect initial_box;
	private Rect nickname_field;
	private Rect ip_field;
	private Rect port_field;
	private Rect label;
	
	private GUIStyle label_style;
	
	private float w;
	private float h;
	
	private bool connect_pressed = false;
	
	private int buttons_width = 90;
	private int buttons_height = 25;
	
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
	
	private List<Player> players;
	private List<ServerPlayer> server_players;
	
	// Use this for initialization
	void Start () 
	{		
		state = INITIAL_SCREEN;
		players = new List<Player>();
		server_players = new List<ServerPlayer>();
		
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

	void InitialScreen ()
	{
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f;
		GUI.Label(label, "Nickname");
		nickname_field.x = initial_box.x + 70;
		nickname_field.y = initial_box.y + h*0.33f;
		player_name = GUI.TextField(nickname_field, player_name);
		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "New Server")) {
			state = CREATE_SERVER;
		}
		if(GUI.Button(new Rect(initial_box.x + w*0.9f - buttons_width, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "Connect")) {
			state = JOIN_SERVER;
		}
	}

	void CreateServer ()
	{
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f;
		GUI.Label(label, "Port");
		port_field.x = initial_box.x + 35f;
		port_field.y = initial_box.y + h*0.33f;
		port = GUI.TextField(port_field, port);
		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "Back")) {
			port = "";
			connect_pressed = false;
			state = INITIAL_SCREEN;
		}
		if(GUI.Button(new Rect(initial_box.x + w*0.9f - buttons_width, 
							   initial_box.y + h*0.71f,
							   buttons_width,
						       buttons_height),
		              "Create")) {
			if(port == ""){
				connect_pressed = true;
			} else {
				Network.InitializeServer(32, int.Parse(port),false);
				state = LOBBY;
				AddPlayer(Network.player, player_name);
			}
		}
		if(connect_pressed){
			label.x = initial_box.x + w*0.33f;
			label.y = initial_box.y + h*0.6f;
			GUI.Label(label, "Insert a valid port");
		}
	}

	void JoinServer ()
	{
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f;
		GUI.Label(label, "IP");
		ip_field.x = initial_box.x + 25;
		ip_field.y = initial_box.y + h*0.33f;
		ip = GUI.TextField(ip_field, ip);
		label.x = initial_box.x + w*0.03f;
		label.y = initial_box.y + h*0.33f*1.5f;
		GUI.Label(label, "Port");
		port_field.x = initial_box.x + 35;
		port_field.y = initial_box.y + h*0.33f*1.5f;
		port = GUI.TextField(port_field, port);
		if(GUI.Button(new Rect(initial_box.x + w*0.1f, 
							   initial_box.y + h*0.71f,
							   85,
						       25),
		              "Back")) {
			port = "";
			ip = "";
			connect_pressed = false;
			state = INITIAL_SCREEN;
		}
		if(GUI.Button(new Rect(initial_box.x + w*0.9f - 80, 
							   initial_box.y + h*0.71f,
							   85,
						       25),
		              "Join")) {
			if(ip == "" || port == ""){
				connect_pressed = true;
			} else {
				Network.Connect(ip, int.Parse(port));
				state = LOBBY;
			}
		}
		if(connect_pressed){
			label.x = initial_box.x + w*0.2f;
			label.y = initial_box.y + h*0.6f;
			GUI.Label(label, "Insert a valid ip address and port");
		}
	}
	
	void LobbyScreen()
	{
		GUI.Box(new Rect(10, 10, Screen.width-20, Screen.height-20), "Lobby"); 
		GUI.Box(new Rect(Screen.width/4 - Screen.width/6, 40, Screen.width/4, Screen.height-60), "Team 1");
		GUI.Box(new Rect(3*Screen.width/4 - Screen.width/12, 40, Screen.width/4, Screen.height-60), "Team 2");
		
		Vector2 team_0 = new Vector2(Screen.width/2,Screen.height/6);
		Vector2 team_1 = new Vector2(Screen.width/6 + Screen.width/25,Screen.height/6);
		Vector2 team_2 = new Vector2(5*Screen.width/6 - Screen.width/25,Screen.height/6);
		
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
							ChangeTeamPlayer(i, 1);
						} else {
							ChangeTeamPlayer(i, 0);
						}
					}
				}
				
				if(player.team != 2){
					if(GUI.Button(new Rect(position.x + 35, position.y + 5, 20, 20), ">")) {
						if (player.team == 0) {
							ChangeTeamPlayer(i, 2);
						} else {
							ChangeTeamPlayer(i, 0);
						}
					}
				}
			}
		}
		
		if(Network.isServer) {
			if(GUI.Button(new Rect(Screen.width/2 - 85, 6*Screen.height/7, 80, 25), "Start")){
				StartGame();
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
	
	void ChangeTeamPlayer(int i, int team) 
	{
		Player player = server_players[i].player;
		player.team = team;
		
		ServerPlayer server_player = server_players[i];
		server_player.player = player;
		server_players[i] = server_player;
		string data = SerializePlayers();
		networkView.RPC("UpdatePlayers", RPCMode.Others, data);
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
	
	void OnGUI()
	{
		if(state != LOBBY) {
			GUI.Box(initial_box, "Welcome");
			
			switch(state)
			{
			case INITIAL_SCREEN:
				InitialScreen ();
				break;
			case CREATE_SERVER:
				CreateServer ();
				break;
			case JOIN_SERVER:
				JoinServer ();
				break;
			}
		} else {
			LobbyScreen();
		}
	}
	
	void StartGame()
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
			
			game_settings.AddPlayer(player.team, player.name, start_position, server_players[i].network_player);
		}
		networkView.RPC("LoadGame", RPCMode.All);
	}
	
	[RPC]
	void LoadGame()
	{
		Application.LoadLevel("Main_Game");
	}
}
