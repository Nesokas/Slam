using UnityEngine;
using System.Collections;

public class Player_Behaviour : MonoBehaviour {

	public float acceleration = 0.2f;
	public float shootVelocity = 11;
	public float max_animation_speed = 2f;
	public float increase_speed = 0.1f;

	public Vector3 velocity = Vector3.zero;
	public Vector3 normalized_velocity = Vector3.zero;
	public bool gamepad = false;

	public int team = 1;
	public int player_num = 1;
	public int gamepad_num = 0;
	public bool move = true;

	public Material normal_team_1_material;
	public Material normal_team_2_material;
	public Material shoot_team_1_material;
	public Material shoot_team_2_material;

	public double debug_hit_remaining_time = 0;
	public double debug_hit_time = 30;
	
	public AudioClip ball_sound;
	public GameObject center_circle_left;
	public GameObject center_circle_right;
	public GameObject[] center_planes;

	private Vector3 direction = Vector3.zero;
	private bool ball_collision = false;
	private Material shoot_material;
	private Material normal_material;

	private bool colliding_with_ball = false;
	private Collider ball_collider;
	private Transform player_base;
	private GameObject ball;

	private bool debug_key_pressed = false;
	private bool debug_mode = false;
	private bool hit = false;
	private Vector3 last_ball_position;
	
	private bool is_adding_speed = false;
	private float animation_speed = 1f;
	
	public void DisableGotoCenter(int scored_team)
	{
		player_base = transform.Find("Base");
		Transform base_collider = player_base.Find("Collider");
		Transform shoot_collider = player_base.Find("ColliderShoot");
		if (scored_team == 0){
			if(team == 1){
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			}
		} else if (team == 1) {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			}
		} else {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			}
		}
		for(int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider, false);
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider, false);
		}
	}
	
	public void EnableGotoCenter()
	{
		player_base = transform.Find("Base");
		Transform base_collider = player_base.Find("Collider");
		Transform shoot_collider = player_base.Find("ColliderShoot");
		for (int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider);
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider);
		}
		Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
	}

	public void InitializePlayerInfo(int num, int team_num, Camera m_camera)
	{
		if(num != 0) {
			gamepad_num = num;
			gamepad = true;
		}

		team = team_num;
		player_num = num;

		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
		name_component.m_camera = m_camera;
		name_component.ChangeName("P" + num);
	}

	void VerifyShoot()
	{
		if(colliding_with_ball && !ball_collision && move){

			if(!ball)
				ball = GameObject.FindGameObjectWithTag("ball");

//			Vector3 contact_point = ball_collider.ClosestPointOnBounds(transform.position);
			Vector3 direction = ball.transform.position - transform.position;
			direction.Normalize();

			if((!gamepad && (Input.GetAxis("Shoot") != 0)) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num) != 0))){
				ball_collider.rigidbody.velocity += direction * shootVelocity;
				ball_collision = true;
				colliding_with_ball = false;
				debug_hit_remaining_time = debug_hit_time;
				AudioSource.PlayClipAtPoint(ball_sound, transform.position);
				
				Ball_Behaviour bb = ball.GetComponent<Ball_Behaviour>();
				bb.ReleasePlayers();
			}
		}
	}

	void OnTriggerEnter (Collider collider) 
	{
	    if(collider.gameObject.tag == "ball") {
			colliding_with_ball = true;
			ball_collider = collider;
		}
	}
	
	void OnTriggerStay (Collider collider)
	{
		if(collider.gameObject.tag == "ball") {
			colliding_with_ball = true;
			ball_collider = collider;
		}
	}

	void OnTriggerExit (Collider collider)
	{
	    if(collider.gameObject.tag == "ball") {
			colliding_with_ball = false;
		}
		Debug.Log("exit");
	}

	void IncreaseSpeed() 
	{
		if(move) {
			if(gamepad){
				direction.x = Input.GetAxis("Vertical_Gamepad_" + gamepad_num);
				direction.z = Input.GetAxis("Horizontal_Gamepad_" + gamepad_num);
			} else {
				direction.x = Input.GetAxis("Vertical");
				direction.z = Input.GetAxis("Horizontal");
			}
			direction.Normalize();
			
			if(direction == Vector3.zero)
				is_adding_speed = false;
			else
				is_adding_speed = true;
	
			rigidbody.velocity += direction*acceleration;
		}
	}

	void UpdateRotation()
	{
		if(!ball)
			ball = GameObject.FindGameObjectWithTag("ball");
		var rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
	    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
	}
	
	void UpdateAnimationSpeed()
	{
		if(is_adding_speed) {
			if(animation_speed < max_animation_speed)
				animation_speed += increase_speed;
			else if(animation_speed > max_animation_speed)
				animation_speed = max_animation_speed;
		} else {
			if(animation_speed > 1f)
				animation_speed -= increase_speed;
			else if (animation_speed < 1f)
				animation_speed = 1f;
		}
		
		animation["Idle"].speed = animation_speed;
	}
	
	public void ChangeAnimation(string animation_to_play)
	{
		switch (animation_to_play){
			case "Idle":
				animation.Play("Idle");
				break;
			case "Celebrate":
				animation.Play("Celebrate");
				break;
			case "Sad":
				animation.Play("Sad");
				break;
		}
	}
	
	public void Start () 
	{
		center_planes = GameObject.FindGameObjectsWithTag("center-plane");
		center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
		center_circle_right = GameObject.FindGameObjectWithTag("center-circle-right");
		
		GameObject court_walls = GameObject.FindGameObjectWithTag("court_walls");
		GameObject[] goal_detection = GameObject.FindGameObjectsWithTag("goal_detection");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		player_base = transform.Find("Base");
		Transform base_collider = player_base.Find("Collider");
		Transform shoot_collider = player_base.Find("ColliderShoot");
		Transform court_collider = court_walls.transform.Find("forcefield");
		Physics.IgnoreCollision(court_collider.collider, base_collider.collider);
		Physics.IgnoreCollision(court_collider.collider, shoot_collider.collider);
		for(int i = 0; i < goal_detection.Length; i++) {
			Physics.IgnoreCollision(goal_detection[i].collider, base_collider.collider);
		}

		for (int i = 0; i < players.Length; i++) {
			Transform other_player_base = players[i].transform.Find("Base");
			Transform other_player_shoot_collider = other_player_base.transform.Find("ColliderShoot");
			if(other_player_shoot_collider.collider != shoot_collider.collider) {
				Physics.IgnoreCollision(other_player_shoot_collider.collider, shoot_collider.collider);
				Physics.IgnoreCollision(other_player_shoot_collider.collider, base_collider.collider);
			}
		}

		if(team == 1) {
			normal_material = normal_team_1_material;
			shoot_material = shoot_team_1_material;
		} else {
			normal_material = normal_team_2_material;
			shoot_material = shoot_team_2_material;
		}

		player_base.renderer.material = normal_material;
		
		animation["Idle"].time = Random.Range(0.0f, animation["Idle"].length);
	}

	void DebugMode() 
	{
		if(!ball)
			ball = GameObject.FindGameObjectWithTag("ball");

		Vector3 direction;
		if(hit)
			direction = last_ball_position - transform.position;
		else 
			direction = ball.transform.position - transform.position;

		direction.Normalize();

		Debug.DrawLine(transform.position, transform.position + direction*3, Color.yellow);	
	}

	// Update is called once per frame
	void Update () 
	{
		if(player_base == null)
			player_base = transform.Find("Base");

		if(ball_collision && !((!gamepad && (Input.GetAxis("Shoot") != 0)) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num) != 0)))) {
			ball_collision = false;
		}
		
		if(move) {
			if(!ball_collision && ((!gamepad && (Input.GetAxis("Shoot") != 0)) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num) != 0))))
				player_base.renderer.material = shoot_material;
			else
				player_base.renderer.material = normal_material;
		}

		IncreaseSpeed();
		VerifyShoot();
		UpdateRotation();
		UpdateAnimationSpeed();

		if(Input.GetKeyDown(KeyCode.F1) && !debug_key_pressed) {
			debug_mode = !debug_mode;
			debug_key_pressed = true;
		}

		if(Input.GetKeyUp(KeyCode.F1) && debug_key_pressed)
			debug_key_pressed = false;

		if(debug_hit_remaining_time == 0)
			hit = false;
		else {
			hit = true;
			if(debug_hit_remaining_time == debug_hit_time) {
				if(!ball)
					ball = GameObject.FindGameObjectWithTag("ball");
				last_ball_position = ball.transform.position;
			}
			debug_hit_remaining_time--;
		}

		if(debug_mode)
			DebugMode();
	}
}