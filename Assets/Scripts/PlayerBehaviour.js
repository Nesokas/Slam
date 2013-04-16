#pragma strict

var acceleration = 0.2;
var shootVelocity = 11.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

var team = 1;
var player_name = "Ines";
var player_num = 1;

var normal_team_1_material : Material;
var normal_team_2_material : Material;
var shoot_team_1_material : Material;
var shoot_team_2_material : Material;

private var direction = Vector3.zero;
private var ball_collision = false;
private var shoot_material : Material;
private var normal_material : Material;
var gamepad_num : int = 0;
private var colliding_with_ball = false;
private var ball_collider : Collider;
private var base : Transform;
private var ball : GameObject;

private var debug_key_pressed = false;
private var debug_mode = false;
private var hit = false;
private var last_ball_position : Vector3;

var debug_hit_remaining_time = 0;
var debug_hit_time = 30;


function InitializePlayerInfo(num:int, team_num:int, m_camera : Camera)
{
	if(num != 0) {
		gamepad_num = num;
		gamepad = true;
	}
	team = team_num;
	player_num = num;
	
	var name_component = transform.Find("Player_name").transform.GetComponent(Player_Name);
	name_component.m_camera = m_camera;
	name_component.ChangeName("P"+num);
	
	Start();
}

function VerifyShoot()
{
	if(colliding_with_ball && !ball_collision){
	
		if(!ball)
			ball = GameObject.FindGameObjectWithTag("ball");
			
		var contact_point = ball_collider.ClosestPointOnBounds(transform.position);
		var direction = ball.transform.position - transform.position;
		direction.Normalize();
		
		if((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))){
			ball_collider.rigidbody.velocity += direction * shootVelocity;
			ball_collision = true;
			colliding_with_ball = false;
			debug_hit_remaining_time = debug_hit_time;
		}
	}
}

/*function OnCollisionEnter(collision : Collision)
{	
	if(collision.gameObject.name == "Ball" || collision.gameObject.tag == "colliderShoot") {
		colliding_with_ball = true;
		ball_collider = collision;
		Debug.Log("----------------");
	}
}

function OnCollisionExit(collision : Collision)
{
	if(collision.gameObject.name == "Ball" || collision.gameObject.tag == "colliderShoot") {
		colliding_with_ball = false;
	}
}*/

function OnTriggerEnter (collider : Collider) {
    
    if(collider.gameObject.name == "Ball" || collider.gameObject.tag == "colliderShoot") {
		colliding_with_ball = true;
		ball_collider = collider;
	}
}

function OnTriggerExit (collider : Collider) {
    
    if(collider.gameObject.name == "Ball" || collider.gameObject.tag == "colliderShoot") {
		colliding_with_ball = false;
	}
}


function increase_speed() 
{
	if(gamepad){
		direction.x = Input.GetAxis("Vertical_Gamepad_" + gamepad_num);
		direction.z = Input.GetAxis("Horizontal_Gamepad_" + gamepad_num);
	} else {
		direction.x = Input.GetAxis("Vertical");
		direction.z = Input.GetAxis("Horizontal");
	}
	direction.Normalize();
		
	rigidbody.velocity += direction*acceleration;
}

function updateRotation()
{
	if(!ball)
		ball = GameObject.FindGameObjectWithTag("ball");
	var rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
}



function Start() {
	
	var court_walls = GameObject.FindGameObjectWithTag("court_walls");
	var goal_detection = GameObject.FindGameObjectsWithTag("goal_detection");
	var players = GameObject.FindGameObjectsWithTag("Player");
	base = transform.Find("Base");
	var base_collider = base.Find("Collider");
	var shoot_collider = base.Find("ColliderShoot");
	Physics.IgnoreCollision(court_walls.collider, base_collider.collider);
	Physics.IgnoreCollision(court_walls.collider, shoot_collider.collider);
	for(var i = 0; i < goal_detection.Length; i++) {
		Physics.IgnoreCollision(goal_detection[i].collider, base_collider.collider);
	}
	
	for (i=0; i < players.Length; i++) {
		var player_base = players[i].transform.Find("Base");
		var player_shoot_collider = player_base.transform.Find("ColliderShoot");
		if(player_shoot_collider.collider != shoot_collider.collider) {
			Physics.IgnoreCollision(player_shoot_collider.collider, shoot_collider.collider);
			Physics.IgnoreCollision(player_shoot_collider.collider, base_collider.collider);
		}
	}
	
	if(team == 1) {
		normal_material = normal_team_1_material;
		shoot_material = shoot_team_1_material;
	} else {
		normal_material = normal_team_2_material;
		shoot_material = shoot_team_2_material;
	}

	base.renderer.material = normal_material;
}

function DebugMode() 
{
	if(!ball)
		ball = GameObject.FindGameObjectWithTag("ball");
	
	var direction : Vector3;
	if(hit)
		direction = last_ball_position - transform.position;
	else 
		direction = ball.transform.position - transform.position;
		
	direction.Normalize();
	
	Debug.DrawLine(transform.position, transform.position + direction*3, Color.yellow);	
}

function Update ()
{
	if(base == null)
		base = transform.Find("Base");
		
	if(ball_collision && !((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num))))) {
		ball_collision = false;
	}
	
	if(!ball_collision && ((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))))
		base.renderer.material = shoot_material;
	else
		base.renderer.material = normal_material;
	
	increase_speed();

	VerifyShoot();
		
	updateRotation();
	
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