using UnityEngine;
using System.Collections;

public class Goal_Behaviour : MonoBehaviour {
	
	public int score = 0;
	public int team = 0;
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.name == "Ball"){
			score++;
			GameObject main_game_object = GameObject.FindWithTag("GameController");
			Game_Behaviour main_game_component = main_game_object.GetComponent<Game_Behaviour>();
			
			main_game_component.ScoreTeam(team);
		}
	}
}
