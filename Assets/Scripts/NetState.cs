using UnityEngine;
using System.Collections;

public class NetState {
	
	public float timestamp;
	public Vector3 pos;
	
	public NetState() 
	{
		timestamp = 0.0f;
		pos = Vector3.zero;
	}
	
	public NetState(float time, Vector3 pos)
	{
		timestamp = time;
		this.pos = pos;
	}
}
