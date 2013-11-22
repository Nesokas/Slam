using UnityEngine;
using System.Collections;

public class LED_Screen : MonoBehaviour {
	
	private float planeScale = 10.0f;
	
	public Texture golden_goal_texture;
	
	public void DrawTieMessage()
	{
		renderer.material.SetTexture("_MainTex", golden_goal_texture);
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			Vector3 v3 = new Vector3(planeScale*transform.localScale.x, planeScale*transform.localScale.y, transform.position.z);
			v3 = Camera.main.WorldToScreenPoint(v3);
			Vector3 v3Zero = Camera.main.WorldToScreenPoint(Vector3.zero);
			v3 = v3 - v3Zero;
			Debug.Log("Image screen size: " + Screen.width + " " + Screen.height);
		}
	}
}
