using UnityEngine;
using System.Collections;

public class Predictor {
	
	private float client_ping;
	private NetState server_state;
	public float position_error_threshold = 0.2f;
	
	public float PING_MARGIN = 0.5f;
	
	public Transform observed_transform;
	
	public Vector3 server_pos;
	private Game_Settings game_settings;


	public Predictor(Transform transform)
	{
		observed_transform = transform;

		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = settings.GetComponent<Game_Settings>();
	}
	
	public void LerpPosToTarget()
	{
		float distance = Vector3.Distance(observed_transform.position, server_pos);
		
		// only correct if the error margin (the distance) is too extreme
		if (distance >= position_error_threshold) {
			
			float lerp = ((1 / distance) * 10f) / 100f;
			
			observed_transform.position = Vector3.Lerp (observed_transform.position, server_pos, lerp);
		}
	}
	
	public void OnSerializePhotonViewBall(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 pos = observed_transform.position;
		Vector3 angVelocity = observed_transform.rigidbody.angularVelocity;
		Vector3 velocity = observed_transform.rigidbody.velocity;
		
		if (stream.isWriting) {
		
			stream.SendNext(pos);
			stream.SendNext(velocity);
			stream.SendNext(angVelocity);
			
		} else {
			
			//This code takes care of the local client
			pos = (Vector3)stream.ReceiveNext();
			velocity = (Vector3)stream.ReceiveNext();
			angVelocity = (Vector3)stream.ReceiveNext();

			server_pos = pos;
			
			// smoothly correct clients position
			LerpPosToTarget();
			
			//Override the first element with the latest server info
			server_state = new NetState((float)info.timestamp, pos, velocity, angVelocity);
		}
	}
	
	
	public void OnPhotonSerializeViewPlayer(PhotonStream stream, PhotonMessageInfo info)
	{
		Debug.Log("Called");
		Vector3 pos = observed_transform.position;
		Vector3 velocity = observed_transform.rigidbody.velocity;
		
		if (stream.isWriting) {
		
			stream.SendNext(pos);
			stream.SendNext(velocity);
			
		} else {
			
			//This code takes care of the local client
			pos = (Vector3)stream.ReceiveNext();
			velocity = (Vector3)stream.ReceiveNext();
			server_pos = pos;
			
			// smoothly correct clients position
			LerpPosToTarget();
			
			//Override the first element with the latest server info
			server_state = new NetState((float)info.timestamp, pos, velocity);
		}
	}
	
	
	public Transform getPredictedTransform()
	{
		return observed_transform;
	}
	
	public void PredictPlayer(PhotonView photonView)
	{
		
		if (PhotonNetwork.player == photonView.owner || game_settings.is_game_creator) {
			return; //This is only for remote peers, get off!!
		}
		
		//client side has **only the server connected**
		client_ping = (PhotonNetwork.networkingPeer.RoundTripTime / 100) + PING_MARGIN;
		
		float interpolation_time = (float)PhotonNetwork.time - client_ping;
		
		//ensure the buffer has at last one element
		if (server_state == null)
			server_state = new NetState(0, observed_transform.position, observed_transform.rigidbody.velocity);

		NetState latest = server_state;
		if(!latest.state_used) {
//			observed_transform.position = Vector3.Lerp(observed_transform.position, latest.pos, 0.5f);
			observed_transform.rigidbody.velocity = latest.velocity;
			server_state.state_used = true;
			
			
			float x,y,z;
		
			x = server_state.pos.x + server_state.velocity.x*((float)PhotonNetwork.time - server_state.timestamp);
			y = server_state.pos.y + server_state.velocity.y*((float)PhotonNetwork.time - server_state.timestamp);
			z = server_state.pos.z + server_state.velocity.z*((float)PhotonNetwork.time - server_state.timestamp);
			
			RaycastHit hit;
			Vector3 predicted_pos = new Vector3(x,y,z);
			Vector3 direction = predicted_pos;
			direction.Normalize();
			
			float distance = Vector3.Distance(latest.pos, predicted_pos);
			
			if(distance != 0 && Physics.Raycast(latest.pos, direction, out hit, Mathf.Abs(distance))) {
				if(hit.collider.gameObject.tag == "court_walls"){
					direction = direction*(-1);
					Transform player_base = observed_transform.Find("Base");
					Transform collider_transform = player_base.Find("Collider");
					x = hit.point.x + direction.x*((SphereCollider)collider_transform.collider).radius;
					y = hit.point.y + direction.y*((SphereCollider)collider_transform.collider).radius;
					z = hit.point.z + direction.z*((SphereCollider)collider_transform.collider).radius;
				}
			}

			observed_transform.position =  Vector3.Lerp (observed_transform.position, new Vector3(x,y,z), 0.25f);
		}
	}
	
	
	public void PredictBall(PhotonView photonView)
	{
		
		if (PhotonNetwork.player == photonView.owner || game_settings.is_game_creator) {
			return; //This is only for remote peers, get off!!
		}
		
		//client side has **only the server connected**
		client_ping = (PhotonNetwork.networkingPeer.RoundTripTime / 100) + PING_MARGIN;
		
		float interpolation_time = (float)PhotonNetwork.time - client_ping;
		
		//ensure the buffer has at last one element
		if (server_state == null)
			server_state = new NetState(0, observed_transform.position, observed_transform.rigidbody.velocity, observed_transform.rigidbody.angularVelocity);

		NetState latest = server_state;
		if(!latest.state_used) {
//			observed_transform.position = Vector3.Lerp(observed_transform.position, latest.pos, 0.5f);
			observed_transform.rigidbody.velocity = latest.velocity;
			observed_transform.rigidbody.angularVelocity = latest.angVelocity;
			
			server_state.state_used = true;
			
			
			float x,y,z;
		
			x = server_state.pos.x + server_state.velocity.x*((float)PhotonNetwork.time - server_state.timestamp);
			y = server_state.pos.y + server_state.velocity.y*((float)PhotonNetwork.time - server_state.timestamp);
			z = server_state.pos.z + server_state.velocity.z*((float)PhotonNetwork.time - server_state.timestamp);
			
			RaycastHit hit;
			Vector3 predicted_pos = new Vector3(x,y,z);
			Vector3 direction = predicted_pos;
			direction.Normalize();
			
			float distance = Vector3.Distance(latest.pos, predicted_pos);
			
			if(distance!=0 && Physics.Raycast(latest.pos, direction, out hit, Mathf.Abs(distance))) {
				if(hit.collider.gameObject.tag == "court_walls"){
					direction = direction*(-1);
					x = hit.point.x + direction.x*((SphereCollider)observed_transform.collider).radius;
					y = hit.point.y + direction.y*((SphereCollider)observed_transform.collider).radius;
					z = hit.point.z + direction.z*((SphereCollider)observed_transform.collider).radius;
				}
				
			}

			observed_transform.position =  Vector3.Lerp (observed_transform.position, new Vector3(x,y,z), 0.25f);
		}
	}	
	
}
