using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {
	
	public Transform observed_transform;
	
	private Predictor predictor;
	
	void Awake()
	{
//		base.Awake();
		observed_transform = transform;
	}
	
	new void Start()
	{
		predictor = new Predictor(transform);
		base.Start();
	}
	
	void OnCollisionEnter(Collision collider)
	{
		if(collider.gameObject.tag == "forcefield") {
			CourtCollision(collider.contacts[0].point);
		} else {
			ReleasePlayers();
		}
	}
	
//	[RPC]
	void CourtCollision(Vector3 point)
	{
		Forcefield forcefield = GameObject.FindGameObjectWithTag("forcefield").GetComponent<Forcefield>();
		forcefield.BallCollition(point);
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
	

	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		predictor.OnSerializeNetworkViewBall(stream, info);
	}
	
	new void Update()
	{
		base.Update();
			
		predictor.PredictBall(networkView);
		
		transform.position = predictor.getPredictedTransform().position;
		transform.rotation = predictor.getPredictedTransform().rotation;
			
	}
}
