var score_team1 : int = 0;
var score_team2 : int = 0;

function score_team(team:int)
{
	if(team == 1)
		score_team1++;
	else
		score_team2++;
}

function OnGUI() 
{
	var score = score_team2 + " - " + score_team1;
	
	GUI.color = Color.black;
	
	var score_style : GUIStyle = GUI.skin.GetStyle("Label");
	score_style.alignment = TextAnchor.UpperCenter;
	score_style.fontSize = 30;
	score_style.fontStyle = FontStyle.Bold;
	GUI.Label(Rect(Screen.width/2, 10, 100, 50), score, score_style);
}