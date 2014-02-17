using UnityEngine;
using System.Collections;

public class GameLobby : Lobby {

	protected void Awake() {
		base.Awake();
		show_lobby_arrows = false;
		show_lobby = false;
	}

	protected void Start()
	{
		base.Start();
		PopulateLists();
	}

	void PopulateLists()
	{
		if(game_settings.players_list != null) {
			for(int i = 0; i < game_settings.players_list.Count; i++){
				Hero_Selection.Player player = game_settings.players_list[i];
				if(game_settings.IsLocalGame()){
					AddLocalPlayer(player.controller, player.player_name, player.team);
				} else {
					AddNetworkPlayer(player.network_player, player.player_name, player.team);
				}
			}
		}
	}

	protected override void LobbyMenu()
	{
		if(!game_settings.IsLocalGame() && GUILayout.Button("Disconnect", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))){
			bool is_server = Network.isServer;
			Network.Disconnect();
			if(is_server)
				MasterServer.UnregisterHost();
			BackToMainMenu();
		}
		if (game_settings.IsLocalGame()) {
			if(GUILayout.Button("Restart and Refresh", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))) {
				game_settings.players_list.Clear();
				Application.LoadLevel("Pre_Game_Lobby");
			}
			GUILayout.FlexibleSpace();
		}

		if(GUILayout.Button("Exit", GUILayout.MinWidth(0.15f*Screen.width), GUILayout.MinHeight(0.06f*Screen.height))) {
			Application.Quit();
		}
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) {
			show_lobby = !show_lobby;
		}
	}

	protected override void LobbyStates()
	{
		LobbyScreen();
	}
}
