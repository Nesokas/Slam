using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public static int KEYBOARD = 0;

	private Game_Settings game_settings;

	public struct Commands 
	{
		public float vertical_direction;
		public float horizontal_direction;
		public float shoot;
		public float dash;
		public float enter;
	}
	
	public Commands commands;
	public int input_num;
	
	// Use this for initialization
	void Awake () 
	{	
		commands = new Commands();
		commands.vertical_direction = 0;
		commands.horizontal_direction = 0;
		commands.shoot = 0;
		commands.dash = 0;

		GameObject settings = GameObject.FindGameObjectWithTag("settings");
		game_settings = (Game_Settings)settings.GetComponent<Game_Settings>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (game_settings.IsLocalGame() && input_num > KEYBOARD){ //which means, if it's a human locally playing with a gamepad

			commands.vertical_direction = Input.GetAxis("Vertical_Gamepad_" + input_num);
			commands.horizontal_direction = Input.GetAxis("Horizontal_Gamepad_" + input_num);
			commands.shoot = Input.GetAxis("Shoot_Gamepad_" + input_num);
			commands.dash = Input.GetAxis("Dash_Gamepad_" + input_num);
			commands.enter = Input.GetAxis("Shoot_Gamepad_" + input_num);
		} else if(input_num == KEYBOARD) {

			commands.vertical_direction = Input.GetAxis("Vertical");
			commands.horizontal_direction = Input.GetAxis("Horizontal");
			commands.shoot = Input.GetAxis("Shoot");
			commands.dash = Input.GetAxis("Dash");
			commands.enter = Input.GetAxis("Shoot");
		//	Debug.Log(Input.GetAxis("Horizontal"));
		} 
	}

	public void SetVerticalDirection(int direction)
	{
		commands.vertical_direction = direction;
	}

	public void SetHorizontalDirection(int direction)
	{
		commands.horizontal_direction = direction;
	}
	
	public Commands GetCommands()
	{
		return commands;
	}
	
	public void setInputNum(int number)
	{
		input_num = number;
	}
}
