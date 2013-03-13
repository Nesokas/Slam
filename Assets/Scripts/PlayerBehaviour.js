#pragma strict

var speed = 0.5;
var decrease_speed = 0.2;
var player_max_speed = 3;

// Directions Vectors
var left_direction = Vector3(0.0, 0.0, 1.0);
var right_direction = Vector3(0.0, 0.0, -1.0);
var up_direction = Vector3(1.0, 0.0, 0.0);
var down_direction = Vector3(-1.0, 0.0, 0.0);

// Movement keys pressed?
var up_keyUp = true; 
var down_keyUp = true; 
var right_keyUp = true;
var left_keyUp = true;

function horizontal_velocity() 
{
	return rigidbody.velocity.z;
}

function vertical_velocity()
{
	return rigidbody.velocity.x;
}


// function to make velocity don't pass the max and to get to 0 if that is the case
function verify_velocity()
{
	if(horizontal_velocity() > player_max_speed)
		rigidbody.velocity = player_max_speed * left_direction;
	else if(horizontal_velocity() < -player_max_speed)
		rigidbody.velocity = player_max_speed * right_direction;
		
	if(vertical_velocity() > player_max_speed)
		rigidbody.velocity = player_max_speed * up_direction;
	else if(vertical_velocity() < -player_max_speed)
		rigidbody.velocity = player_max_speed * down_direction;
	
	if(horizontal_velocity() < decrease_speed && horizontal_velocity() > -decrease_speed)
		rigidbody.velocity.z = 0;
	
	if(vertical_velocity() < decrease_speed && vertical_velocity() > -decrease_speed)
		rigidbody.velocity.x = 0;
}

function increaseSpeed(direction) 
{
	switch (direction) {
	case "right":
		if(horizontal_velocity() > -player_max_speed)
			rigidbody.velocity += right_direction * speed;
		break;
	case "left":
		if(horizontal_velocity() < player_max_speed)
			rigidbody.velocity += left_direction * speed;
		break;
	case "up":
		if(vertical_velocity() < player_max_speed)
			rigidbody.velocity += up_direction * speed;
		break;
	case "down":
		if(vertical_velocity() > -player_max_speed)
			rigidbody.velocity += down_direction * speed;
	}
}

function decreaseVelocity() 
{
	if(right_keyUp && horizontal_velocity() < 0)
		rigidbody.velocity -= right_direction * decrease_speed;
	if(left_keyUp && horizontal_velocity() > 0)
		rigidbody.velocity -= left_direction * decrease_speed;
		
	if(down_keyUp && vertical_velocity() < 0)
		rigidbody.velocity -= down_direction * decrease_speed;
	if(up_keyUp && vertical_velocity() > 0)
		rigidbody.velocity -= up_direction * decrease_speed;
}

function Start () {}

function Update () 
{
	if(Input.GetKey("a")) {
		increaseSpeed("left");
		left_keyUp = false;
	}
	if(Input.GetKey("d")) {
		increaseSpeed("right");
		right_keyUp = false;
	}
	if(Input.GetKey("w")) {
		increaseSpeed("up");
		up_keyUp = false;
	}
	if(Input.GetKey("s")) {
		increaseSpeed("down");
		down_keyUp = false;
	}
	
	if(Input.GetKeyUp("a"))
		left_keyUp = true;
	if(Input.GetKeyUp("d"))
		right_keyUp = true;
	if(Input.GetKeyUp("w"))
		up_keyUp = true;
	if(Input.GetKeyUp("s"))
		down_keyUp = true;
	
	//verify_velocity();
	//decreaseVelocity();
}