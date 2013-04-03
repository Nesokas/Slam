#pragma strict

var acceleration = 0.3;
var player_max_speed = 4;
var shootVelocity = 8.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

var team = 1;
var player_name = "Ines";
var player_num = 1;

private var direction = Vector3.zero;
private var ball_collision = false;
private var shoot_material : Material;
private var normal_material : Material;
var gamepad_num : int = 0;
private var colliding_with_ball = false;
private var ball_collider : Collision;

function AddGamepadNum(num:int)
{
	gamepad_num = num;
	gamepad = true;
}

function VerifyShoot()
{
	if(!ball_collision && colliding_with_ball){
		if((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))){
			ball_collider.rigidbody.velocity -= ball_collider.contacts[0].normal * shootVelocity;
			ball_collision = true;
			colliding_with_ball = false;
		}
	}
}

function OnCollisionEnter(collision : Collision)
{
	colliding_with_ball = true;
	ball_collider = collision;
}

function OnCollisionExit(collision : Collision)
{
	colliding_with_ball = false;
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

function Start () {

	var court_walls = GameObject.FindGameObjectWithTag("court_walls");
	var goals = GameObject.FindGameObjectsWithTag("goal");
	Physics.IgnoreCollision(court_walls.collider, this.collider);

	for(var i = 0; i < goals.Length; i++)
		Physics.IgnoreCollision(goals[i].collider, this.collider);
		
	if(team == 1) {
		normal_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player1.mat", typeof(Material)) as Material;
		shoot_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player1_shoot.mat", typeof(Material)) as Material;
	} else {
		normal_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player2.mat", typeof(Material)) as Material;
		shoot_material = AssetDatabase.LoadAssetAtPath("Assets/Materials/Player2_shoot.mat", typeof(Material)) as Material;
	}
	
	renderer.material = normal_material;
}

function Update ()
{
	if(ball_collision && !((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num))))) {
		ball_collision = false;
	}
	
	if(!ball_collision && ((!gamepad && (Input.GetAxis("Shoot"))) || (gamepad && (Input.GetAxis("Shoot_Gamepad_" + gamepad_num)))))
		renderer.material = shoot_material;
	else
		renderer.material = normal_material;
	
	increase_speed();
	VerifyShoot();
}