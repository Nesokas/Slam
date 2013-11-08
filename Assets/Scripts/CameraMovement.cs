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
	private float INIT_BEST_SCORER_SHAKE_TIME = 0.25f;
	private float best_scorer_shake_time;
	public bool is_shaking = false;
	private Quaternion initial_rotation;
	private float CAMERA_SHAKE_SPEED = 2f;
	private float camera_shake_rotate;
	private float MAX_CAMERA_SHAKE_ROTATE = 2f;
	private bool camera_shake_rot_decreasing = true;
	
	void Awake()
	{
		camera_shake_rotate = MAX_CAMERA_SHAKE_ROTATE;
		best_scorer_shake_time = INIT_BEST_SCORER_SHAKE_TIME;
		rel_camera_pos_mag = -1;	
	}
	
	void Update()
	{
		if (!ball) {
			if(GameObject.FindGameObjectWithTag("ball")){
				ball = GameObject.FindGameObjectWithTag("ball").transform;
				rel_camera_pos = transform.position - ball.position;
				rel_camera_pos_mag = rel_camera_pos.magnitude;
			}
		} else {
			Vector3 standard_pos = ball.position + rel_camera_pos;
			Vector3 above_pos = ball.position + Vector3.up * rel_camera_pos_mag;
	
			clamped_pos = new Vector3(Mathf.Clamp(standard_pos.x, -9.5f, -7.8f), standard_pos.y, Mathf.Clamp(standard_pos.z, -5f, 5f));
		//	Debug.Log(Vector3.Lerp(transform.position, clamped_pos, smooth * Time.deltaTime));
			transform.position = Vector3.Lerp(transform.position, clamped_pos, smooth * Time.deltaTime);
		}
//		ShakeCamera();
		if (is_shaking) {
			transform.rotation = 
				Quaternion.Euler(transform.rotation.eulerAngles.x,
					transform.rotation.eulerAngles.y,
					transform.rotation.eulerAngles.z +  AlternateCameraShake());
			best_scorer_shake_time -= Time.deltaTime;
			if (best_scorer_shake_time <= 0)
				StopShaking();
		}
	}
	
	private float AlternateCameraShake()
	{
		if (camera_shake_rot_decreasing)
			if (camera_shake_rotate > -MAX_CAMERA_SHAKE_ROTATE )
				camera_shake_rotate -= CAMERA_SHAKE_SPEED;
			else
				camera_shake_rot_decreasing = false;
		else {
			if (camera_shake_rotate < MAX_CAMERA_SHAKE_ROTATE) {
				camera_shake_rotate += CAMERA_SHAKE_SPEED;
				
			}
			else
				camera_shake_rot_decreasing = true;
		}
		return camera_shake_rotate;

	}
	
	void SmoothLookAt()
	{
		Vector3 rel_ball_position = ball.position - transform.position;
		Quaternion lookat_rotation = Quaternion.LookRotation(rel_ball_position, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, lookat_rotation, smooth*Time.deltaTime);
	}
	
	public void ShakeCamera()
	{	
		if (is_shaking == true)
			return;
		initial_rotation = transform.rotation;
		is_shaking = true;
	}
	
	public void StopShaking()
	{
		
		transform.rotation = initial_rotation;
		is_shaking = false;
		best_scorer_shake_time = INIT_BEST_SCORER_SHAKE_TIME;
	}
}
