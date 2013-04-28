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
	}
}
