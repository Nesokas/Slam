using UnityEngine;
using System.Collections;

public class LED_Screen : MonoBehaviour {
	
	public Texture golden_goal_texture;
	
	public void DrawTieMessage()
	{
		renderer.material.SetTexture("_MainTex", golden_goal_texture);
	}
}
