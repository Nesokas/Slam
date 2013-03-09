#pragma strict

var speed = 0.001;
var player_max_speed = 0.5;

// Directions Vectors
var left = Vector3(0.0, 0.0, -1.0);
var right = Vector3(0.0, 0.0, 1.0);
var up = Vector3(1.0, 0.0, 0.0);
var down = Vector3(-1.0, 0.0, 0.0);

function increaseSpeed(direction) {
	switch (direction) {
	case "right":
		if(rigidbody.velocity.z >= -player_max_speed)
			rigidbody.velocity += left * speed;
		break;
	case "left":
		if(rigidbody.velocity.z <= player_max_speed)
			rigidbody.velocity -= right * speed;
		break;
	case "up":
		if(rigidbody.velocity.x <= player_max_speed)
			rigidbody.velocity += up*speed;
		break;
	case "down":
		if(rigidbody.velocity.x >= -player_max_speed)
			rigidbody.velocity += down * speed;
	}
}

function Start () {

}

function Update () 
{
	if(Input.GetKey("a"))
		increaseSpeed("left");
	
	if(Input.GetKey("d"))
		increaseSpeed("right");
		
	if(Input.GetKey("w"))
		increaseSpeed("up");
	
	if(Input.GetKey("s"))
		increaseSpeed("down");
}