using UnityEngine;
using System.Collections;

public class NetworkLoading : MonoBehaviour {

	int total_players;
	int total_load_complete = 0;
	bool is_loading = false;
	Game_Settings game_settings;

	void Awake(){
		DontDestroyOnLoad(this);
		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = settings.GetComponent<Game_Settings>();
	}

	public IEnumerator StartLoading()
	{
		Debug.Log("Start Loading");
		is_loading = true;

		total_players = game_settings.team_1_count + game_settings.team_2_count;

		AsyncOperation async = Application.LoadLevelAsync("Main_Game");
		yield return async;

		Debug.Log("Loading Complete");
		networkView.RPC("LoadingComplete", RPCMode.Server);
	}

	void LoadingComplete()
	{
		total_load_complete++;

		if(total_load_complete == total_players) {
			GameObject starter = GameObject.Find("GameStarter");
			GameStarter game_starter = starter.GetComponent<GameStarter>();

			is_loading = false;
			game_starter.StartNetworkGame();
		}
	}

	void OnGUI()
	{
		if(is_loading) {
			GUI.Label(new Rect(10, 10, 100, 20), "Loading...");
		}
	}
}
