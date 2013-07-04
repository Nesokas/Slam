using UnityEngine;
using System.Collections;

public class GUIManager {
	
	public Font slam_font ;
	public Material red;
	public Material blue;
	
	private GUIStyle style_title;
	
	public GUIManager(string type)
	{
		style_title = new GUIStyle();

		if (type == "SPC1") {
			
			style_title.font = Resources.LoadAssetAtPath("Assets/Graphics/Fonts/EraserRegular.ttf", typeof (Font)) as Font;
			style_title.fontSize = 40;
			style_title.alignment = TextAnchor.MiddleCenter;
		
		}
	}
	
	public void DrawChallengeTitle(string str)
	{
		Rect pos = new Rect(Screen.width/2 - 10, 30, 10 , 10);
	
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
	/*
	protected void DrawText(string str, string type)
	{	
		if(type == "challenge_title")
			DrawChallengeTitle(str);
	}
	*/
}
