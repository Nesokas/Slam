using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {

	void OnCollisionEnter(Collision collider)
	{
		if(collider.gameObject.tag == "forcefield") {
			networkView.RPC("CourtCollision", RPCMode.All, collider.contacts[0].point);
		} else {
			ReleasePlayers();

		}
	}
	
	[RPC]
	void CourtCollision(Vector3 point)
	{
		Forcefield forcefield = GameObject.FindGameObjectWithTag("forcefield").GetComponent<Forcefield>();
		forcefield.BallCollition(point);
		Debug.Log("wall hit");
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
	
	new void Start()
	{
		if (!networkView.isMine) {	
			enabled = false;
		}
		base.Start();
	}
}
