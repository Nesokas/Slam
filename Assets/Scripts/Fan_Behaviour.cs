using UnityEngine;
using System.Collections;

public class Fan_Behaviour : MonoBehaviour {
	
	public int team;
	private GameObject center;
	private GameObject ball;
	private bool celebration_period = false;
	private string play_animation;
	private bool was_sad;
	private Transform hero;
	
	void Awake()
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "StopCelebration");
		NotificationCenter.DefaultCenter.AddObserver(this, "ChangeReaction");
		//hero.animation.Stop();
	}

	void StopCelebration()
	{
		celebration_period = false;
	}

	public void HeroStarted(GameObject center)
	{
		hero = transform.GetChild(0);
		hero.animation.Stop();
		this.center = center;
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
		GameObject look_to = ball;
		if(!ball){
			ball = GameObject.FindGameObjectWithTag("ball");

			if(!ball) {
				look_to = center;
			}

		} else {
			var rotation = Quaternion.LookRotation(transform.position - look_to.transform.position);
		    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRotation();
		if(celebration_period){
			if(!hero.animation.IsPlaying(play_animation)){
				int random = Random.Range(0, 30);
				if (random == 0){
					hero.animation.CrossFade(play_animation, 0.2f);
				}
			}
		} else if(!hero.animation.IsPlaying("Idle")) {
			hero.animation.CrossFade("Idle",0.5f);
			hero.animation["Idle"].time = Random.Range(0, hero.animation["Idle"].length);
		}
		
	}
	
	public IEnumerator Celebrate()
	{
		if(!celebration_period) {
			hero.animation.CrossFade("Celebrate", 1f);
			yield return new WaitForSeconds(Random.Range(hero.animation["Celebrate"].length*8f, hero.animation["Celebrate"].length*16f));
			hero.animation.CrossFade("Idle", 0.5f);
		}
	}
}
