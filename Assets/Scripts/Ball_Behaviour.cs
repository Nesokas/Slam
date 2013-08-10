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
		Debug.Log(collider.transform.tag);
		if (collider.transform.tag == "court_walls") {
			
			int random = Random.Range(0,100);
			if(random <= 10) {
				transform.animation["Rolling_Eyes"].wrapMode = WrapMode.Loop;
				StartCoroutine(LoopAnimation("Rolling_Eyes", 1));
			}
		}
	}
	
	IEnumerator LoopAnimation(string str, int repeatNumber)
	{
		transform.animation.CrossFade(str, 0.3f);
		Debug.Log(animation[str].length*repeatNumber);
		yield return new WaitForSeconds(animation[str].length*0.35f);
		animation.CrossFade("Tired", 1.5f);
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
		transform.animation["Rolling_Eyes"].speed = 7.5f;
	}
	
	void Update()
	{
		if(!transform.animation.isPlaying && Random.Range(0,100) == 0) {
			int random = Random.Range(0,100);
			if(random <= 85)
				transform.animation.Play("Blink");
			else
				transform.animation.Play("Tired");
		}
	}
}
