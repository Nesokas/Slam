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

	private const int UP = 1, DOWN = 2;

	private const int ABOVE = 1, BELOW = 2;

	//private const int RED = 1, BLUE = 2;

//	private bool has_ball = false;

	private bool is_clockwise_rotation = true;

	private bool touched_ball = false;

	//distance from which the player is considered to be in possession
	private float possession_distance_threshold = 1.25f;

	private Transform sphere;

	private int goto_area = 0;

	private int key = 0;

	Transform colliderAIPossession;
	Transform colliderAIPossessionLeft;
	Transform colliderAIPossessionRight;
	Transform player_collider;

	private struct Beliefs 
	{
		public Vector3 own_goal_position;
		public Vector3 opponent_goal_position;
		public float goal_width;
		public bool opponent_has_ball;
		public bool teammate_has_ball;
		public int team;
		public bool has_ball;
		public bool is_obstructed_path;
		public float distance_to_ball;
		//public List<int> opponents_in_the_way;
	}

	private enum Desires 
	{
		PASS,
		SCORE,
		DRIBBLE,
		TACKLE,
		MOVE_TO_AREA,
		TAKE_POSSESSION
		
	}

	Beliefs beliefs;

	// The desire to which the agent has commited will be the intention
	Desires desire; 

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
		ai_manager.InsertAI(this);
		this.team = player.team;
		beliefs.team = player.team;
		if (beliefs.team == GlobalConstants.RED) {
			beliefs.own_goal_position = ai_manager.GetRedTeamGoalPosition();
			beliefs.opponent_goal_position = ai_manager.GetBlueTeamGoalPosition();
		} else if (beliefs.team == GlobalConstants.BLUE) {
			beliefs.own_goal_position = ai_manager.GetBlueTeamGoalPosition();
			beliefs.opponent_goal_position = ai_manager.GetRedTeamGoalPosition();
			Debug.Log(this.GetTeam() +  " CORRECT TEAM " + GlobalConstants.BLUE);
		}
		beliefs.goal_width = ai_manager.GoalWidth();

		beliefs.distance_to_ball = 0;

		desire = Desires.TAKE_POSSESSION;

	}

	public void Update () 
	{
		
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

		ResetControllers();
		UpdatePossession();
		//DribbleToArea(16);


	//	Pass ();

		if(beliefs.has_ball) {
			if (!CheckObstructedPath())
				if (IsInArea(0))
				    Pass ();
				else
					DribbleToArea(0);
		} else {
			GoToBall();
		//	Debug.Log("gotoball");
		}
	//	Debug.Log(beliefs.has_ball);
		//Score();
	//	Pass ();
	}


	private void Score()
	{
		int layer_mask = 1 << 29 | 1 << 28 | 1 << 27;
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
	
		RaycastHit goal_hit;
		RaycastHit shoot_hit;

		Vector3 goal_pos = new Vector3(beliefs.opponent_goal_position.x, -0.1f, beliefs.opponent_goal_position.z);
		Ray goal_ray = new Ray(ball_vector, beliefs.opponent_goal_position - ball_vector);
		Ray shoot_ray = new Ray(ball_vector, -1*(goal_pos - ball_vector));
		
		if(Physics.Raycast(goal_ray, out goal_hit, Mathf.Infinity, layer_mask)) {
			if (goal_hit.collider.CompareTag("goal_detection")) {
				if (colliderAIPossession.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
					player.player_controller.commands.shoot = 1;
				}
			}
		}

		Debug.DrawRay(ball_vector, goal_pos - ball_vector);
		Debug.DrawRay(ball_vector, -1*(goal_pos - ball_vector));
	}

	private void Pass()
	{
		Vector3 flanks = ai_manager.IsTeammateAloneInFlanks();

		if (flanks.x != team) {

			PassTopFlank();
		}
//		else if (flanks.y == team)
//			PassMidFlank();
//		else if (flanks.z == team)
//			PassBottomTeam();
		
	}

	private void PassTopFlank()
	{
		List<Hero>[] top_flank_heroes = ai_manager.GetTopFlankHeroes();

		for (int i = 0; i < 6; i++)
			foreach(Hero hero in top_flank_heroes[i]) {
				PassTeammate(hero);
				
				return;
		}

	}

	private void PassTeammate(Hero hero)
	{
		int layer_mask = 1 << 28;
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);

		RaycastHit teammate_hit;
		RaycastHit shoot_hit;

		Vector3 player_pos = new Vector3(hero.GetPosition().x, -0.1f, hero.GetPosition().z);
		Ray teammate_ray = new Ray(ball_vector, player_pos - ball_vector);
		Ray shoot_ray = new Ray(ball_vector, -1*(player_pos - ball_vector));

		RotateAroundBall(hero.GetPosition());

		if(Physics.Raycast(teammate_ray, out teammate_hit, Mathf.Infinity, layer_mask)) {
			if (teammate_hit.collider.CompareTag("colliderShoot")) {
				if (colliderAIPossession.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
					player.player_controller.commands.shoot = 1;
					Debug.Log("PASS!!!");

				}
			}
		}
		
		Debug.DrawRay(ball_vector, player_pos - ball_vector);
		Debug.DrawRay(ball_vector, -1*(player_pos - ball_vector));
	}



//	// The vector it returns will be (T,F,F) if the Top flank has at least a teammate and
//	// no opponent in the flank, and the Mid and Bottom flanks have at least one opponent
//	private Vector3 IsTeammateAloneInFlank()
//	{
//		List<Hero>[] top_flank = ai_manager.GetTopFlankHeroes();
//		List<Hero>[] top_flank = ai_manager.GetTopFlankHeroes();
//		List<Hero>[] top_flank = ai_manager.GetTopFlankHeroes();
//	}

	private bool IsInArea(int index)
	{
		if (current_area == index)
			return true;
		else return false;
	}



	private void UpdateBeliefs()
	{
		UpdatePossession();
		if (beliefs.has_ball)
			CheckObstructedPath();

	}

	private void UpdatePossession()
	{

		beliefs.distance_to_ball = FindDistanceToBall();

	//	Debug.Log(beliefs.distance_to_ball + " + " + possession_distance_threshold);

		bool teammate_has_ball = false;
		bool opponent_has_ball = false;
		if (beliefs.distance_to_ball < possession_distance_threshold) {
			beliefs.has_ball = true;
			ai_manager.InsertPlayerInPossession(this);
		} else {
			beliefs.has_ball = false;
			ai_manager.RemovePlayerInPossession(this);
		}
		foreach(Hero hero in ai_manager.GetPlayersInPossession()) {
			if (hero.GetTeam() == this.team)
				teammate_has_ball = true;
			else if (hero.GetTeam() != this.team)
				opponent_has_ball = true;
		}

		beliefs.teammate_has_ball = teammate_has_ball;
		beliefs.opponent_has_ball = opponent_has_ball;

	}

	private float FindDistanceToBall()
	{
		float x;
		float z;

		x = player.transform.position.x - ball.transform.position.x;
		z = player.transform.position.z - ball.transform.position.z;

		return Mathf.Abs(Mathf.Sqrt(x*x + z*z));
	}

	private bool CheckObstructedPath()
	{
		if(beliefs.has_ball == false) {
			beliefs.is_obstructed_path = false;
			return false;
		
		}
		int layer_mask = 1 << 30;
		int index = 0;
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		Ray ray = new Ray(ball_vector, (ai_manager.GetPitchAreaCoords(index) - ball_vector));

		RaycastHit[] hits;
		hits = Physics.RaycastAll(ray, Mathf.Infinity , layer_mask);

		for (int i=0; i < hits.Length; i++) {
			RaycastHit hit = hits[i];
			if (IsOponnentInArea(int.Parse(hit.collider.name))) {
				beliefs.is_obstructed_path = true;
				return true;
			}
		}
		
		Debug.DrawRay(ball_vector, ai_manager.GetPitchAreaCoords(index) - ball_vector);
		Debug.DrawRay(ball_vector, -100*(ai_manager.GetPitchAreaCoords(index) - ball_vector));

		beliefs.is_obstructed_path = false;
		return false;
	}

	private bool IsOponnentInArea(int index)
	{
		List<Hero> hero_list = ai_manager.GetPlayerListFromArea(index);
	//	Debug.Log("area -> " + index + "hero_list size = " + hero_list.Count);
		foreach(Hero hero in hero_list) {
	//		Debug.Log("hero - " + hero.GetTeam() + "this - " + this.team);
			if (hero.GetTeam() != this.team) {
			//	Debug.Log("true");
				return true;
					
			}
		}
	//	Debug.Log("false");
		return false;
	}

	private bool IsTeammateInArea(int index)
	{
		List<Hero> hero_list = ai_manager.GetPlayerListFromArea(index);
		foreach(Hero hero in hero_list) {
			if (hero.GetTeam() == this.team) {
				return true;
			}
		}
		return false;
	}

	private void GoToArea(int index)
	{
		int below_or_above = IsAboveOrBellow(player.transform.position, ai_manager.GetPitchAreaCoords(index));
		int left_or_right = IsLeftOrRight(player.transform.position, ai_manager.GetPitchAreaCoords(index));

		if (below_or_above == ABOVE)
			Move(DOWN);
		else if (below_or_above == BELOW)
			Move(UP);
		else 
			StopMovingVertically();

		if (left_or_right == LEFT)
			Move (RIGHT);
		else if (left_or_right == RIGHT)
			Move (LEFT);
		else
			StopMovingHorizontally();

	}

	private void Move(int direction)
	{
		if (direction == UP)
			player.player_controller.commands.vertical_direction = MOVE_UP;
		else if (direction == DOWN)
			player.player_controller.commands.vertical_direction = MOVE_DOWN;
		else if (direction == LEFT)
			player.player_controller.commands.horizontal_direction = MOVE_LEFT;
		else if (direction == RIGHT)
			player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
	}

	private void StopMovingVertically()
	{
			player.player_controller.commands.vertical_direction = 0;
	}

	private void StopMovingHorizontally()
	{
		player.player_controller.commands.horizontal_direction = 0;
	}

	private void DribbleToArea(int index)
	{

		goto_area = index;

		RaycastHit hit;
		
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		Ray ray = new Ray(ball_vector, -1*(ai_manager.GetPitchAreaCoords(index) - ball_vector));

		if (!beliefs.has_ball) {
	
			GoToBall();

		} else {

			ResetControllers();
		
			RotateAroundBall(ai_manager.GetPitchAreaCoords(index));
			
			int below_or_above = IsAboveOrBellow(ball_behaviour.transform.position, ai_manager.GetPitchAreaCoords(index));
			int left_or_right = IsLeftOrRight(ball_behaviour.transform.position, ai_manager.GetPitchAreaCoords(index));

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


	private void RotateAroundBall(Vector3 target)
	{
		int ball_below_or_above_target = IsAboveOrBellow(ball_behaviour.transform.position, target);
		int ball_left_or_right_target = IsLeftOrRight(ball_behaviour.transform.position, target);

		int player_left_or_right_target = IsLeftOrRight(ball_behaviour.transform.position, target);
		int player_below_or_above_target = IsAboveOrBellow(player.transform.position, target);

		int player_below_or_above_ball = IsAboveOrBellow(player.transform.position, ball.transform.position);
		int player_left_or_right_ball = IsLeftOrRight(player.transform.position, ball.transform.position);


		float ball_target_slope = GetSlope(ball.transform.position, target);
		float ball_target_y_intercept = GetYIntercept(ball_target_slope, ball.transform.position);

		float player_target_slope = GetSlope(player.transform.position, target);

//		Debug.Log(IsAboveLine(player.transform.position, ball_target_slope, ball_target_y_intercept));

		if (IsAboveLine(player.transform.position, ball_target_slope, ball_target_y_intercept))
			if (ball_below_or_above_target == ABOVE)
				if (ball_target_slope > 0)
					RotateAroundBallClockwise();
				else
					RotateAroundBallCounterclockwise();
			else if (ball_below_or_above_target == BELOW)
				if(ball_target_slope > 0)
					RotateAroundBallCounterclockwise();
				else
					RotateAroundBallClockwise();
			else if(ball_left_or_right_target == LEFT)
				RotateAroundBallCounterclockwise();
			else
				RotateAroundBallClockwise();
		else
			if (ball_below_or_above_target == ABOVE)
				if(ball_target_slope > 0)
					RotateAroundBallCounterclockwise();
				else
					RotateAroundBallClockwise();
			else if (ball_below_or_above_target == BELOW)
				if (ball_target_slope > 0)
					RotateAroundBallClockwise();
				else
					RotateAroundBallCounterclockwise();
			else if(ball_left_or_right_target == LEFT)
				RotateAroundBallClockwise();
			else
				RotateAroundBallCounterclockwise();
		//	else
		//	RotateAroundBallClockwise();
	

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
	//	Debug.Log("player point -> " + point);
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
			//Debug.Log("moving down");
			}
		}
	}

	private int IsAboveOrBellow(Vector3 ball_pos, Vector3 target_pos)
	{

		if (ball_pos.x > target_pos.x)
			return ABOVE;
		else
			return BELOW;

	}


	private void ResetControllers()
	{
		player.player_controller.commands.shoot = 0;
		player.player_controller.commands.vertical_direction = 0;
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

	private int IsLeftOrRight(Vector3 area, Vector3 current_area)
	{

		if (area.z > current_area.z)
			return LEFT;
		else
			return RIGHT;
		
	}

	public override void UsePower(PlayerController.Commands commands){}

	public override void EmmitPowerFX(string type = "none"){}

}
