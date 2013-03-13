#pragma strict

var max_velocity = 3;

function horizontal_velocity() 
{
	return rigidbody.velocity.z;
}

function vertical_velocity()
{
	return rigidbody.velocity.x;
}

function Update () {
	if(horizontal_velocity() > max_velocity)
		rigidbody.velocity.z = max_velocity;
	else if(horizontal_velocity() < -max_velocity)
		rigidbody.velocity.z = -max_velocity;
		
	if(vertical_velocity() > max_velocity)
		rigidbody.velocity.x = max_velocity;
	else if(vertical_velocity() < -max_velocity)
		rigidbody.velocity.x = -max_velocity;
}