using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	public const int KEYBOARD = 0;
	public const int JOYSTICK_1 = 1;
	public const int JOYSTICK_2 = 2;
	public const int JOYSTICK_3 = 3;
	public const int JOYSTICK_4 = 4;
	
	public struct Commands 
	{
		public float vertical_direction;
		public float horizontal_direction;
		public float shoot;
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
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(input_num == KEYBOARD) {
			commands.vertical_direction = Input.GetAxis("Vertical");
			commands.horizontal_direction = Input.GetAxis("Horizontal");
			commands.shoot = Input.GetAxis("Shoot");
		} else {
			commands.vertical_direction = Input.GetAxis("Vertical_Gamepad_" + input_num);
			commands.horizontal_direction = Input.GetAxis("Horizontal_Gamepad_" + input_num);
			commands.shoot = Input.GetAxis("Shoot_Gamepad_" + input_num);
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
