using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {
	
	private bool game_restarted = true;
	private bool animation_finished = true;
	private bool rolling_eyes = false;
	
	/*animations of type1 can be crossfaded to type2
	 * such as look_left -> Default*/
	string[] animationsType1;
	string[] animationsType2;
	
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
				if (!rolling_eyes && !animation.IsPlaying("Tired") && !animation.IsPlaying("rolling_eyes")) {
					StopCoroutine("PlayAnimation");
					animation.Stop();
					rolling_eyes = true;
					animation_finished = true;
					StartCoroutine(LoopAnimation("Rolling_Eyes", "Tired", 1));
				}
				
			}
		}
	}
	
	IEnumerator LoopAnimation(string anim1, string anim2, int repeatNumber)
	{
		transform.animation.CrossFade(anim1, 0.3f);
		yield return new WaitForSeconds(animation[anim1].length*0.35f);
		transform.animation["Rolling_Eyes"].wrapMode = WrapMode.Loop;
		animation.CrossFade(anim2, 1f);
		rolling_eyes = false;
	}
	
	IEnumerator PlayAnimation(string[] animType1, string[] animType2, int repeatNumber)
	{	
		string animation1 = animType1[Random.Range(0, animType1.Length)];
		string animation2 = animType2[0];
		animation_finished = false;
		animation[animation1].wrapMode = WrapMode.ClampForever;
		transform.animation.CrossFade(animation1, 0.3f);
		yield return new WaitForSeconds(animation[animation1].length*2.35f);
		if (!rolling_eyes) {
			animation.CrossFade(animation2, 0.2f);
		}
		animation_finished = true;
//		animation[anim1].wrapMode = WrapMode.Default;
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
		
<<<<<<< HEAD
		animationsType1 = new string[] {"look_left", "look_right"};
		animationsType2 = new string[] {"Default"};
		
=======
		if (!networkView.isMine) {	
			enabled = false;
		}
>>>>>>> 4819894bb1d3734b76e37484fa148b520f691afd
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
		animation["Blink"].layer = 1;
		animation["Tired"].layer = 1;
		animation["Default"].layer = 10;
		animation_finished = true;
	}
	
	void Update()
	{
		if (!rolling_eyes && !animation.IsPlaying("Tired") && !animation.IsPlaying("rolling_eyes")) {
			if (animation_finished == true) {
				Debug.Log("FINISHED");
				int rand = Random.Range(0, 100);
				if (rand < 1) {
					StartCoroutine(PlayAnimation(animationsType1, animationsType2, 1));
				
				}
			} else {
				Debug.Log("animation not finished");
			}
			if (Random.Range(0,100) == 0)
				animation.Play("Blink");
		}
			
	}
}
