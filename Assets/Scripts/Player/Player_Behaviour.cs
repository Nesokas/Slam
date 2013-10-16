using UnityEngine;
using System.Collections;

public class Player_Behaviour : MonoBehaviour {
	
	private float DASH_COOLDOWN = 12f;
	private float DASH_STRENGTH = 2f;
	public float ACCELERATION = 0.065f;
	public float MAX_ANIMATION_SPEED = 2f;
	public float SHOOT_VELOCITY = 9;
	
	public float increase_speed = 0.1f;
	private float dash_cooldown;

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
	
	public GameObject player_controller_prefab;

	protected Vector3 direction = Vector3.zero;
	protected bool ball_collision = false;
	protected Material shoot_material;
	protected Material normal_material;

	protected bool colliding_with_ball = false;
	protected Collider ball_collider;
	protected Transform player_base;
	protected Transform dash_bar;
	protected Transform dash_bar_fill;
	protected Transform player_indicator_container;
	public Vector3 viewport_dash_pos;
	protected GameObject ball;

	protected bool debug_key_pressed = false;
	protected bool debug_mode = false;
	protected bool hit = false;
	protected Vector3 last_ball_position;
	
	protected bool is_adding_speed = false;
	protected float animation_speed = 1f;
	
	private float NATIVE_HORIZONTAL_RESOLUTION = 1296f;
	private float NATIVE_VERTICAL_RESOLUTION = 729f;
	
	protected PlayerController.Commands commands;
	
	private float PLAYER_ARROW_SIZE = 0.012f;
    private float PLAYER_ARROW_HEIGHT = 0.02f;

	void VerifyShoot()
	{
		if(colliding_with_ball && !ball_collision && move){

			if(!ball)
				ball = GameObject.FindGameObjectWithTag("ball");

			Vector3 direction = ball.transform.position - transform.position;
			direction.Normalize();

			if((!gamepad && commands.shoot != 0)){
				ball_collider.rigidbody.velocity += direction * SHOOT_VELOCITY;
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
//		Debug.Log(dash_cooldown + " " + Time.time);
		if (commands.dash != 0 && (Time.time > dash_cooldown) && (commands.horizontal_direction != 0 || commands.vertical_direction != 0)) {
			dash_cooldown =  DASH_COOLDOWN + Time.time;
			rigidbody.velocity *= DASH_STRENGTH;
//			StartCoroutine(DrawDashTrail());
			dash_bar_fill.renderer.material.color = Color.red;
		}
		UpdateDashBarFill();
	}
	
//	protected IEnumerator DrawDashTrail()
//	{
//		Debug.Log("DASHING");
//		dash_trail.particleEmitter.emit = true;
//		yield return new WaitForSeconds(0.2f);
//		dash_trail.particleEmitter.emit = false;
//	}
	
	void UpdateDashBarFill()
	{
		float current_value = DASH_COOLDOWN - (dash_cooldown-Time.time);
		if (current_value < 0)
			current_value = DASH_COOLDOWN;
		else if (current_value > DASH_COOLDOWN) {
			current_value = DASH_COOLDOWN;
			dash_bar_fill.renderer.material.color = Color.green;
		}
		float fill_percent = current_value/DASH_COOLDOWN;

		dash_bar_fill.localScale = new Vector3(1f, 1f,fill_percent);
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
	
			rigidbody.velocity += direction*ACCELERATION;
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
			if(animation_speed < MAX_ANIMATION_SPEED)
				animation_speed += increase_speed;
			else if(animation_speed > MAX_ANIMATION_SPEED)
				animation_speed = MAX_ANIMATION_SPEED;
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
				animation.CrossFade("Idle", 0.3f);
				break;
			case "Celebrate":
				animation.CrossFade("Celebrate", 0.3f);
				break;
			case "Sad":
				animation.CrossFade("Sad", 0.3f);
				break;
		}
	}
	
	protected void Start() 
	{		
		GameObject court_walls = GameObject.FindGameObjectWithTag("court_walls");
		GameObject[] goal_detection = GameObject.FindGameObjectsWithTag("goal_detection");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		player_base = transform.Find("Base");
		dash_bar = transform.Find("Dash_Bar");
		dash_bar_fill = dash_bar.Find("Dash_Fill");
		
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
	
	protected 
	void Awake()
	{
		player_indicator_container = transform.Find("Player_Indicator_Container");
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
	
	private void UpdatePlayerIndicator()
	{
		//player_indicator_container.rotation = Quaternion.LookRotation(Camera.main.transform.position - player_indicator_container.position);
		
	}
	
	public Texture indicator_arrow;
	public Texture dash_bar_outline;
	public Texture dash_bar_green;
	public Texture dash_bar_red;
	
	void OnGUI()
	{
		float x=0;
		float y=0;
		float width=0;
		float height=0;
		
		Vector2 player_indicator_position = Camera.main.WorldToViewportPoint(dash_bar.position);
		
		if (player_indicator_position.x < 0 && player_indicator_position.y < 0) {
		
			y = Screen.height*0.99f - Screen.height*PLAYER_ARROW_SIZE;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(45f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
		
		} else if (player_indicator_position.x < 0 && player_indicator_position.y > 1) {
		
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(135f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
			
		} else if (player_indicator_position.x > 1 && player_indicator_position.y < 0) {
			y = Screen.height*0.99f - Screen.height*PLAYER_ARROW_SIZE;
			x = Screen.width - Screen.height*PLAYER_ARROW_HEIGHT;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(-45f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
			
		} else if (player_indicator_position.x > 1 && player_indicator_position.y > 1) {
			x= Screen.width - Screen.height*PLAYER_ARROW_HEIGHT;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(-135f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
			
		}
			
		else if (player_indicator_position.x < 0) {
			x = Screen.height*PLAYER_ARROW_HEIGHT;
			y = (1 - player_indicator_position.y) * Screen.height - Screen.width*PLAYER_ARROW_SIZE/2f - Screen.height*PLAYER_ARROW_HEIGHT;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(90f, new Vector2(x, y));
			
		
		} else if (player_indicator_position.x > 1) {
			x = Screen.width - Screen.height*PLAYER_ARROW_HEIGHT;
			y = (1 - player_indicator_position.y)* Screen.height - Screen.width*PLAYER_ARROW_SIZE/2f - Screen.height*PLAYER_ARROW_HEIGHT;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(-90f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
			
		} else if (player_indicator_position.y > 1) {
			x = (player_indicator_position.x * Screen.width - Screen.width*PLAYER_ARROW_SIZE/2f);
			y = 0;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
			GUIUtility.RotateAroundPivot(180f, new Vector2(x+Screen.width*PLAYER_ARROW_SIZE/2f, y+Screen.width*PLAYER_ARROW_SIZE/2f));
		
		} else if (player_indicator_position.y < 0) {
			x = (player_indicator_position.x * Screen.width - Screen.width*PLAYER_ARROW_SIZE/2f);
			y = Screen.height*0.99f - Screen.height*PLAYER_ARROW_SIZE;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
		}
		
		else {
			x = (player_indicator_position.x * Screen.width - Screen.width*PLAYER_ARROW_SIZE/2f);
			y = (1 - player_indicator_position.y) * Screen.height - Screen.width*PLAYER_ARROW_SIZE/2f - Screen.height*PLAYER_ARROW_HEIGHT;
			width = Screen.width*PLAYER_ARROW_SIZE;
			height = Screen.width*PLAYER_ARROW_SIZE;
		}
		
		GUI.DrawTexture( new Rect(x, y, width, height), indicator_arrow,	ScaleMode.ScaleToFit, true);
			
// FOR MAKING THE DASHBAR WITH TEXTURES
//		float dash_bar_width = 50;
//		float dash_bar_height = 5;
//		
//		
//		GUI.DrawTexture(new Rect(player_indicator_position.x * Screen.width - dash_bar_width/2f, (1 - player_indicator_position.y) * Screen.height - dash_bar_height/2f - dash_bar_position_heigth, dash_bar_width, dash_bar_height), 
//						dash_bar_outline, 
//						ScaleMode.StretchToFill, 
//	
	}

	// Update is called once per frame
	protected void FixedUpdate () 
	{
		/* TODO: Uma forma mais inteligente de fazer isto */
		if (player_base == null)
			player_base = transform.Find("Base");
		/***************************************************/
		
		if (dash_bar_fill == null)
			dash_bar_fill = transform.Find("Dash_Fill");
		
		dash_bar.rotation = Quaternion.Euler(0,180f,0);
		viewport_dash_pos = Camera.main.WorldToViewportPoint(dash_bar.position);
		
//		Debug.Log(Camera.main.WorldToViewportPoint(dash_bar.position));
		
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
		UpdatePlayerIndicator();
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