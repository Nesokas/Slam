using UnityEngine;
using System.Collections;

public class Player_Name : MonoBehaviour {
	
	public Camera m_camera;
	public string player_name;
	
	public void ChangeName(string name)
	{
		TextMesh text_component = (TextMesh)transform.GetComponent("TextMesh");
		text_component.text = name;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_camera != null) {
			transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward,
				m_camera.transform.rotation * Vector3.up);
		}
	}
}
