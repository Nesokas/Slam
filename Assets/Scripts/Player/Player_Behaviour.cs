using UnityEngine;
using System.Collections;

public class Player_Behaviour : MonoBehaviour {
	
	private float DASH_COOLDOWN = 10f;
	private float DASH_DURATION = 2f;
	private float dash_cooldown = 0f;
	
	public float acceleration = 0.2f;
	public float shootVelocity = 11;
	public float max_animation_speed = 2f;
	public float increase_speed = 0.1f;
	

	public Vector3 velocity = Vector3.zero;
	public Vector3 normalized_velocity = Vector3.zero;
	public bool gamepad = false;

	public int gamepad_num = 0;
	public bool move = true;

	public double debug_hit_remaining_time = 0;
	public double debug_hit_time = 30;
	
	public AudioClip ball_sound;
	
	public Material normal_team_1_material;
	public Material shoot_team_1_material;

	protected Vector3 direction = Vector3.zero;
	protected bool ball_collision = false;
	protected Material shoot_material;
	protected Material normal_material;

	protected bool colliding_with_ball = false;
	protected Collider ball_collider;
	protected Transform player_base;
	protected GameObject ball;

	protected bool debug_key_pressed = false;
	protected bool debug_mode = false;
	protected bool hit = false;
	protected Vector3 last_ball_position;
	
	protected bool is_adding_speed = false;
	protected float animation_speed = 1f;
	
	protected PlayerController.Commands commands;


	void VerifyShoot()
	{
		if(colliding_with_ball && !ball_collision && move){

			if(!ball)
				ball = GameObject.FindGameObjectWithTag("ball");

			Vector3 direction = ball.transform.position - transform.position;
			direction.Normalize();

			if((!gamepad && commands.shoot != 0)){
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
	
	void VerifyDash()
	{		
		Debug.Log(dash_cooldown + " " + Time.time);
		if (commands.dash != 0 && (Time.time > dash_cooldown) && (commands.horizontal_direction != 0 || commands.vertical_direction != 0)) {
			dash_cooldown =  DASH_COOLDOWN + Time.time;
			rigidbody.velocity *= 2f;
			Debug.Log("DASHING");
		}
	}

	protected void OnTriggerEnter (Collider collider) 
	{
	    if(collider.gameObject.tag == "ball") {
			colliding_with_ball = true;
			ball_collider = collider;
		}
	}
	
	protected void OnTriggerStay (Collider collider)
	{
		if(collider.gameObject.tag == "ball") {
			colliding_with_ball = true;
			ball_collider = collider;
		}
	}

	protected void OnTriggerExit (Collider collider)
	{
	    if(collider.gameObject.tag == "ball") {
			colliding_with_ball = false;
		}
	}

	void IncreaseSpeed() 
	{
		if(move) {
			direction.x = commands.vertical_direction;
			direction.z = commands.horizontal_direction;
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
				animation.CrossFade("Celebrate", 0.3f);
				break;
			case "Sad":
				animation.CrossFade("Sad", 0.3f);
				break;
		}
	}
	
	protected void Start () 
	{		
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
		
		normal_material = normal_team_1_material;
		shoot_material = shoot_team_1_material;

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
	protected void Update () 
	{
		if(player_base == null)
			player_base = transform.Find("Base");

		if(ball_collision && commands.shoot == 0) {
			ball_collision = false;
		}
		
		if(move) {
			if(!ball_collision && commands.shoot != 0)
				player_base.renderer.material = shoot_material;
			else
				player_base.renderer.material = normal_material;
		}
		
		IncreaseSpeed();
		VerifyShoot();
		VerifyDash();
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