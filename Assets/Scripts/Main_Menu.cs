using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

public class Main_Menu : MonoBehaviour 
{
	
	/*********** CONSTANTS ******************/
	
	private const int JOIN_TAB = 0;
	private const int CREATE_TAB = 1;
	private const int FAVOURITES_TAB = 2;
	private const int SETTINGS_TAB = 3;
	
	private const int NICKNAME_SCREEN = 0;
	private const int MAIN_MENU = 1;
	
	private const int SPECTATING = 0;
	private const int TEAM_1 = 1;
	private const int TEAM_2 = 2;
	
	private const string GAME_TYPE = "Default";
	
	/****************************************/
	
	public GameObject settings_prefab;
	private GameObject settings;
	private Game_Settings game_settings;
	
	public string[] tabs = new string[] {"Join", "Create", "Favorites", "Settings"};
	public string[] sortable_columns = new string[] {"Room Name", "Court", "Players", "Ping", "Country"};
	public string[] team_colors = new string[] {"Red", "Blue", "Green"};
	
	private int tab_selected;
	private int room_selected;
	private int total_players_connected;
	private int menu_state;
	private bool offline_game;
	
    private bool[] toogle_columns;
	private bool[] room_selection;
	
	private string room_name;
	private string password;
	private string search;
	
	private Vector2 rooms_scroll_position;
	
	private ArrayList available_rooms;
	
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
		search = "";
		
		menu_state = NICKNAME_SCREEN;
		
		toogle_columns = new bool[sortable_columns.Length];
		for(int i = 0; i < toogle_columns.Length; i++)
			toogle_columns[i] = false;
		
		InitializeMSF(); //MSF -> Master Server Facilitator

		if(GameObject.FindGameObjectWithTag("settings") == null) {
			settings = (GameObject)Instantiate(settings_prefab);
		} else {
			settings = GameObject.FindGameObjectWithTag("settings");
		}

		game_settings = settings.GetComponent<Game_Settings>();
		
		if(game_settings.player_name != "")
			menu_state = MAIN_MENU;
	}
	
	void Start()
	{

	}

	void JoinRoom()
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
			
				game_settings.connected = true;
				game_settings.connect_to = (HostData)available_rooms[room_selected];
				
				Application.LoadLevel("Main_Game");
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
					
					game_settings.local_game = false;
				} else {
					game_settings.local_game = true;
				}
				Application.LoadLevel("Main_Game");
			}
		GUILayout.EndVertical();
		
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
						game_settings.player_name = GUILayout.TextField(game_settings.player_name, GUILayout.MinWidth(0.2f*Screen.width));
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

	void MainMenuScreen ()
	{
		GUILayout.BeginArea(new Rect(0,0, Screen.width*0.7f, Screen.height - Screen.height*0.02f));
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					// Draw tabs
					tab_selected = GUILayout.Toolbar(tab_selected, tabs);
					
					switch(tab_selected){
					case JOIN_TAB:
						JoinRoom();
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
		}
		GUILayout.EndArea();
	}
	
}
