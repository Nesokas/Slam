using UnityEngine;
using System.Collections;

public class Menu_Behaviour : MonoBehaviour {
	
	public string type;
	
	void OnMouseEnter()
	{
		renderer.material.color = Color.cyan;
	}
	
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
	}
	
	void OnMouseUp()
	{
		if(type == "new_game")
			Application.LoadLevel(1);
		else Application.Quit();
	}
}
