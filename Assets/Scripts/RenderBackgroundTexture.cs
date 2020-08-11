using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RenderBackgroundTexture : MonoBehaviour {

	void Start() 
	{
		Image gui_texture = GetComponent<Image>();
		//GUITexture gui_texture = this.GetComponent<GUITexture>();

		gui_texture.transform.position = new Vector3(-Screen.width / 2.0f, -Screen.height / 2.0f, Screen.width);
		//gui_texture.pixelInset = new Rect(-Screen.width/2.0f, -Screen.height/2.0f, Screen.width, Screen.height);
	}
}
