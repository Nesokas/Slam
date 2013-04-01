#pragma strict

var acceleration = 0.3;
var player_max_speed = 4;
var shootVelocity = 5.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

private var direction = Vector3.zero;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		if(Input.GetAxis("Shoot") && !gamepad){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
		}
		else if(Input.GetAxis("Shoot_Gamepad") && gamepad){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
		}
	}
}

function OnCollisionStay(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		if(Input.GetAxis("Shoot") && !gamepad){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
		}
		else if(Input.GetAxis("Shoot_Gamepad") && gamepad){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
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
}

function Update () 
{
	increase_speed();
}