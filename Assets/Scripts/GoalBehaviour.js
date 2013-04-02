#pragma strict
var score = 0;
var team = 0;

function OnCollisionEnter(collision : Collision)
{
	if(collision.gameObject.name == "Ball"){
		score++;
		var main_game_object = GameObject.FindWithTag("GameController");
		var main_game_component : GameBehaviour = main_game_object.GetComponent(GameBehaviour);
		
		main_game_component.score_team(team);
	}
}

function Start () {

}

function Update () {

}