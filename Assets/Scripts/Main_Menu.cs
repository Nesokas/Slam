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
	private const int STANDARD_MAX_CHARS = 20;
//	
//	private const int SPECTATING = 0;
//	private const int TEAM_1 = 1;
//	private const int TEAM_2 = 2;
	
	private const string GAME_TYPE = "Default";

	private const int BUTTON_SIDE_SIZE = 100;
	
	/****************************************/
	
	public GameObject settings_prefab;
	private GameObject settings;
	private Game_Settings game_settings;
	
//	public string[] tabs = new string[] {"Join", "Create", "Favorites", "Settings"};
	public string[] tabs = new string[] {"Join", "Create"};
	public string[] sortable_columns = new string[] {"Room Name", "Players", "Ping", "Country"};
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

	public GUIStyle main_menu_style;
	public Texture background_texture;
	public GUISkin gui_skin;
	
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

	private void DrawRoomListTableHeader()
	{

			toogle_columns[0] = GUILayout.Toggle(toogle_columns[0], sortable_columns[0], GUILayout.MinWidth(Screen.width*0.15f));
			for(int i = 1; i < sortable_columns.Length; i++)
				toogle_columns[i] = GUILayout.Toggle(toogle_columns[i], sortable_columns[i], GUILayout.MaxWidth(Screen.width*0.07f));

	}

	private void DrawRoomListTableFields()
	{
		rooms_scroll_position = GUILayout.BeginScrollView(rooms_scroll_position);

			GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true));
				GUILayout.BeginVertical();
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
	}

	void JoinRoom()
	{
		GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal(GUILayout.Width(Screen.width*0.85f));
				DrawRoomListTableHeader();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					DrawRoomListTableFields();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	void CreateRoom()
	{
		GUILayout.BeginHorizontal("box", GUILayout.ExpandHeight(true));
			GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
				GUILayout.BeginHorizontal();
				GUILayout.Label("Offline Game", GUILayout.Width(Screen.width*0.2f));
					offline_game = GUILayout.Toggle(offline_game, "");
				GUILayout.EndHorizontal();
				if (!offline_game) {
					GUILayout.BeginHorizontal();
						GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
							GUILayout.Label("Room Name:", GUILayout.Width(Screen.width*0.2f));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							GUIStyle myStyle = new GUIStyle(GUI.skin.textField);
							myStyle.alignment = TextAnchor.MiddleLeft;
							room_name = GUILayout.TextField(room_name, STANDARD_MAX_CHARS, myStyle, GUILayout.Width(0.14f*Screen.width), GUILayout.Height(0.05f*Screen.height));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							GUILayout.Label("Password:", GUILayout.MaxWidth(Screen.width*0.2f));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							password = GUILayout.PasswordField(password, '*', STANDARD_MAX_CHARS, myStyle, GUILayout.MinWidth(0.14f*Screen.width), GUILayout.Height(0.05f*Screen.height));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					for(int i = 0; i < 100; i++)
						GUILayout.FlexibleSpace();
				} else {
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				}
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				if(GUILayout.Button("Create", GUILayout.Width(BUTTON_SIDE_SIZE), GUILayout.Height(50f))){
						game_settings.local_game = true;
					Application.LoadLevel("Pre_Game_Lobby");
				}
				GUILayout.FlexibleSpace();
		if(GUILayout.Button("Back", GUILayout.Width(BUTTON_SIDE_SIZE), GUILayout.MinHeight(0.05f*Screen.height)))
					menu_state = NICKNAME_SCREEN;
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	void NicknameScreen()
	{
		GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical("box", GUILayout.MaxHeight(0.3f*Screen.height), GUILayout.MaxWidth(0.4f*Screen.width));
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							GUILayout.Label("Nickname:", GUILayout.Width(90));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							GUIStyle myStyle = new GUIStyle(GUI.skin.textField);
							myStyle.alignment = TextAnchor.MiddleLeft;
							game_settings.player_name = GUILayout.TextField(game_settings.player_name,
			                                                STANDARD_MAX_CHARS, 
		                                                	myStyle,
			                                                GUILayout.MinWidth(0.2f*Screen.width), 
			                                                GUILayout.MinHeight(0.05f*Screen.height));
							GUILayout.FlexibleSpace();
						GUILayout.EndVertical();
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("Exit", GUILayout.MinWidth(0.1f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height)))
							Application.Quit();
						GUILayout.FlexibleSpace();
						if(GUILayout.Button("Start", GUILayout.MinWidth(0.1f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height)))
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
		GUILayout.BeginArea(new Rect(0,0, Screen.width*0.98f, Screen.height - Screen.height*0.02f));
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					// Draw tabs
					tab_selected = GUILayout.Toolbar(tab_selected, tabs, GUILayout.MinHeight(0.05f*Screen.height));
					
					switch(tab_selected){
					case JOIN_TAB:
						JoinRoom();
						break;
					case CREATE_TAB:
						CreateRoom();
						break;
					}
		
					//Version
//					GUILayout.BeginHorizontal();
//						if(GUILayout.Button("Back"))
//							menu_state = NICKNAME_SCREEN;
//						GUILayout.FlexibleSpace();
//						GUILayout.Label("Version 1.0; " + total_players_connected + " online");
//					GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void OnGUI()
	{	
		GUI.skin = gui_skin;
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
