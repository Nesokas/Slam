#pragma strict

var max_velocity = 3;
var has_shoot = false;

function horizontal_velocity() 
{
	return rigidbody.velocity.z;
}

function vertical_velocity()
{
	return rigidbody.velocity.x;
}

function shoot()
{
	has_shoot = true;
}

function Update () 
{
	/*if(!has_shoot) 
	{
		if(horizontal_velocity() > max_velocity)
			rigidbody.velocity.z = max_velocity;
		else if(horizontal_velocity() < -max_velocity)
			rigidbody.velocity.z = -max_velocity;
			
		if(vertical_velocity() > max_velocity)
			rigidbody.velocity.x = max_velocity;
		else if(vertical_velocity() < -max_velocity)
			rigidbody.velocity.x = -max_velocity;
	} 
	else if (rigidbody.velocity.x <= max_velocity || 
			   rigidbody.velocity.z <= max_velocity ||
			   rigidbody.velocity.x >= -max_velocity || 
			   rigidbody.velocity.z >= -max_velocity) 
	{
		has_shoot = false;
	}*/
}