using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public const int KEYBOARD = 0;
	public const int JOYSTICK_1 = 1;
	public const int JOYSTICK_2 = 2;
	public const int JOYSTICK_3 = 3;
	public const int JOYSTICK_4 = 4;

	private Game_Settings game_settings;

	public struct Commands 
	{
		public float vertical_direction;
		public float horizontal_direction;
		public float shoot;
		public float dash;
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
		if (game_settings.IsLocalGame())
			if(input_num == KEYBOARD) {
				commands.vertical_direction = Input.GetAxis("Vertical");
				commands.horizontal_direction = Input.GetAxis("Horizontal");
				commands.shoot = Input.GetAxis("Shoot");
				commands.dash = Input.GetAxis("Dash");
			} else {
				commands.vertical_direction = Input.GetAxis("Vertical_Gamepad_" + input_num);
				commands.horizontal_direction = Input.GetAxis("Horizontal_Gamepad_" + input_num);
				commands.shoot = Input.GetAxis("Shoot_Gamepad_" + input_num);
				commands.dash = Input.GetAxis("Dash_Gamepad_" + input_num);
			}
		else {
			input_num = 1;
			commands.vertical_direction = Input.GetAxis("Vertical_Gamepad_" + input_num) + Input.GetAxis("Vertical");
			commands.horizontal_direction = Input.GetAxis("Horizontal_Gamepad_" + input_num) + Input.GetAxis("Horizontal");
			commands.shoot = Input.GetAxis("Shoot_Gamepad_" + input_num) + Input.GetAxis("Shoot");
			commands.dash = Input.GetAxis("Dash_Gamepad_" + input_num) + Input.GetAxis("Dash");
		}
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
