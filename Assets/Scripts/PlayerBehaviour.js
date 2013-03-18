#pragma strict

var acceleration = 0.3;
var player_max_speed = 4;
var shootVelocity = 3.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;
var gamepad = false;

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

function horizontal_velocity() 
{
	return rigidbody.velocity.z;
}

function vertical_velocity()
{
	return rigidbody.velocity.x;
}

function increase_speed() 
{
	var vertical_speed : float;
	var horizontal_speed : float;
	if(gamepad){
		vertical_speed = Input.GetAxis("Vertical_Gamepad")*acceleration;
		 horizontal_speed = Input.GetAxis("Horizontal_Gamepad")*acceleration;
	} else {
		vertical_speed = Input.GetAxis("Vertical")*acceleration;
		horizontal_speed = Input.GetAxis("Horizontal")*acceleration;
	}
	
	rigidbody.velocity.z -= horizontal_speed;
	rigidbody.velocity.x += vertical_speed;
	
	if(rigidbody.velocity.x < -player_max_speed)
		rigidbody.velocity.x = -player_max_speed;
	else if (rigidbody.velocity.x > player_max_speed)
		rigidbody.velocity.x = player_max_speed;
		
	if(rigidbody.velocity.z < -player_max_speed)
		rigidbody.velocity.z = -player_max_speed;
	else if (rigidbody.velocity.z > player_max_speed)
		rigidbody.velocity.z = player_max_speed;
}

function normalize_velocity()
{
	var degree = Mathf.Atan2(Mathf.Abs(horizontal_velocity()), 
							 Mathf.Abs(vertical_velocity()));
	normalized_velocity.x = rigidbody.velocity.x * Mathf.Cos(degree);
	normalized_velocity.z = rigidbody.velocity.z * Mathf.Sin(degree); 
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

	if (Input.GetAxis("Vertical")!=0)
		rigidbody.velocity.x = velocity.x;
		
	if (Input.GetAxis("Horizontal")!=0)
		rigidbody.velocity.z = velocity.z;
		
	increase_speed();
	
	normalize_velocity();
	velocity = rigidbody.velocity;
	if (Input.GetAxis("Vertical")!=0)
		rigidbody.velocity.x = normalized_velocity.x;
		
	if (Input.GetAxis("Horizontal")!=0)
		rigidbody.velocity.z = normalized_velocity.z;
		
	//Debug.Log(rigidbody.velocity);

}