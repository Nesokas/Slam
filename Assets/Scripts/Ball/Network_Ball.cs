using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {
	
	private float client_ping;
	private NetState[] server_state_buffer = new NetState[20];
	public float position_error_threshold = 0.2f;
	
	public float PING_MARGIN = 0.5f;
	
	public Transform observed_transform;
	
	
	public Vector3 server_pos;
	
	new void Awake()
	{
//		base.Awake();
		observed_transform = transform;
	}
	
	void OnCollisionEnter(Collision collider)
	{
		if(collider.gameObject.tag == "forcefield") {
//			networkView.RPC("CourtCollision", RPCMode.All, collider.contacts[0].point);
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
	
	public void LerpToTarget()
	{
		float distance = Vector3.Distance(transform.position, server_pos);
		
		// only correct if the error margin (the distance) is too extreme
		if (distance >= position_error_threshold) {
			
			float lerp = ((1 / distance) * 10f) / 100f;
			
			transform.position = Vector3.Lerp (transform.position, server_pos, lerp);
		}
	}
	
	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 pos = observed_transform.position;
		Vector3 velocity = observed_transform.rigidbody.velocity;
		
		if (stream.isWriting) {
		
			stream.Serialize(ref pos);
			stream.Serialize(ref velocity);
			
		} else {
			
			//This code takes care of the local client
			stream.Serialize(ref pos);
			stream.Serialize(ref velocity);
			server_pos = pos;
			
			// smoothly correct clients position
			LerpToTarget();
			
			// Take care of data for interpolating remote objects movements
			// Shift up the buffer
			for (int i = server_state_buffer.Length-1; i >= 1; i--)
				server_state_buffer[i] = server_state_buffer[i-1];
			
			//Override the first element with the latest server info
			server_state_buffer[0] = new NetState((float)info.timestamp, pos, velocity);
		}
	}
	
	new void Update()
	{
		base.Update();
		
		if (Network.player == networkView.owner || Network.isServer) {
			return; //This is only for remote peers, get off!!
		}
		
		//client side has **only the server connected**
		client_ping = (Network.GetAveragePing(Network.connections[0]) / 100) + PING_MARGIN;
		
		float interpolation_time = (float)Network.time - client_ping;
		
		//ensure the buffer has at last one element
		if (server_state_buffer[0] == null)
			server_state_buffer[0] = new NetState(0, transform.position, transform.rigidbody.velocity);
		
		
		Debug.Log(Network.time - server_state_buffer[0].timestamp);
		

		
		//Try interpolation if possible.
		//If the latest server_state_buffer timestamp is smaller than the latency
		//we're not slow enough to really lag out and just extrapolate.
//		if (server_state_buffer[0].timestamp > interpolation_time) {
//			for (int i = 0; i < server_state_buffer.Length; i++) {
//				if (server_state_buffer[i] == null)
//					continue;
//				// Find the state which matches the interp. time or use last state
//				if (server_state_buffer[i].timestamp <= interpolation_time || i == server_state_buffer.Length-1) {
//					// The state one frame newer than the best playback state
//					NetState best_target = server_state_buffer[Mathf.Max(i-1,0)];
//					//The best playback state (closest current network time)
//					NetState best_start = server_state_buffer[i];
//					
//					float time_diff = best_target.timestamp - best_start.timestamp;
//					float lerp_time = 0.0f;
//					
//					if (time_diff > 0.0001) {
//						lerp_time = ((interpolation_time - best_start.timestamp) / time_diff);
//					}
//					
//					transform.position = Vector3.Lerp(best_start.pos, best_target.pos, lerp_time);
//					
//					return;
//				}
//			}
//		}
//		
//		else {
		NetState latest = server_state_buffer[0];
		if(!latest.state_used){
			transform.position = Vector3.Lerp(transform.position, latest.pos, 0.5f);
			transform.rigidbody.velocity = latest.velocity;
			server_state_buffer[0].state_used = true;
			
			
			float x,y,z;
		
			x = server_state_buffer[0].pos.x + server_state_buffer[0].velocity.x*((float)Network.time - server_state_buffer[0].timestamp);
			y = server_state_buffer[0].pos.y + server_state_buffer[0].velocity.y*((float)Network.time - server_state_buffer[0].timestamp);
			z = server_state_buffer[0].pos.z + server_state_buffer[0].velocity.z*((float)Network.time - server_state_buffer[0].timestamp);
			
			transform.position =  Vector3.Lerp (transform.position, new Vector3(x,y,z), 0.25f);
			
			
		}
//		Debug.Log(transform.rigidbody.velocity + " " + transform.position);
//		}
	}
}
