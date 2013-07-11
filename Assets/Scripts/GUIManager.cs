using UnityEngine;
using System.Collections;

public class GUIManager {
	
	public Font slam_font ;
	public Material red;
	public Material blue;
	
	private int RED_TEAM = 1;
	private int BLUE_TEAM = 2;
	
	private GUIStyle style_title;
	
	private float native_horizontal_resolution = 1296f;
	private float native_vertical_resolution = 729f;
	private string GOAL_STR = "GOAL!";
	private int GOAL_STR_CHAR_WIDTH = 60;
	private float goal_scored = 0.0f;
	
	public GUIManager(string type)
	{
		style_title = new GUIStyle();
		
		red = Resources.LoadAssetAtPath("Assets/Materials/Player1.mat", typeof (Material)) as Material;
		blue = Resources.LoadAssetAtPath("Assets/Materials/Player2.mat", typeof (Material)) as Material;
		
		if (type == "SPC1") {
			
			style_title.font = Resources.LoadAssetAtPath("Assets/Graphics/Fonts/EraserRegular.ttf", typeof (Font)) as Font;
			style_title.fontSize = 30;
			style_title.alignment = TextAnchor.MiddleCenter;
		
		}
		
		if (type == "MainGame") {
		
			style_title.font = Resources.LoadAssetAtPath("Assets/Graphics/Fonts/EraserRegular.ttf", typeof (Font)) as Font;
			style_title.alignment = TextAnchor.MiddleCenter;
		}
	}
	
	public void DrawScore(int score_team1, int score_team2)
	{
		style_title.fontSize = 75;
		Rect pos = new Rect(native_horizontal_resolution/2 - 10, 20, 10 , 50);
		pos.x -= 50;
		DrawOutline(pos, score_team1.ToString(), red.color, Color.black);
		pos.x += 50;
		DrawOutline(pos, "-", Color.white, Color.black);
		pos.x += 50;
		DrawOutline(pos, score_team2.ToString(), blue.color, Color.black);
	}
	
	public void DrawGoalScored(int team)
	{
		style_title.fontSize = 100;
		Rect pos = new Rect(native_horizontal_resolution/2 - GOAL_STR_CHAR_WIDTH*1/2*(GOAL_STR.Length-1), 90 , 10 , 50);
		Rect temp = pos;
		Color color;
	//	Debug.Log(team);
		if (team == RED_TEAM)
			color = red.color;
		else
			color = blue.color;
		foreach(char ch in GOAL_STR) {
			temp.x = pos.x + Random.Range(-6, 6);
			temp.y = pos.y + Random.Range(-6, 6);
			DrawOutline(temp, ch.ToString() , color, Color.black);
			pos.x += GOAL_STR_CHAR_WIDTH;
		}
	}
	
	private void DrawOutline(Rect pos, string str, Color color, Color outline)
	{
		GUI.matrix = Matrix4x4.TRS (new Vector3(0,0,0),
		Quaternion.identity, new Vector3 (Screen.width / native_horizontal_resolution, Screen.height / native_vertical_resolution, 1));
		
		style_title.normal.textColor = outline;
		pos.x--;
		GUI.Label(pos, str, style_title);
		pos.x += 4;
		GUI.Label(pos, str, style_title);
		pos.x -= 2;
		pos.y -= 2;
		GUI.Label(pos, str, style_title);
		pos.y +=4;
		GUI.Label(pos, str, style_title);
		pos.y--;
		style_title.normal.textColor = color;
		GUI.Label(pos, str, style_title);
	}
	
	public void DrawChallengeTitle(string str)
	{
		Rect pos = new Rect(Screen.width/2 - 10, 20, 10 , 10);
	
		style_title.normal.textColor = Color.black;
		pos.x--;
		GUI.Label(pos, str, style_title);
		pos.x += 4;
		GUI.Label(pos, str, style_title);
		pos.x -= 2;
		pos.y -= 2;
		GUI.Label(pos, str, style_title);
		pos.y +=4;
		GUI.Label(pos, str, style_title);
		pos.y--;
		style_title.normal.textColor = Color.white;
		GUI.Label(pos, str, style_title);
	}
	
//	void Update()
//	{
//		goal_scored += (Time.deltaTime * 10.0);
//	}
}
