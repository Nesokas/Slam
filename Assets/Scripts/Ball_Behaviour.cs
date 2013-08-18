using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {
	
	private bool game_restarted = true;
	
	private GameObject last_player_touched;
	
	private bool is_looking_somewhere;
	
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
				StartCoroutine(LoopAnimation("Rolling_Eyes", "Tired", 1));
			}
			else {
				animation.CrossFade("look_left", 0.5f);
//				animation.CrossFade("Default", 1.5f);
			}
		}
	}
	
	IEnumerator LoopAnimation(string anim1, string anim2, int repeatNumber)
	{
		transform.animation.CrossFade(anim1, 0.3f);
		Debug.Log(animation[anim1].length*repeatNumber);
		yield return new WaitForSeconds(animation[anim1].length*0.35f);
		animation.CrossFade(anim2, 1.5f);
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
		is_looking_somewhere = false;
		
		if (!networkView.isMine) {	
			enabled = false;
		}
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
		animation["Rolling_Eyes"].layer=1;
		animation["Blink"].AddMixingTransform( transform.Find("Armature_001") );
//		animation["Blink"].AddMixingTransform( transform.Find("Armature_001/Bone") );
//		animation["Blink"].AddMixingTransform( transform.Find("Bone") );
		animation["Blink"].layer = 1;
		animation["Tired"].layer = 1;
//		animation["look_left"].AddMixingTransform(transform.Find("Armature/Bone_004"));
//		animation["look_left"].wrapMode = WrapMode.ClampForever;
		animation["Default"].layer = 10;
//		animation["Default"].AddMixingTransform(transform.Find("Armature/Bone_004"));
	}
	
	void Update()
	{
//		if (is_looking_somewhere && Random.Range(0,50) == 0) {
//			animation.Blend("Default",1f, 0.3f);
//			is_looking_somewhere = false;
//		}
//		if(Random.Range(0,100) == 0) {
//			int random = Random.Range(0,100);
//			if(random <= 50) {
//				animation.Blend("Tired", 1f, 0.7f);
//				
//			} else if (random <= 100 && animation["look_left"].enabled == false) {
//				transform.animation.CrossFade("look_left", 0.2f);
//				is_looking_somewhere = true;
//			} else if (random <= 20) {
//				transform.animation.Play("look_right");
//				is_looking_somewhere = true;
//			}
//			else {
//				transform.animation.CrossFade("Blink",0.1f);
//			}
//		}
	}
}
