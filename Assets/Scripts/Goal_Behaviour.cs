using UnityEngine;
using System.Collections;

public class Goal_Behaviour : MonoBehaviour {
	
	public int team = 0;
	public GameObject screen_text;
	public AudioClip goal_sound;
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.tag == "ball") {
			if(Application.loadedLevelName == "Main_Game") {
				Screen_Text_Behaviour stb = screen_text.GetComponent<Screen_Text_Behaviour>();
				if(team == 1)
					stb.TeamScored(2);
				else 
					stb.TeamScored(1);
				AudioSource.PlayClipAtPoint(goal_sound, Vector3.zero);
			}
			NotificationCenter.DefaultCenter.PostNotification(this, "OnGoal");
		}
	}
	
}
