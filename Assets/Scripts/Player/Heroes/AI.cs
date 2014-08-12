using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : Hero {

	private AIManager ai_manager;
//	private PlayerController controller;
	Local_Player player;
	private GameObject ball;

	private const int BOTTOM_FLANK = 0, MID_FLANK = 1, TOP_FLANK = 2, LEFT = 3, RIGHT = 4;

	private const int MOVE_UP = 1, MOVE_DOWN = -1, MOVE_LEFT = 1, MOVE_RIGHT = -1;

	private bool has_ball = false;

	private float distance_to_ball = 0;
	private float possession_distance_threshold = 1f;

	private GameObject sphere;
//	private struct beliefs {
//	
//		private Dictionary teammates_positions;
//		private Dictionary 
//	
//	}

	public AI(Player_Behaviour player)
	{
		hero_prefab = Resources.Load<GameObject>("Heroes/Sam");
		ai_manager = GameObject.Find("AIManager").GetComponent<AIManager>();
		this.player = (Local_Player)player;
		player.SetIsAI(true);
		ball = GameObject.FindGameObjectWithTag("ball");

		//ai_manager.PrintPitchAreaCords();
	}
	// Use this for initialization
	public override void Start () {
//		controller = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
		ai_manager.InsertAI(this);
	}

	public void Update () 
	{
//		distance_to_ball = Vector3.Distance (ball.transform.position, player.transform.position);
//
//		if (has_ball == false)
//			GoToBall();
//
//		if (distance_to_ball < possession_distance_threshold) {
//			has_ball = true;
//			ResetControllers();
//		} else {
//			has_ball = false;
//		}
//
//		Debug.Log(has_ball);

		DribbleToArea(0);
	}

	private void DribbleToArea(int index)
	{
		RaycastHit hit;

//		if (Physics.Raycast (ball.transform.position, ai_manager.GetPitchAreaCoords(index) - ball.transform.position, out hit)) {
//		
//				if (hit.transform == player.transform)
//					Debug.Log("hello world");
//		
//		}
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		//Vector3 pitch_vector = new Vector3ai_manager.GetPitchAreaCoords().x
		Ray ray = new Ray(ball_vector, -1*(ai_manager.GetPitchAreaCoords(index) - ball_vector));
		if (player.transform.Find("Collider").collider.Raycast(ray, out hit, Mathf.Infinity))
			Debug.Log("HIT");
			//Debug.DrawLine(ray.origin, hit.point);


		Debug.DrawRay(ball_vector, ai_manager.GetPitchAreaCoords(index) - ball_vector);
		Debug.DrawRay(ball_vector, -100*(ai_manager.GetPitchAreaCoords(index) - ball_vector));
		Debug.Log(ball_vector + " - " + ai_manager.GetPitchAreaCoords(index));
		//Debug.Log(ai_manager.GetPitchAreaCoords(index));
		//Physics.raycast
	}



	private void ResetControllers()
	{
		player.player_controller.commands.vertical_direction = 0;
		player.player_controller.commands.horizontal_direction = 0;
	}

	private void GoToArea(int area)
	{
		int area_flank = AreaToFlank(area);
		int current_flank = AreaToFlank(player.getCurrentArea());
		
		if (current_flank < area_flank)
			player.player_controller.commands.vertical_direction = 1;
		else if (current_flank > area_flank)
			player.player_controller.commands.vertical_direction = -1;
		else if (current_flank == area_flank)
			player.player_controller.commands.vertical_direction = 0;

		if (IsLeftOrRight(area, player.getCurrentArea()) == LEFT)
			player.player_controller.commands.horizontal_direction = 1;
		else if (IsLeftOrRight(area, player.getCurrentArea()) == RIGHT)
			player.player_controller.commands.horizontal_direction = -1;
		else
			player.player_controller.commands.horizontal_direction = 0;
	}

	private void GoToBall()
	{
		if (player.transform.position.x > ball.transform.position.x)
			player.player_controller.commands.vertical_direction = MOVE_DOWN;
		if (player.transform.position.x < ball.transform.position.x)
			player.player_controller.commands.vertical_direction = MOVE_UP;
		if (player.transform.position.z > ball.transform.position.z)
			player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
		if (player.transform.position.z < ball.transform.position.z)
			player.player_controller.commands.horizontal_direction = MOVE_LEFT;
	}

	private void RotateAroundBallCounterclockwise()
	{
		int quadrant = GetQuadrant();

		Debug.Log(quadrant);

		if (quadrant == 1) {
		
			player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			player.player_controller.commands.vertical_direction = 0;
		
		} else if (quadrant == 2) {

			player.player_controller.commands.vertical_direction = MOVE_UP;
			player.player_controller.commands.horizontal_direction = 0;
		
		} else if (quadrant == 3) {
		
			player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			player.player_controller.commands.vertical_direction = 0;
			
		} else if (quadrant == 4) {

			player.player_controller.commands.horizontal_direction = 0;
			player.player_controller.commands.vertical_direction = MOVE_DOWN;
		
		}
	}

	private void RotateAroundBallClockwise()
	{
		int quadrant = GetQuadrant();
		
		Debug.Log(quadrant);
		
		if (quadrant == 1) {
			
			player.player_controller.commands.horizontal_direction = 0;
			player.player_controller.commands.vertical_direction = MOVE_UP;
			
		} else if (quadrant == 2) {
			
			player.player_controller.commands.vertical_direction = 0;
			player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			
		} else if (quadrant == 3) {
			
			player.player_controller.commands.horizontal_direction = 0;
			player.player_controller.commands.vertical_direction = MOVE_DOWN;
			
		} else if (quadrant == 4) {
			
			player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			player.player_controller.commands.vertical_direction = 0;
			
		}
	}

	private float GetAnglePlayerBall()
	{
		Vector2 a = new Vector2(ball.transform.position.x - player.transform.position.x, ball.transform.position.z-player.transform.position.z);
		Vector2 b = new Vector2(player.transform.position.x, player.transform.position.z);

		return Vector2.Angle(a,b);
	}

	private int GetQuadrant()
	{
		float Xb = ball.transform.position.x;
		float Xp = player.transform.position.x;

		float Zb = ball.transform.position.z;
		float Zp = player.transform.position.z;

		if (Xb > Xp)
		
			if (Zb < Zp) 
				return 1;
			else
				return 2;

		else

			if (Zb < Zp)
				return 4;
			else 
				return 3;

		return 0;

	}

	// given an area, it returns the flank
	private int AreaToFlank(int area) 
	{
		int flank;

		for (int i = 0; i < 6; i++)
			if (area == 3*i)
				return BOTTOM_FLANK;
			else if (area == 3*i+1)
				return MID_FLANK;
			else if (area == 3*i+2)
				return TOP_FLANK;
		return -1;

	}

	private int IsLeftOrRight(int area, int current_area)
	{
		if(current_area == area || current_area == area+1 || current_area == area-1)
			return 0; //is neither left nor right;
		else if (current_area > area)
			return RIGHT;
		else if (current_area < area)
			return LEFT;

		return 0;
	}

	public override void UsePower(PlayerController.Commands commands){}

	public override void EmmitPowerFX(string type = "none"){}

}
