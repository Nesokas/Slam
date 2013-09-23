using UnityEngine;
using System.Collections;

public class Fan_Behaviour : MonoBehaviour {
	
	public int team;
	private GameObject ball;
	private bool celebration_period = false;
	private int team_scored;
	private bool was_sad;
	
	void Awake()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
		NotificationCenter.DefaultCenter.AddObserver(this, "StopCelebration");
		transform.animation.Stop();
	}
	
	void StopCelebration()
	{
		celebration_period = false;
	}
	
	void OnGoal(NotificationCenter.Notification notification)
	{
		team_scored = (int)notification.data["team"];
		celebration_period = true;
		was_sad = false;
	}
	
	void UpdateRotation()
	{
		if(!ball)
			ball = GameObject.FindGameObjectWithTag("ball");
		else {
			var rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
		    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRotation();
		if(celebration_period){
			if(team == team_scored){
				if(!transform.animation.IsPlaying("Celebrate")){
					int random = Random.Range(0, 30);
					if (random == 0){
						transform.animation.CrossFade("Celebrate", 0.2f);
					}
				}
			} else {
				if(!transform.animation.IsPlaying("Sad") && !was_sad){
					int random = Random.Range(0, 10);
					if (random == 0){
						was_sad = true;
						transform.animation.CrossFade("Sad", 0.2f);
					}
				}
			}
		} else if(!transform.animation.IsPlaying("Idle")) {
			transform.animation.CrossFade("Idle",0.5f);
			transform.animation["Idle"].time = Random.Range(0, transform.animation["Idle"].length);
		}
		
	}
	
	public IEnumerator Celebrate()
	{
		if(!celebration_period) {
			transform.animation.CrossFade("Celebrate", 1f);
			yield return new WaitForSeconds(Random.Range(transform.animation["Celebrate"].length*8f, transform.animation["Celebrate"].length*16f));
			transform.animation.CrossFade("Idle", 0.5f);
		}
	}
}
