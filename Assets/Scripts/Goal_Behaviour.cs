using UnityEngine;
using System.Collections;

public class Goal_Behaviour : MonoBehaviour {
	
	public int team = 0;
	//public GameObject game_behaviour;
	public AudioClip goal_sound;
	
	private bool celebrating;
	
	void Start()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "StopCelebration");
		celebrating = false;
	}
	
	void StopCelebration()
	{
		celebrating = false;
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "ball") {
			if(Application.loadedLevelName == "Main_Game") {
				Hashtable data = new Hashtable();
				if(team == 1)
					data["team"] = 2;
				else
					data["team"] = 1;
				//Game_Behaviour game_manager = game_behaviour.GetComponent<Game_Behaviour>();
				if(!celebrating) {
					celebrating = true;
					NotificationCenter.DefaultCenter.PostNotification(this, "OnGoal", data);
				}
			}
		}
	}
	
}
