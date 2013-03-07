#pragma strict

var speed = 10000.0;

function Start () {

}

function Update () 
{
	var directionVertical = Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
	directionVertical = directionVertical.normalized;
	
	if(Input.GetKey("a")) {
		rigidbody.velocity = directionVertical * speed;
	} 
}