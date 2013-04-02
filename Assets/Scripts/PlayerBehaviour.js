#pragma strict

var acceleration = 0.3;
var player_max_speed = 4;
var shootVelocity = 8.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

var team = 1;
var player_name = "Ines";

private var direction = Vector3.zero;
private var ball_collision = false;
private var shoot_material : Material;
private var normal_material : Material;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball" && !ball_collision){
		if((Input.GetAxis("Shoot") && !gamepad) || (Input.GetAxis("Shoot_Gamepad") && gamepad)){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
			ball_collision = true;
		}
	}
}

function OnCollisionStay(collision : Collision)
{
	if(collision.gameObject.name == "Ball" && !ball_collision){
		if((Input.GetAxis("Shoot") && !gamepad) || (Input.GetAxis("Shoot_Gamepad") && gamepad)){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
			ball_collision = true;
		}
	}
}

function increase_speed() 
{
	var vertical_speed : float;
	var horizontal_speed : float;
	if(gamepad){
		direction.x = Input.GetAxis("Vertical_Gamepad");
		direction.z = Input.GetAxis("Horizontal_Gamepad");
		Debug.Log(direction);
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
	if(!((Input.GetAxis("Shoot") && !gamepad) || (Input.GetAxis("Shoot_Gamepad") && gamepad)) &&
		ball_collision) {
		ball_collision = false;
	}
	
	if(((Input.GetAxis("Shoot") && !gamepad) || (Input.GetAxis("Shoot_Gamepad") && gamepad)) && !ball_collision)
		renderer.material = shoot_material;
	else
		renderer.material = normal_material;
	
	increase_speed();
}