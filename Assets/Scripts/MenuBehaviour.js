var scale = 0.0025;

private var buttons_scale_width = Screen.height * scale;

function OnGUI()
{
	if(GUI.Button(Rect(Screen.width/2 - 35*buttons_scale_width, Screen.height/2 - 20*buttons_scale_width, 70*buttons_scale_width, 25*buttons_scale_width), "Start"))
		Application.LoadLevel(1);
	
	if(GUI.Button(Rect(Screen.width/2 - 35*buttons_scale_width, Screen.height/2 + 50*buttons_scale_width, 70*buttons_scale_width, 25*buttons_scale_width), "Exit"))
		Application.Quit();
}