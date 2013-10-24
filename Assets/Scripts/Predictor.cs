using UnityEngine;
using System.Collections;

public class Predictor : MonoBehaviour {
	
	public float PING_MARGIN = 0.5f;
	
	public Transform observed_transform;
	public GameObject subject_player;
	
	private float client_ping;
	private NetState[] server_state_buffer = new NetState[20];
	
	
	
	void Start () 
	{
		
	}
	
	void Update ()
	{

	}
}
