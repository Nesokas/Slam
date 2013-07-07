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
				//Screen_Text_Behaviour stb = screen_text.GetComponent<Screen_Text_Behaviour>();
				Game_Behaviour game_manager = game_behaviour.GetComponent<Game_Behaviour>();
				if(team == 1) {
					//stb.TeamScored(2);
				//	game_manager.TeamScored(2);
					NotificationCenter.DefaultCenter.PostNotification(this, "OnGoal", data);
				}
				else  {
					//stb.TeamScored(1);
			//		game_manager.TeamScored(1);
					NotificationCenter.DefaultCenter.PostNotification(this, "OnGoal", data);
				}
				AudioSource.PlayClipAtPoint(goal_sound, Vector3.zero);
			}
		}
	}
	
}
