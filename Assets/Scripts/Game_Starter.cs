using UnityEngine;
using System.Collections;

public class Game_Starter : MonoBehaviour {
	
	public GameObject local_game_prefab;
	public GameObject network_game_prefab;

	// Use this for initialization
	void Start () {
			
		GameObject settings =  GameObject.FindGameObjectWithTag("settings");
		if (settings != null) {
			Game_Settings game_settings = settings.GetComponent<Game_Settings>();
			
			if(game_settings.IsLocalGame()) {
				Instantiate(local_game_prefab, Vector3.zero, transform.rotation);
			} else {
				Instantiate(network_game_prefab, Vector3.zero, transform.rotation);
			}
		} else {
			Instantiate(local_game_prefab, Vector3.zero, transform.rotation);
		}
	}
}