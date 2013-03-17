#pragma strict

var acceleration = 0.3;
var player_max_speed = 4;
var shootVelocity = 5.0;

var velocity = Vector3.zero;
var normalized_velocity = Vector3.zero;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		if(Input.GetAxis("Shoot")){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
			var script : BallBehaviour = collision.gameObject.GetComponent(BallBehaviour);
			script.shoot();
		}
	}
}

function OnCollisionStay(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		if(Input.GetAxis("Shoot")){
			collision.rigidbody.velocity -= collision.contacts[0].normal * shootVelocity;
			var script : BallBehaviour = collision.gameObject.GetComponent(BallBehaviour);
			script.shoot();
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
	
	var vertical_speed : float = Input.GetAxis("Vertical")*acceleration;
	var horizontal_speed : float = Input.GetAxis("Horizontal")*acceleration;
	
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
	Physics.IgnoreCollision(court_walls.collider, this.collider);
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
		
	Debug.Log(rigidbody.velocity);

}