using UnityEngine;
using System.Collections;

public class NetState {
	
	public float timestamp;
	public Vector3 pos;
	public Vector3 velocity;
	public bool state_used;
	
	public NetState() 
	{
		timestamp = 0.0f;
		pos = Vector3.zero;
		velocity = Vector3.zero;
		state_used = false;
	}
	
	public NetState(float time, Vector3 pos, Vector3 velocity)
	{
		timestamp = time;
		this.pos = pos;
		this.velocity = velocity;
		state_used = false;
	}
}
