#pragma strict

var players_team_1 : Array;
var players_team_2 : Array;

function Awake () {
	DontDestroyOnLoad (transform.gameObject);
	players_team_1 = new Array();
	players_team_2 = new Array();
}

function AddNewPlayer(team : int, name : String)
{
	if(team == 1)
		players_team_1.Push(name);
	else 
		players_team_2.Push(name);
}
