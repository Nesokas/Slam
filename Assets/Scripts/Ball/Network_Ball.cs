using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {
	
	private float client_ping;
	private NetState[] server_state_buffer = new NetState[20];
	public float position_error_threshold = 0.2f;
	
	public float PING_MARGIN = 0.5f;
	
	public Transform observed_transform;
	
	public Vector3 server_pos;
	
	private Predictor predictor;
	
	new void Awake()
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
			
		predictor.Predict(networkView);
		
		transform.position = predictor.getObservedTransform().position;
			
	}
}
