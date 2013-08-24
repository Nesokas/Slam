using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{

	public float smooth = 0.8f;

	private Transform ball;
	private Vector3 rel_camera_pos;
	private float rel_camera_pos_mag;
	private Vector3 new_pos;
	private Vector3 clamped_pos;
	
	void Awake()
	{
		rel_camera_pos_mag = -1;	
	}
	
	void Update()
	{
		if (!ball && GameObject.FindGameObjectWithTag("ball")) {
			ball = GameObject.FindGameObjectWithTag("ball").transform;
			rel_camera_pos = transform.position - ball.position;
			rel_camera_pos_mag = rel_camera_pos.magnitude;
		
			Vector3 standard_pos = ball.position + rel_camera_pos;
			Vector3 above_pos = ball.position + Vector3.up * rel_camera_pos_mag;
		
			clamped_pos = new Vector3(Mathf.Clamp(standard_pos.x, -9.5f, -7.8f), standard_pos.y, Mathf.Clamp(standard_pos.z, -5f, 5f));
		//	Debug.Log(Vector3.Lerp(transform.position, clamped_pos, smooth * Time.deltaTime));
			transform.position = Vector3.Lerp(transform.position, clamped_pos, smooth * Time.deltaTime);
		}
	}
	
	
	void SmoothLookAt()
	{
		Vector3 rel_ball_position = ball.position - transform.position;
		Quaternion lookat_rotation = Quaternion.LookRotation(rel_ball_position, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, lookat_rotation, smooth*Time.deltaTime);
	}
}
