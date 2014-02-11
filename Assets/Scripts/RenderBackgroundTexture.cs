using UnityEngine;
using System.Collections;

public class RenderBackgroundTexture : MonoBehaviour {

	void Start() 
	{
		GUITexture gui_texture = this.GetComponent<GUITexture>();

		gui_texture.pixelInset = new Rect(-Screen.width/2.0f, -Screen.height/2.0f, Screen.width, Screen.height);
	}
}
