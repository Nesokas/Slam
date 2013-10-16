using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
	public Font slam_font ;
	
	private int GOAL_STR_CHAR_WIDTH = 60;
	private int GOAL_STR_CHAR_SIZE = 100;
	
	private int DRAW_STR_CHAR_WIDTH = 30;
	private int DRAW_STR_CHAR_SIZE = 50;
	
	public Material red;
	public Material blue;
	
	private int RED_TEAM = 1;
	private int BLUE_TEAM = 2;
	
	private GUIStyle style_title;
	
	private float NATIVE_HORIZONTAL_RESOLUTION = 1296f;
	private float NATIVE_VERTICAL_RESOLUTION = 729f;
	private float goal_scored = 0.0f;
	
	public void Awake()
	{
		style_title = new GUIStyle();
		
		/*red = Resources.LoadAssetAtPath("Assets/Materials/Player1.mat", typeof (Material)) as Material;
		blue = Resources.LoadAssetAtPath("Assets/Materials/Player2.mat", typeof (Material)) as Material;
		
		if (type == "SPC1") {
			
			style_title.font = Resources.LoadAssetAtPath("Assets/Graphics/Fonts/EraserRegular.ttf", typeof (Font)) as Font;
			style_title.fontSize = 30;
			style_title.alignment = TextAnchor.MiddleCenter;
		
		}
		
		if (type == "MainGame") {
		
			style_title.font = Resources.LoadAssetAtPath("Assets/Graphics/Fonts/EraserRegular.ttf", typeof (Font)) as Font; */
			style_title.alignment = TextAnchor.MiddleCenter;
		//}
		
	 	style_title.font = slam_font;
	}
/*	
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
	*/
	
	public void DrawGoalScored(int team, string str)
	{
		style_title.fontSize = GOAL_STR_CHAR_SIZE;
		Rect pos = new Rect(NATIVE_HORIZONTAL_RESOLUTION/2 - GOAL_STR_CHAR_WIDTH*1/2*(str.Length-1), 90 , 10 , 50);
		Rect temp = pos;
		Color color;
	//	Debug.Log(team);
		if (team == RED_TEAM)
			color = red.color;
		else
			color = blue.color;
		foreach(char ch in str) {
			temp.x = pos.x + Random.Range(-6, 6);
			temp.y = pos.y + Random.Range(-6, 6);
			DrawOutline(temp, ch.ToString() , color, Color.black);
			pos.x += GOAL_STR_CHAR_WIDTH;
		}
	}
	
	/* It will display the appropriate message - "Draw", or "Golden Goal" */
	public void DrawTieMessage(string str)
	{
		style_title.fontSize = DRAW_STR_CHAR_SIZE;
		Rect pos = new Rect(NATIVE_HORIZONTAL_RESOLUTION/6 - DRAW_STR_CHAR_WIDTH*1/2*(str.Length-1), 45 , 10 , 50);
		foreach(char ch in str) {
			DrawOutline(pos, ch.ToString() , Color.yellow, Color.black);
			pos.x += DRAW_STR_CHAR_WIDTH;
		}
	}
	
	private void DrawOutline(Rect pos, string str, Color color, Color outline)
	{
		GUI.matrix = Matrix4x4.TRS (new Vector3(0,0,0),
		Quaternion.identity, new Vector3 (Screen.width / NATIVE_HORIZONTAL_RESOLUTION, Screen.height / NATIVE_VERTICAL_RESOLUTION, 1));
		
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
