using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {

	void Start () 
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
	
		for(int i = 0; i < players.Length; i++) {
			Transform player_base = players[i].transform.Find("Base");
			Transform player_shoot_colider = player_base.transform.Find("ColliderShoot");
			Physics.IgnoreCollision(player_shoot_colider.collider, transform.collider);
		}
		
		GameObject[] goals = GameObject.FindGameObjectsWithTag("goal_detection");
		for(int i = 0; i < goals.Length; i++)
			Physics.IgnoreCollision(goals[i].transform.collider, transform.collider);
	}
}
