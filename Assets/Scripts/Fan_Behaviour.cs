using UnityEngine;
using System.Collections;

public class Fan_Behaviour : MonoBehaviour {
	
	public int team;
	private GameObject ball;
	private bool celebration_period = false;
	private string play_animation;
	private bool was_sad;
	
	void Awake()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "StopCelebration");
		NotificationCenter.DefaultCenter.AddObserver(this, "ChangeReaction");
		transform.animation.Stop();
	}
	
	void StopCelebration()
	{
		celebration_period = false;
	}
	
	void ChangeReaction(NotificationCenter.Notification notification)
	{
		if ((int)notification.data["team"] == team) {
			play_animation = (string)notification.data["reaction"];
			celebration_period = true;
			was_sad = false;
		}
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
			if(!transform.animation.IsPlaying(play_animation)){
				int random = Random.Range(0, 30);
				if (random == 0){
					transform.animation.CrossFade(play_animation, 0.2f);
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
