using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {
	
	private bool game_restarted = true;
	
	private GameObject last_player_touched;
	
	public void GameHasRestarted()
	{
		game_restarted = true;
	}
	
	public void OnCollisionExit(Collision collider)
	{
		last_player_touched = collider.gameObject;
	}
	
	void OnCollisionEnter(Collision collider)
	{
		ReleasePlayers();
	}
	
	public GameObject GetLastPlayerTouched()
	{
		return last_player_touched;
	}
	
	public void ReleasePlayers()
	{
		if (Application.loadedLevelName == "Main_Game" && game_restarted)
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
		if(Application.loadedLevelName == "Main_Game") {
			GameObject[] center_planes = GameObject.FindGameObjectsWithTag("center-plane");
			GameObject center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
			GameObject center_circle_rigth = GameObject.FindGameObjectWithTag("center-circle-right");
			
			for(int i = 0; i < center_planes.Length; i++)
				Physics.IgnoreCollision(center_planes[i].collider, transform.collider);
			Physics.IgnoreCollision(center_circle_left.collider, transform.collider);
			Physics.IgnoreCollision(center_circle_rigth.collider, transform.collider);
		}
		transform.animation.Stop();
	}
	
	void Update()
	{
		if(!transform.animation.isPlaying && Random.Range(0,100) == 0){
			int random = Random.Range(0,100);
			if(random <= 75)
				transform.animation.Play("Blink");
			else
				transform.animation.Play("Tired");
		}
	}
}
