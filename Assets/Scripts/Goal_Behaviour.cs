using UnityEngine;
using System.Collections;

public class Goal_Behaviour : MonoBehaviour {
	
	public int team = 0;
	public GameObject screen_text;
	
	void OnCollisionEnter(Collision collision)
	{
		Screen_Text_Behaviour stb = screen_text.GetComponent<Screen_Text_Behaviour>();
		if(team == 1)
			stb.TeamScored(2);
		else stb.TeamScored(1);
	}
}
