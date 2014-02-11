using UnityEngine;
using System.Collections;

public class NetworkLoading : MonoBehaviour {

	int total_players;
	int total_load_complete = 0;
	bool is_loading = false;
	Game_Settings game_settings;
	PhotonView photonView;
	NetworkPreLoading network_pre_loading;

	void Awake(){
		DontDestroyOnLoad(this);
		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = settings.GetComponent<Game_Settings>();

		photonView = PhotonView.Get(this);
	}

	public IEnumerator StartLoading(NetworkPreLoading network_pre_loading)
	{
		is_loading = true;

		total_players = game_settings.team_1_count + game_settings.team_2_count;

		AsyncOperation async = Application.LoadLevelAsync("Main_Game");
		yield return async;

		this.network_pre_loading = network_pre_loading;
		photonView.RPC("LoadingComplete", game_settings.game_creator);

		if(!game_settings.is_game_creator) {
			GameObject loading_camera = GameObject.Find("NetworkLoadingCamera");

			Destroy(loading_camera);
			Destroy(network_pre_loading);
			Destroy(this);
		}
	}

	[RPC]
	void LoadingComplete()
	{
		total_load_complete++;

		if(total_load_complete == total_players) {
			GameObject starter = GameObject.Find("GameStarter");
			GameStarter game_starter = starter.GetComponent<GameStarter>();

			photonView.RPC("StopLoading", PhotonTargets.All);
			game_starter.StartNetworkGame();

			GameObject loading_camera = GameObject.Find("NetworkLoadingCamera");
			
			Destroy(loading_camera);
			Destroy(network_pre_loading);
			Destroy(this);
		}
	}

	[RPC]
	void StopLoading()
	{
		is_loading = false;
	}

	void OnGUI()
	{
		if(is_loading) {
			GUI.Label(new Rect(10, 10, 100, 20), "Loading...");
		}
	}
}
