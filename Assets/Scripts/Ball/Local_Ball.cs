using UnityEngine;
using System.Collections;

public class Local_Ball : Ball_Behaviour {

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
}
