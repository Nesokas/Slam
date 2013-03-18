#pragma strict
var score = 0;
var team = 0;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		score++;
		var gui_object = GameObject.FindWithTag("gui");
		var gui_component : Gui = gui_object.GetComponent(Gui);
		
		gui_component.score_team(team);
		
		Debug.Log("Team " + team + " scored.");
	}
}

function Start () {

}

function Update () {

}