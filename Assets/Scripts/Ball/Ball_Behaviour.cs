using UnityEngine;
using System.Collections;

public class Ball_Behaviour : MonoBehaviour {
	
	protected bool game_restarted = true;
	protected bool animation_finished = true;
	protected bool rolling_eyes = false;

	private int current_area = 7;

	/*animations of type1 can be crossfaded to type2
	 * such as look_left -> Default*/
	string[] animationsType1;
	string[] animationsType2;
	
	protected GameObject last_player_touched;
//	protected GameObject last_player_shoot;
	
	protected bool is_looking_somewhere;
	
	public void GameHasRestarted()
	{
		game_restarted = true;
	}
	
	void OnCollisionEnter(Collision collider)
	{
		if(collider.gameObject.tag == "forcefield") {
			CourtCollision(collider.contacts[0].point);
		} else {
			ReleasePlayers();
		}
		
	}
	
		void CourtCollision(Vector3 point)
	{
		Forcefield forcefield = GameObject.FindGameObjectWithTag("forcefield").GetComponent<Forcefield>();
		forcefield.BallCollition(point);
		int random = Random.Range(0,100);
		if(random <= 10) {
			transform.GetComponent<Animation>()["Rolling_Eyes"].wrapMode = WrapMode.Loop;
			if (!rolling_eyes && !GetComponent<Animation>().IsPlaying("Tired") && !GetComponent<Animation>().IsPlaying("rolling_eyes")) {
				StopCoroutine("PlayAnimation");
				GetComponent<Animation>().Stop();
				rolling_eyes = true;
				animation_finished = true;
				StartCoroutine(LoopAnimation("Rolling_Eyes", "Tired", 1));
			}
			
		}
	}

	public void SetCurrentArea(int area)
	{
		current_area = area;
	}

	public int GetCurrentArea()
	{
		return current_area;
	}
	
	protected void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "colliderShoot")
			last_player_touched = collider.gameObject.transform.parent.gameObject;
	}
	
	public GameObject GetLastPlayerTouched() 
	{
		return last_player_touched;
	}
	
	public void SetLastPlayerTouched(GameObject player)
	{
		last_player_touched = player;
	}
	
	public void OnCollisionExit(Collision collider)
	{
		if(collider.gameObject.tag == "Player")
			last_player_touched = collider.gameObject;

	}
	
	protected IEnumerator LoopAnimation(string anim1, string anim2, int repeatNumber)
	{
		transform.GetComponent<Animation>().CrossFade(anim1, 0.3f);
		yield return new WaitForSeconds(GetComponent<Animation>()[anim1].length*0.35f);
		transform.GetComponent<Animation>()["Rolling_Eyes"].wrapMode = WrapMode.Loop;
		GetComponent<Animation>().CrossFadeQueued(anim2, 1f, QueueMode.PlayNow);
		 GetComponent<Animation>().CrossFadeQueued("Default", 2.5f, QueueMode.PlayNow);
		rolling_eyes = false;
	}
	
	protected IEnumerator PlayAnimation(string[] animType1, string[] animType2, int repeatNumber)
	{	
		string animation1 = animType1[Random.Range(0, animType1.Length)];
		string animation2 = animType2[0];
		animation_finished = false;
		GetComponent<Animation>()[animation1].wrapMode = WrapMode.ClampForever;
		transform.GetComponent<Animation>().CrossFade(animation1, 0.3f);
		yield return new WaitForSeconds(GetComponent<Animation>()[animation1].length*2.35f);
		if (!rolling_eyes) {
			GetComponent<Animation>().CrossFade(animation2, 0.2f);
		}
		animation_finished = true;
//		animation[anim1].wrapMode = WrapMode.Default;
	}
	
	public void ReleasePlayers()
	{
	//	if (Application.loadedLevelName == "Main_Game" && game_restarted) //ELIMINEI PARA A TESE
		if (game_restarted)
		{
			GameObject gbo = GameObject.FindGameObjectWithTag("GameController");
			Game_Behaviour gb = gbo.GetComponent<Game_Behaviour>();
			gb.ReleasePlayers();
			game_restarted = false;
		}
	}

	protected void Start () 
	{	
		is_looking_somewhere = false;
		
		animationsType1 = new string[] {"look_left", "look_right", "look_up", "look_down"};
		animationsType2 = new string[] {"Default"};
		
	//	if(Application.loadedLevelName == "Main_Game") {
			GameObject[] center_planes = GameObject.FindGameObjectsWithTag("center-plane");
			GameObject center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
			GameObject center_circle_rigth = GameObject.FindGameObjectWithTag("center-circle-right");
			
			for(int i = 0; i < center_planes.Length; i++)
				Physics.IgnoreCollision(center_planes[i].GetComponent<Collider>(), transform.GetComponent<Collider>());
			Physics.IgnoreCollision(center_circle_left.GetComponent<Collider>(), transform.GetComponent<Collider>());
			Physics.IgnoreCollision(center_circle_rigth.GetComponent<Collider>(), transform.GetComponent<Collider>());
	//	}
		transform.GetComponent<Animation>().Stop();
		transform.GetComponent<Animation>()["Rolling_Eyes"].speed = 7.5f;
		GetComponent<Animation>()["Rolling_Eyes"].layer=1;
		GetComponent<Animation>()["Blink"].AddMixingTransform( transform.Find("Armature_001") );
		GetComponent<Animation>()["Blink"].layer = 1;
		GetComponent<Animation>()["Tired"].layer = 1;
		GetComponent<Animation>()["Default"].layer = 10;
		animation_finished = true;
	}
	
	protected void Update()
	{
		if (!rolling_eyes && !GetComponent<Animation>().IsPlaying("Tired") && !GetComponent<Animation>().IsPlaying("rolling_eyes")) {
			if (animation_finished == true) {
//				Debug.Log("FINISHED");
				int rand = Random.Range(0, 1000);
				if (rand < 1) {
					GetComponent<Animation>().CrossFade("Tired", 0.1f);
				} else if (rand < 10) {
					StartCoroutine(PlayAnimation(animationsType1, animationsType2, 1));
				}
			} else {
//				Debug.Log("animation not finished");
			}
			if (Random.Range(0,100) == 0)
				GetComponent<Animation>().Play("Blink");
		}
			
	}
}
