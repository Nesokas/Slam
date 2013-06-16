using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {
	
	private bool game_restarted = true;
	
	public void GameHasRestarted()
	{
		game_restarted = true;
	}
	
	void OnCollisionEnter(Collision collider)
	{
		ReleasePlayers();
	}
	
	public void ReleasePlayers()
	{
		if (game_restarted)
		{
			GameObject gbo = GameObject.FindGameObjectWithTag("GameController");
			Game_Behaviour gb = gbo.GetComponent<Game_Behaviour>();
			Debug.Log("Release Players");
			gb.ReleasePlayers();
			game_restarted = false;
		}
	}

	void Start () 
	{		
		GameObject[] center_planes = GameObject.FindGameObjectsWithTag("center-plane");
		GameObject center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
		GameObject center_circle_rigth = GameObject.FindGameObjectWithTag("center-circle-right");
		
		for(int i = 0; i < center_planes.Length; i++)
			Physics.IgnoreCollision(center_planes[i].collider, transform.collider);
		Physics.IgnoreCollision(center_circle_left.collider, transform.collider);
		Physics.IgnoreCollision(center_circle_rigth.collider, transform.collider);
	}
}
