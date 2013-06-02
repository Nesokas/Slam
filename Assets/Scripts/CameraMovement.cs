using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{

	public float smooth = 1.5f;
	
	private Transform ball;
	private Vector3 rel_camera_pos;
	private float rel_camera_pos_mag;
	private Vector3 new_pos;
	
	void Awake()
	{
		//ball = GameObject.FindGameObjectWithTag("ball").transform;
	//	rel_camera_pos = transform.position - ball.position;
		//rel_camera_pos_mag = rel_camera_pos.magnitude - 0.5f;
		rel_camera_pos_mag = -1;
		
	}
	
	void Update()
	{
		if (!ball) {
			ball = GameObject.FindGameObjectWithTag("ball").transform;
			rel_camera_pos = transform.position - ball.position;
			rel_camera_pos_mag = rel_camera_pos.magnitude - 0.5f;
		}
		Vector3 standard_pos = ball.position + rel_camera_pos;
		Vector3 above_pos = ball.position + Vector3.up * rel_camera_pos_mag;
		Vector3[] checkpoints = new Vector3[5];
		checkpoints[0] = standard_pos;
		checkpoints[1] = Vector3.Lerp(standard_pos, above_pos, 0.25f);
		checkpoints[2] = Vector3.Lerp(standard_pos, above_pos, 0.5f);
		checkpoints[3] = Vector3.Lerp(standard_pos, above_pos, 0.75f);
		checkpoints[4] = above_pos;
		
		for (int i = 0; i < checkpoints.Length; i++)
			if (ViewPosCheck(checkpoints[i]))
				break;
		
		transform.position = Vector3.Lerp(transform.position, new_pos, smooth * Time.deltaTime);
		SmoothLookAt();
		
	}
		
	bool ViewPosCheck(Vector3 check_pos)
	{
		RaycastHit hit;
	Debug.Log( rel_camera_pos_mag );
		if (Physics.Raycast(check_pos, ball.position - check_pos, out hit, rel_camera_pos_mag)){
			
			if(hit.transform != ball)
				return false;
		}
		new_pos = check_pos;
		return true;
	}
	
	void SmoothLookAt()
	{
		Vector3 rel_ball_position = ball.position - transform.position;
		Quaternion lookat_rotation = Quaternion.LookRotation(rel_ball_position, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, lookat_rotation, smooth*Time.deltaTime);
	}
}
