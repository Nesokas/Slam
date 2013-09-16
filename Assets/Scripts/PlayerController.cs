using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public struct Commands 
	{
		public float vertical_direction;
		public float horizontal_direction;
		public float shoot;
		public float dash;
	}
	
	public Commands commands;
	
	// Use this for initialization
	void Awake () 
	{	
		commands = new Commands();
		commands.vertical_direction = 0;
		commands.horizontal_direction = 0;
		commands.shoot = 0;
		commands.dash = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		commands.vertical_direction = Input.GetAxis("Vertical");
		commands.horizontal_direction = Input.GetAxis("Horizontal");
		commands.shoot = Input.GetAxis("Shoot");
		commands.dash = Input.GetAxis("Dash");
	}
	
	public Commands GetCommands()
	{
		return commands;
	}
}
