using UnityEngine;
using System.Collections;

public class Goal_Behaviour : MonoBehaviour {
	
	public int team = 0;
	public GameObject screen_text;
	public GameObject game_behaviour;
	public AudioClip goal_sound;
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "ball") {
			if(Application.loadedLevelName == "Main_Game") {
				Hashtable data = new Hashtable();
				data["team"] = team;
				Game_Behaviour game_manager = game_behaviour.GetComponent<Game_Behaviour>();
				NotificationCenter.DefaultCenter.PostNotification(this, "OnGoal", data);
			}
		}
	}
	
}
