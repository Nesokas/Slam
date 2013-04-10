#pragma strict

private var num_players_team_1 = 1;
private var num_players_team_2 = 1;
private var controllers_assigned[5];
private var players_team_1[];
private var players_team_2[];
private var keyboard = 0;
private var controller_1 = 1;
private var controller_2 = 2;
private var controller_3 = 3;
private var controller_4 = 4;

function Awake () 
{
    DontDestroyOnLoad(transform.gameObject);
}

function AddPlayerTeam(num:int)
{
	var controller_to_assign : int;
	for (var i = 0; i < 5; i++) {
		if(!controllers_assigned[i]) {
			controller_to_assign = i;
			controllers_assigned[i] = true;
			break;
		}
	}
	
	if(num == 1 && num_players_team_1 == 1) {
		num_players_team_1 = 2;
		players_team_1.push(controller_to_assign)
	}
	else if (num == 2 && num_players_team_2 == 1) {
		num_players_team_2 = 2;
		players_team_2.push(controller_to_assign)
	}
}

function RemovePlayerTeam(num:int)
{
	if(num == 1 && num_players_team_1 == 2) {
		num_players_team_1 = 1;
		if(players_team_1[0] == controllers_assigned[keyboard])
			keyboard_assigned = false;
		
	}
	else if (num == 2 && num_players_team_2 == 2) {
		num_players_team_2 = 1;
		if(players_team_1[0] == controllers_assigned[keyboard])
			keyboard_assigned = false;
	}
}

function Start () 
{
	for(var i = 0; i < 5; i++)
		controllers_assigned[i] = false;
}

function Update () 
{

}