using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Lobby : MonoBehaviour {

	/*********** CONSTANTS ******************/
	
	protected const int JOIN_TAB = 0;
	protected const int CREATE_TAB = 1;
	protected const int FAVOURITES_TAB = 2;
	protected const int SETTINGS_TAB = 3;
	
	protected const int NICKNAME_SCREEN = 0;
	protected const int MAIN_MENU = 1;
	protected const int LOBBY = 2;
	
	protected const int SPECTATING = 0;
	protected const int TEAM_1 = 1;
	protected const int TEAM_2 = 2;

	/****************************************/

	protected struct Player
	{
		public string name;
		public int team;
		public NetworkPlayer network_player;
		public int controller;
		public bool is_network;
		
		public int hero;
		public bool ready;
		public GameObject hero_choosen;
	}

	protected int team_1_color;
	protected int team_2_color;

	protected List<Player> spectating;
	protected List<Player> team_1;
	protected List<Player> team_2;

	protected Game_Settings game_settings;
	public GameObject settings_prefab;

	protected Player self_player;
	public GUISkin gui_skin;

	public string[] team_colors = new string[] {"Red", "Blue", "Green"};

	protected bool show_lobby;
	protected bool show_lobby_arrows;

	protected void Awake()
	{
		team_1_color = 0;
		team_2_color = 1;
		
		spectating = new List<Player>();
		team_1 = new List<Player>();
		team_2 = new List<Player>();

		show_lobby = true;
	}

	protected void Start()
	{
		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		if(settings != null) {
			game_settings = settings.GetComponent<Game_Settings>();
			game_settings.local_game = game_settings.IsLocalGame();
		}
	}

	Player CreatePlayer(string player_name, int team)
	{		
		Player player = new Player();
		player.name = player_name;
		player.team = team;
		return player;
	}
	
	[RPC]
	protected void AddNetworkPlayer (NetworkPlayer network_player, string player_name, int team = SPECTATING)
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
		
		if(network_player == Network.player)
			self_player = player;
	}
	
	protected void AddLocalPlayer(int controller, string player_name, int team=0) 
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

	void DrawPlayers(List<Player> players, int team)
	{
		for(int i = 0; i < players.Count; i++) {
			bool change_team = false;
			int new_team = SPECTATING;
			
			GUILayout.BeginHorizontal();
				if(show_lobby_arrows && (team == TEAM_2 || team == SPECTATING) && (game_settings.IsLocalGame() || Network.isServer)) {
					if(GUILayout.Button("<", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_1;
						change_team = true;
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.Label(players[i].name);
				if(!game_settings.IsLocalGame() && players[i].network_player != Network.player) {
					GUILayout.Label("" + Network.GetAveragePing(players[i].network_player));
				}
				GUILayout.FlexibleSpace();
				if(show_lobby_arrows && (team == TEAM_1 || team == SPECTATING) && (game_settings.IsLocalGame() || Network.isServer)) {
					if(GUILayout.Button(">", GUILayout.MaxWidth(0.03f*Screen.width))) {
						if(team == SPECTATING)
							new_team = TEAM_2;
						change_team = true;
					}
				}
			GUILayout.EndHorizontal();

			if(change_team) {
				if(game_settings.IsLocalGame())
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

	protected abstract void LobbyStates();

	void OnGUI()
	{	
		if(show_lobby) {
			GUI.skin = gui_skin;
			GUILayout.BeginArea(new Rect(Screen.width*0.01f, Screen.height*0.01f, Screen.width - Screen.width*0.02f, Screen.height - Screen.height*0.02f));
			LobbyStates();
			GUILayout.EndArea();
		}
	}

	protected void LobbyScreen()
	{
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
				GUILayout.BeginVertical(GUILayout.MinWidth(0.3f*Screen.width));
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Red");
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
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
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Blue");
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
						DrawPlayers(team_2, TEAM_2);
					GUILayout.EndVertical();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				LobbyMenu();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	protected abstract void LobbyMenu();

	protected void BackToMainMenu()
	{
		Application.LoadLevel(game_settings.main_menu_scene);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Application.LoadLevel(game_settings.main_menu_scene);
	}
}
