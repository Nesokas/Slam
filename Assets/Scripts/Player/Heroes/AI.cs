using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : Hero {

	private AIManager ai_manager;
//	private PlayerController controller;
	Local_Player player;
	private GameObject ball;
	private Ball_Behaviour ball_behaviour;

	private const int BOTTOM_FLANK = 0, MID_FLANK = 1, TOP_FLANK = 2, LEFT = 3, RIGHT = 4;

	private const int MOVE_UP = 1, MOVE_DOWN = -1, MOVE_LEFT = 1, MOVE_RIGHT = -1;

	private const int ABOVE = 1, BELOW = 2;

	private bool has_ball = false;

	private bool is_clockwise_rotation = true;

	private bool touched_ball = false;

	private float distance_to_ball = 0;

	//distance from which the player is considered to be in possession
	private float possession_distance_threshold = 1f;

	private Transform sphere;

	private int goto_area = 0;

	private int key = 0;

	Transform colliderAIPossession;
	Transform colliderAIPossessionLeft;
	Transform colliderAIPossessionRight;
	Transform player_collider;
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

		ball_behaviour = ball.GetComponent<Ball_Behaviour>();

		colliderAIPossession = player.transform.Find("ColliderAIPossession");
		colliderAIPossessionLeft = player.transform.Find("ColliderAIPossession/ColliderAIPossessionLeft");
		colliderAIPossessionRight = player.transform.Find("ColliderAIPossession/ColliderAIPossessionRight");
		player_collider = player.transform.Find("Collider");
		colliderAIPossession.gameObject.SetActive(true);

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


		if (Input.GetKeyDown("0"))
		    key = 0;
		else if (Input.GetKeyDown("1"))
			key = 1;
		else if (Input.GetKeyDown("2"))
			key = 2;
		if (Input.GetKeyDown("3"))
			key = 3;
		if (Input.GetKeyDown("4"))
			key = 4;



		if (distance_to_ball < possession_distance_threshold) {
			has_ball = true;
		} else {
			has_ball = false;
		}
		DribbleToArea(key);

//		test(key);
//
//		RotateAroundBall(key);
	}

	private void test(int index)
	{
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		Ray ray = new Ray(ball_vector, -1*(ai_manager.GetPitchAreaCoords(index) - ball_vector));
		RaycastHit hit;
		if (player_collider.collider.Raycast(ray, out hit, Mathf.Infinity)) {
			if (colliderAIPossession.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				Debug.Log("HIT");
				
			} else if (colliderAIPossessionLeft.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				Debug.Log("LEFT");
				
			} else if (colliderAIPossessionRight.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				Debug.Log("RIGHT");
			}
		}
		Debug.DrawRay(ball_vector, ai_manager.GetPitchAreaCoords(index) - ball_vector);
		Debug.DrawRay(ball_vector, -100*(ai_manager.GetPitchAreaCoords(index) - ball_vector));

	}

	private void DribbleToArea(int index)
	{

		goto_area = index;

		RaycastHit hit;
		
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		Ray ray = new Ray(ball_vector, -1*(ai_manager.GetPitchAreaCoords(index) - ball_vector));

		if (!has_ball) {

			GoToBall();

		} else {

			ResetControllers();
		
			RotateAroundBall(index);
		
			int below_or_above = IsBallAboveOrBellow(ball_behaviour.GetCurrentArea(), index);
			int left_or_right = IsLeftOrRight(ball_behaviour.GetCurrentArea(), index);

			if (player_collider.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				if (colliderAIPossession.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				//	Debug.Log("HIT");
					if (below_or_above == ABOVE) //if ball's index is above the target's, that means player is above the ball so he must move down
						player.player_controller.commands.vertical_direction = MOVE_DOWN;
					else if (below_or_above == BELOW)
						player.player_controller.commands.vertical_direction = MOVE_UP;
					else
						player.player_controller.commands.vertical_direction = 0;
					
					if (left_or_right == LEFT)
						player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
					else if (left_or_right == RIGHT)
						player.player_controller.commands.horizontal_direction = MOVE_LEFT;
					else
						player.player_controller.commands.horizontal_direction = 0;
				
				} else if (colliderAIPossessionLeft.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				//	Debug.Log("LEFT");
					ResetControllers();
					AdjustAccordingToQuadrant(LEFT);
				
				} else if (colliderAIPossessionRight.collider.Raycast(ray, out hit, Mathf.Infinity)) {
				//	Debug.Log("RIGHT");
					ResetControllers();
					AdjustAccordingToQuadrant(RIGHT);
				}
			}
		}
		Debug.DrawRay(ball_vector, ai_manager.GetPitchAreaCoords(index) - ball_vector);
		Debug.DrawRay(ball_vector, -100*(ai_manager.GetPitchAreaCoords(index) - ball_vector));


	}


	private void RotateAroundBall(int index)
	{
		int ball_below_or_above_target = IsBallAboveOrBellow(ball_behaviour.GetCurrentArea(), index);
		int ball_left_or_right_target = IsLeftOrRight(ball_behaviour.GetCurrentArea(), index);

		int player_left_or_right_target = IsLeftOrRight(ball_behaviour.GetCurrentArea(), index);
		int player_below_or_above_target = IsBallAboveOrBellow(player.getCurrentArea(), index);
		int player_below_or_above_ball;
		int player_left_or_right_ball;

		if (player.transform.position.x > ball.transform.position.x) {
			player_below_or_above_ball = ABOVE;
			//Debug.Log("---> PLAYER IS ABOVE THE BALL");
		}
		else {
			player_below_or_above_ball = BELOW;
		//	Debug.Log("---> PLAYER IS BELOW THE BALL");
		}

		if (player.transform.position.z > ball.transform.position.z) {
			player_left_or_right_ball = LEFT;
			//Debug.Log("---> PLAYER IS LEFT FROM BALL");
		}
		else {
			player_left_or_right_ball = RIGHT;
		//	Debug.Log("---> PLAYER IS RIGHT FROM BALL");
		}

		float ball_target_slope = GetSlope(ball.transform.position, ai_manager.GetPitchAreaCoords(index));
		float ball_target_y_intercept = GetYIntercept(ball_target_slope, ball.transform.position);

		float player_target_slope = GetSlope(player.transform.position, ai_manager.GetPitchAreaCoords(index));

		Debug.Log(IsAboveLine(player.transform.position, ball_target_slope, ball_target_y_intercept));

		if (IsAboveLine(player.transform.position, ball_target_slope, ball_target_y_intercept))
			if (ball_below_or_above_target == ABOVE)
				if (ball_target_slope > 0)
					RotateAroundBallClockwise();
				else
					RotateAroundBallCounterclockwise();
			else
				if(ball_target_slope > 0)
					RotateAroundBallCounterclockwise();
				else
					RotateAroundBallClockwise();
		else
			if (ball_below_or_above_target == ABOVE)
				if(ball_target_slope > 0)
					RotateAroundBallCounterclockwise();
				else
					RotateAroundBallClockwise();
			else
				if (ball_target_slope > 0)
					RotateAroundBallClockwise();
				else
					RotateAroundBallCounterclockwise();
	

	//	Debug.Log("player slope -> " + player_target_slope + " ball slope -> " + ball_target_slope);
		
		//ROTATE CLOCKWISE

		//Debug.Log(ball_left_or_right_target == LEFT);

		

	//	Debug.Log(ball_target_slope);
	}

	private float GetYIntercept(float slope, Vector3 point)
	{
		//y = m.x + b

		float y = point.x;
		float x = -point.z;

		float b = y - slope * x;

		return b;
	}

	private bool IsAboveLine(Vector3 point, float slope, float b)
	{
		Debug.Log("player point -> " + point);
		float x = -point.z;

		float y = x * slope + b;

		if (y > point.x)
			return false;

		else
			return true;
	}

	private float GetSlope(Vector3 vec1, Vector3 vec2)
	{
		float m = 0;

		m = -(vec2.x - vec1.x)/(vec2.z - vec1.z);

		return m;
	}



	//called when AI is controlling the ball but it slips to either of its left or right possession colliders
	private void AdjustAccordingToQuadrant(int left_or_right)
	{
		int quadrant = GetQuadrant();

		//Debug.Log("quadrant -> " + quadrant + " left_or_right-> " + left_or_right);
		ResetControllers();
		//if ray hit the left collider
		if (left_or_right == LEFT) {
			if(quadrant == 1)
				player.player_controller.commands.vertical_direction = MOVE_UP;
			else if (quadrant == 2)
				player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			else if (quadrant == 3)
				player.player_controller.commands.vertical_direction = MOVE_DOWN;
			else if (quadrant == 4)
				player.player_controller.commands.horizontal_direction = MOVE_RIGHT;

		} else { //if ray hit the right collider
			if (quadrant == 1)
				player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			else if (quadrant == 2)
				player.player_controller.commands.vertical_direction = MOVE_UP;
			else if (quadrant == 3)
				player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			else if (quadrant == 4) {
				player.player_controller.commands.vertical_direction = MOVE_DOWN;
			Debug.Log("moving down");
			}
		}
	}

	private int IsBallAboveOrBellow(int ball_area, int target_area)
	{
		int area_flank = AreaToFlank(target_area);
		int ball_flank = AreaToFlank(ball_area);

		if (ball_flank > area_flank)
			return ABOVE;

		else if (ball_flank < area_flank)
			return BELOW;

		else 
			return 0;

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

	//	Debug.Log(quadrant);

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
