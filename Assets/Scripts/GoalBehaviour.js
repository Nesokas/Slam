#pragma strict
var score = 0;
var team = 0;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		score++;
		Debug.Log("Team " + team + " scored.");
	}
}

function Start () {

}

function Update () {

}