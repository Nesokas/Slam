using UnityEngine;
using System.Collections;

public class LED_Screen : MonoBehaviour {
	
	private float planeScale = 10.0f;
	private float DEFAULT_PIXEL_SIZE = 20.4f;
	private float INITIAL_PIXEL_SIZE = 100f;

	private bool is_animating_tie_message = false;
	
	public Texture golden_goal_texture;

//	private void Start()
//	{
//		
//	}

	public void DrawTieMessage()
	{
		renderer.material.SetFloat("_PixelSize", INITIAL_PIXEL_SIZE);
		renderer.material.SetTexture("_MainTex", golden_goal_texture);
		is_animating_tie_message = true;
		
	}

	public void AnimateTieMessage()
	{
		if (renderer.material.GetFloat("_PixelSize") > DEFAULT_PIXEL_SIZE)
			renderer.material.SetFloat("_PixelSize", renderer.material.GetFloat("_PixelSize")-1f);
		else {
			is_animating_tie_message = false;
		}
	}

	private void Update()
	{
		if (is_animating_tie_message)
			AnimateTieMessage();
	}
}
