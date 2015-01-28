using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI : Hero {


//	private PlayerController controller;
	Local_Player local_player;
	private GameObject ball;
	private Ball_Behaviour ball_behaviour;
	private Transform angry_steam;
	private Transform sweat;
	private const int BOTTOM_FLANK = 0, MID_FLANK = 1, TOP_FLANK = 2, LEFT = 3, RIGHT = 4;

	private const int MOVE_UP = 1, MOVE_DOWN = -1, MOVE_LEFT = 1, MOVE_RIGHT = -1;

	private const int UP = 1, DOWN = 2;

	private const int ABOVE = 1, BELOW = 2;

	private const int MAX_DEPTH = 6;

	private bool is_clockwise_rotation = true;

	private bool touched_ball = false;

	//distance from which the player is considered to be in possession
	private float possession_distance_threshold = 1.25f;

	private Transform sphere;

	private int goto_area = 0;

	private int key = 0;

	Transform colliderAIPossessionCenter;
	Transform colliderAIPossessionRotation;
	Transform player_collider;

	private int emotion;

	private Vector3 look_target;

	Transform hands;
	private Animator hand_animator;

	private int Hide_state;
	private int Point_state;
	private int PointEnd_state;
	private int OKStart_state;
	private int OKEnd_state;
	private int Celebrate_state;
	private int Sad_state;
	private int AskForBallStart_state;
	private int AskForBallEnd_state;

	private enum Actions
	{
		GO_TO_AREA,
		GO_TO_BALL,
		DRIBBLE_TO_AREA,
		PASS,
		PASSED,
		PASS_TO_AREA,
		SCORE,
		RECEIVE_PASS,
		POSITION_TO_SHOOT,
		HESITATE_SHOOT,
		NULL
	}

	private struct Action
	{
		public Actions intent;
		public int args;
	}

	private struct Beliefs 
	{
		public Vector3 own_goal_position;
		public Vector3 opponent_goal_position;
		public float goal_width;
		public bool opponent_has_ball;
		public bool teammate_has_ball;
		public bool is_in_scoring_depth;
		public int team;
		public bool has_ball;
		public bool is_obstructed_path;
		public float distance_to_ball;
		public int team_in_possession;
		public Hero hero_in_possession;
		public Hero teammate_closer_to_ball;
		public Hero opponent_closer_to_ball;
		public AI teammate;
		public Expressions teammate_expression;
		public bool has_shot;
		public bool teammate_has_passed;
		public float ball_z_prediction;
		//public List<int> opponents_in_the_way;
	}

	private enum Desires 
	{
		PASS,
		SCORE,
		DRIBBLE,
		TACKLE,
		MOVE_TO_AREA,
		TAKE_POSSESSION,
		RECEIVE_BALL
	}

	private enum Expectations
	{
		PASS,
		SCORE,
		DEFEND,
		NULL
	}

	private enum Expressions 
	{
		REQUEST_PASS,
		INTEND_TO_PASS,
		INTEND_TO_SCORE,
		OK,
		EXPECT_SCORE,
		GOING_TO_AREA,
		GOING_TO_BALL,
		NULL
		
	}

	private enum EnvironmentMessages
	{
		WALL_HIT,
		NULL
	}

	private struct Expression
	{
		public Expressions expression;
		public int args;
	}

	int CONFIDENCE_THRESHOLD = 50; // Threshold (%) from which the action may or may not be interrupted
	int current_confidence = 0;

	int HIGH_CONFIDENCE_VALUE = 15;
	int LOW_CONFIDENCE_VALUE = 30;


	Beliefs beliefs;
	Expectations expectation;
	//Expressions expression;
	// The desire to which the agent has commited will be the intention
	Desires desire;
	Action current_intention;
	Expression current_expression;

	public AI(Player_Behaviour player)
	{
		hero_prefab = Resources.Load<GameObject>("Heroes/Tom");
		ai_manager = GameObject.Find("AIManager").GetComponent<AIManager>();
		this.player = player;
		this.local_player = (Local_Player)player;
		player.SetIsAI(true);
		is_ai = true;
		ball = GameObject.FindGameObjectWithTag("ball");

		ball_behaviour = ball.GetComponent<Ball_Behaviour>();

		colliderAIPossessionCenter = player.transform.Find("ColliderAIPossession/ColliderAIPossessionCenter");
		colliderAIPossessionRotation = player.transform.Find("ColliderAIPossession/ColliderAIPossessionRotation");
		player_collider = player.transform.Find("Collider");
		colliderAIPossessionCenter.gameObject.SetActive(true);


	}
	// Use this for initialization
	public override void Start() 
	{
		this.team = player.team;
		beliefs.team = player.team;
		ai_manager.InsertHero(this);
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
		beliefs.has_shot = false;
		beliefs.teammate_has_passed = false;
		desire = Desires.TAKE_POSSESSION;

		expectation = Expectations.DEFEND;
		current_intention = new Action();

		Transform mesh = player.transform.Find("Mesh");
		hands = mesh.Find("Hands");
		hands.gameObject.SetActive(false);

		sweat = player.transform.Find("Mesh").Find("Sweat");
		sweat.particleSystem.Stop();
		angry_steam = player.transform.Find("Mesh").Find("Angry_Steam");
		foreach (Transform child in angry_steam) {
			child.particleSystem.Stop();
		}

		look_target = ball.transform.position;
		hand_animator = hands.GetComponent<Animator>();
		Point_state = Animator.StringToHash("Base Layer.Point");
		PointEnd_state = Animator.StringToHash("Base Layer.PointEnd");
		OKStart_state = Animator.StringToHash("Base Layer.OKStart");
		OKEnd_state = Animator.StringToHash("Base Layer.OKEnd");
		AskForBallStart_state = Animator.StringToHash("Base Layer.AskForBallStart");
		AskForBallEnd_state = Animator.StringToHash("Base Layer.AskForBallEnd");
		Hide_state = Animator.StringToHash("Base Layer.Hide");
		Celebrate_state = Animator.StringToHash("Base Layer.Celebrate");
		Sad_state = Animator.StringToHash("Base Layer.Sad");

		NotificationCenter.DefaultCenter.AddObserver(this.player, "AnticipateToPass");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnSignalOK");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnRequestPass");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "AnticipateToScore");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnScore"); //When he shoots in fact
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnPass");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnWallHit");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnGoingToArea");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnGoingToBall");
		NotificationCenter.DefaultCenter.AddObserver(this.player, "OnCancelAction");
	}


	public override void Update() 
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
		if (Input.GetKeyDown("5"))
			key = 5;

		ResetControllers();
	
		UpdateBeliefs();
		UpdatePossession();

		look_target = ball.transform.position;

		if (current_intention.intent == Actions.GO_TO_BALL) {
			GoToBall();
		} else if(current_intention.intent == Actions.DRIBBLE_TO_AREA) {
			DribbleToArea(current_intention.args);
		} else if (current_intention.intent == Actions.GO_TO_AREA) {
			GoToArea(current_intention.args);
		} else if (current_intention.intent == Actions.PASS) {
			look_target = beliefs.teammate.GetPosition();
			if(beliefs.teammate_expression == Expressions.OK) {
				//Pass();
				PassTeammate(beliefs.teammate);
			}
		} else if (current_intention.intent == Actions.PASS_TO_AREA) {
			look_target = ai_manager.GetPitchAreaCoords(current_intention.args);
			if (beliefs.teammate_expression == Expressions.OK) {
				PassPos(ai_manager.GetPitchAreaCoords(current_intention.args));
			} else if (beliefs.has_ball != true) {
				RotateAroundBall(ai_manager.GetPitchAreaCoords(current_intention.args));
			}
		} else if (current_intention.intent == Actions.SCORE) {
			look_target = beliefs.opponent_goal_position;
			if (current_confidence >= CONFIDENCE_THRESHOLD) {
				Score();
			} else {
				OnCancelAction();
			//	GetFrustrated();
			//	SetActionPass();
			}
		} else if (current_intention.intent == Actions.RECEIVE_PASS && beliefs.teammate_has_passed) {
			ReceivePass();
			ThumbsUpEnd();
		} else if (current_intention.intent == Actions.HESITATE_SHOOT) {
			look_target = beliefs.opponent_goal_position;
		}
		UpdateLookAt(look_target);

		if(hand_animator.GetCurrentAnimatorStateInfo(0).nameHash == Hide_state) {
		
			hands.gameObject.SetActive(false);
		
		}

		/*
		if (current_action.action == Actions.PASS || current_action.action == Actions.PASS_TO_AREA) {
			if (beliefs.has_ball != true)
				GoToBall();
		}*/


		











/*
		if (beliefs.has_ball) {
			if (ai_manager.GetPlayersInPossession().Count > 1) {
				//Shoot();
				Score();
			} else GoOpenFlank();
			if (beliefs.is_in_scoring_depth) {
				if(!Score()) { //returns false if can't find a clear opening to shoot to score
					Pass();
					expectation = Expectations.PASS; //If can't score will try to shoot
				} else {
					expectation = Expectations.SCORE;
				}
			} else
				Pass();
		} else {
			if (ai_manager.GetPlayersInPossession().Count > 1 || !beliefs.teammate_closer_to_ball.Equals(this)) {
				Defend();
			}
		}

		if (beliefs.team_in_possession == 0 || beliefs.team_in_possession != team) {
			if (beliefs.teammate_closer_to_ball.Equals(this)) {
				GoToBall();
			}
		} else if (!beliefs.has_ball && beliefs.team_in_possession == team) {
			Unmark ();
		//	expectation = Actions.PASS;
		}
		*/

	}

	public void UpdateLookAt(Vector3 target) 
	{
		Quaternion rotation = Quaternion.LookRotation(target - player.transform.position);
		player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * 5);
	}

	public void OnCancelAction()
	{
		GetFrustrated();
		current_intention.intent = Actions.HESITATE_SHOOT;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnCancelAction");


	}

	public IEnumerator CancelAction(NotificationCenter.Notification notification) 
	{
		if (object.ReferenceEquals(this.player, notification.sender)) {
			yield return new WaitForSeconds(0.5f);
			StopPointing();
			//ai_manager.AgentResponse(this);
		}
		if (object.ReferenceEquals(this.player, notification.sender)) {
			yield return new WaitForSeconds(0.5f);
			ai_manager.AgentResponse(this);
		}


	}

	public void SetActionRequestPass(int area)
	{
		OnRequestPass(area);
	}

	public void SetActionDribbleToArea(int area)
	{
		current_intention.intent = Actions.DRIBBLE_TO_AREA;
		current_intention.args = area;
	}

	public void SetActionGoToArea(int area)
	{
		OnGoingToArea(area);
		current_intention.intent = Actions.GO_TO_AREA;
		current_intention.args = area;
	}

	public void SetActionGoingToBall()
	{
		OnGoingToBall();
		current_intention.intent = Actions.GO_TO_BALL;
		//current_action.args = area;
	}

	public void SetActionPass()
	{
		Debug.Log("SET ACTION PASS");
		AnticipateToPass(beliefs.teammate.GetCurrentArea());
		current_intention.intent = Actions.PASS;
		current_intention.args = -1;
	}

	public void SetActionScore(int confidence)
	{
		current_confidence = confidence;
		AnticipateToScore();
		current_intention.intent = Actions.SCORE;
		current_intention.args = -1;
	}

	public void SetActionNull()
	{
		current_intention.intent = Actions.NULL;
		ai_manager.AgentResponse(this);
	}

	public void SetActionReceivePass()
	{
		OnSignalOK();
		current_intention.intent = Actions.RECEIVE_PASS;
	}

	public void SetActionPassToArea(int index)
	{
		AnticipateToPass(index);
		current_intention.intent = Actions.PASS_TO_AREA;
		current_intention.args = index;
	}

	public void SetDesireReceiveBall()
	{
		desire = Desires.RECEIVE_BALL;
	}

	public void ReceivePass()
	{
//		Debug.Log(beliefs.ball_z_prediction);
	//	Debug.Log(beliefs.ball_z_prediction + " - " + player.transform.position.z );
		if (beliefs.ball_z_prediction > player.transform.position.z) {
			Move(LEFT);
	//		Debug.Log("left");
		} else {
	//		Debug.Log("right");
			Move(RIGHT);
		}
		if (beliefs.distance_to_ball < 2.5) {
	//		Debug.Log("response");
			ai_manager.AgentResponse(this);
		}
	}

	private bool GoOpenFlank()
	{
		Vector3 flanks = ai_manager.IsTeammateAloneInFlanks(this);
		int current_flank = ai_manager.AreaToFlank(current_area);
		if (current_flank == GlobalConstants.TOP_FLANK) {
			if (flanks.x == -1) {
				DribbleToArea(DepthToArea(GlobalConstants.TOP_FLANK ,AreaToDepth(current_area)+1));
				//return true;
			}
		} else if (current_flank == GlobalConstants.MID_FLANK) {
			if (flanks.y == -1) {
				DribbleToArea(DepthToArea(GlobalConstants.MID_FLANK, AreaToDepth(current_area)+1));
			//	return true;
			}
		} else if (current_flank == GlobalConstants.BOTTOM_FLANK) {
			if (flanks.z == -1) {
				DribbleToArea(DepthToArea(GlobalConstants.BOTTOM_FLANK, AreaToDepth(current_area)+1));
				//return true;
			}
		}
	//	Debug.Log(DepthToArea(GlobalConstants.TOP_FLANK ,AreaToDepth(current_area)+1));
		return false;

	}


	private bool DisputingBall()
	{
		if (ai_manager.GetPlayersInPossession().Count > 1)
			return true;
		else return false;
	}

	private void Defend()
	{
		int current_depth = AreaToDepth(beliefs.teammate_closer_to_ball.GetCurrentArea());
		
		if (team == GlobalConstants.RED)
			current_depth += 2;
		else
			current_depth -= 2;
		
		int new_area = DepthToArea(GlobalConstants.MID_FLANK, current_depth);

		//Debug.Log(current_depth);

		GoToArea(new_area);
		
	}

	private void UpdateScoringDepth()
	{
		int team = beliefs.team;

		if (team == GlobalConstants.RED) {
			if (AreaToDepth(current_area) <= 2) {
				beliefs.is_in_scoring_depth = true;
			} else {
			beliefs.is_in_scoring_depth = false;
			}
		} else {
			if (AreaToDepth(current_area) >= 5) {
				beliefs.is_in_scoring_depth = true;
			} else {
				beliefs.is_in_scoring_depth = false;
			}
		}
	}

	public bool Score()
	{
		RotateAroundBall(beliefs.opponent_goal_position);

		int layer_mask = 1 << 29 | 1 << 28 | 1 << 27;
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
	
		RaycastHit goal_hit;
		RaycastHit shoot_hit;

		Vector3 goal_pos = new Vector3(beliefs.opponent_goal_position.x, -0.1f, beliefs.opponent_goal_position.z);
		Ray goal_ray = new Ray(ball_vector, beliefs.opponent_goal_position - ball_vector);
		Ray shoot_ray = new Ray(ball_vector, -1*(goal_pos - ball_vector));

		if(Physics.Raycast(goal_ray, out goal_hit, Mathf.Infinity, layer_mask)) {
			if (goal_hit.collider.CompareTag("goal_detection")) {
				if (player_collider.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
					AdjustAccordingToQuadrant();	
					if (colliderAIPossessionRotation.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
						if (beliefs.has_ball != true) {
							GoToBall();
						} else {
							Shoot();

						}
						//return true;
					}
				}
			}
		}
		Debug.DrawRay(ball_vector, goal_pos - ball_vector);
		Debug.DrawRay(ball_vector, -1*(goal_pos - ball_vector));
		return true; // keep trying
	}

	private void Shoot()
	{
		if (player.IsCollidingWithBall() && beliefs.has_shot == false) {
			beliefs.has_shot = true;
			local_player.player_controller.commands.shoot = 1;
			ai_manager.AgentResponse(this);


			if (current_intention.intent == Actions.SCORE) {
				OnScore();
			}

			if (current_intention.intent == Actions.PASS || current_intention.intent == Actions.PASS_TO_AREA) {
				OnPass();
			}

//				NotificationCenter.DefaultCenter.PostNotification("ExpectingScore");
//				ExpectingScore();
//			}
		}


	}



	public void Pass()
	{
		Vector3 flanks = ai_manager.IsTeammateAloneInFlanks(this);

		if (flanks.x == team) {
			PassToFlank(ai_manager.GetTopFlankHeroes());
		}
		else if (flanks.y == team) {
			PassToFlank(ai_manager.GetMidFlankHeroes());
		}
		else if (flanks.z == team) {
			PassToFlank(ai_manager.GetBottomFlankHeroes());
		}
		
	}

	private void PassToFlank(List<Hero>[] flank_heroes)
	{

		for (int i = 0; i < 6; i++) {
			foreach(Hero hero in flank_heroes[i]) {
				if (!hero.Equals(this)) {
					PassTeammate(hero);
					return;
					//Debug.Log(hero);
				}


			}
		}

	}

	public void PassTeammate(Hero hero)
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
				if (colliderAIPossessionCenter.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
					Shoot();
					//Debug.Log("PASS!!!");

				}
			}
		}
		//Debug.Log(player_pos - ball_vector);
		Debug.DrawRay(ball_vector, player_pos - ball_vector);
		Debug.DrawRay(ball_vector, -1*(player_pos - ball_vector));
	}

	public void PassPos(Vector3 pos)
	{

		int layer_mask = 1 << 28;
		Vector3 ball_vector = new Vector3 (ball.transform.position.x, -0.1f, ball.transform.position.z);
		
		RaycastHit teammate_hit;
		RaycastHit shoot_hit;
		
		Vector3 player_pos = new Vector3(pos.x, -0.1f, pos.z);
		Ray teammate_ray = new Ray(ball_vector, player_pos - ball_vector);
		Ray shoot_ray = new Ray(ball_vector, -1*(player_pos - ball_vector));
		
		RotateAroundBall(pos);
		
		if(Physics.Raycast(teammate_ray, out teammate_hit)) {
		//	if (teammate_hit.collider.CompareTag("colliderShoot")) {
//				Debug.Log("PASS!!!");
				if (colliderAIPossessionCenter.collider.Raycast(shoot_ray, out shoot_hit, Mathf.Infinity)) {
					Shoot();
				}
			}
		//}
		//Debug.Log(player_pos - ball_vector);
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
		UpdateScoringDepth();
		beliefs.teammate_closer_to_ball = ai_manager.GetHeroCloserToBall(team);
		if (team == GlobalConstants.RED)
			beliefs.opponent_closer_to_ball = ai_manager.GetHeroCloserToBall(GlobalConstants.BLUE);
		else
			beliefs.opponent_closer_to_ball = ai_manager.GetHeroCloserToBall(GlobalConstants.RED);
//		if (beliefs.team == GlobalConstants.RED) {
//			beliefs.teammate_going_for_ball = ai_manager.GetGoingForBall(GlobalConstants.RED);
//			beliefs.opponent_going_for_ball = ai_manager.GetGoingForBall(GlobalConstants.BLUE);
//		} else {
//			beliefs.teammate_going_for_ball = ai_manager.GetGoingForBall(GlobalConstants.BLUE);
//			beliefs.opponent_going_for_ball = ai_manager.GetGoingForBall(GlobalConstants.RED);
//		}
	}

	private void UpdateDesires()
	{
//		if (beliefs.has_ball == false)
//			if (beliefs.teammate_has_ball == false)
//				if (beliefs.teammate_going_for_ball == false)
//					GoToBall();
//				else 
//					GoToArea(0);
//			else
//				GoToArea(10);
//		else
//			GoToArea(5);
		
	}

	private void Unmark()
	{
		Vector3 flanks = ai_manager.IsTeammateAloneInFlanks(this);

		Hero hero = ai_manager.GetPlayersInPossession()[0];
		int area_possession = hero.GetCurrentArea();
		//Debug.Log(area_possession);
		int current_flank = ai_manager.AreaToFlank(area_possession);
		int new_depth = AreaToDepth(area_possession);
		
		if (beliefs.team == GlobalConstants.RED)
			new_depth += -2;
		else
			new_depth += 2;
		
		int area = DepthToArea(current_flank, new_depth );

		int unmark_to_area = 0;
		if (flanks.x == -1)
			unmark_to_area = UnmarkToArea(area, GlobalConstants.TOP_FLANK);
		else if (flanks.y == -1)
			unmark_to_area = UnmarkToArea(area, GlobalConstants.MID_FLANK);
		else if (flanks.z == -1)
			unmark_to_area = UnmarkToArea(area, GlobalConstants.BOTTOM_FLANK);

//		Debug.Log(unmark_to_area);
		GoToArea(unmark_to_area);
		//Debug.Log(AreaToDepth();
	}

	//area: current area with the added depth; flank: new flank to which to unmark
	// return: new area to which to unmark
	private int UnmarkToArea(int area, int flank)
	{
		int current_flank = ai_manager.AreaToFlank(area);



		if (current_flank == flank)
			return area;
		
		else if (current_flank == GlobalConstants.TOP_FLANK) {
			if (flank == GlobalConstants.MID_FLANK)
				return area-1;
			else if (flank == GlobalConstants.BOTTOM_FLANK)
				return area-2;
		
		} else if (current_flank == GlobalConstants.MID_FLANK) {
			if (flank == GlobalConstants.TOP_FLANK)
				return area+1;
			else if (flank == GlobalConstants.BOTTOM_FLANK)
				return area-1;

		} else if (current_flank == GlobalConstants.BOTTOM_FLANK) {
			if (flank == GlobalConstants.TOP_FLANK)
				return area+2;
			else if (flank == GlobalConstants.MID_FLANK)
				return area+1;
		}
		//Debug.Log(area + " - " + current_flank + " - " + flank);
		return -1; //happens if area < 0 || area > 17

	}

	// Receives a flank and a depth (from 1 to 6) and converts that depth
	// to the corresponding area 
	private int DepthToArea(int flank, int depth)
	{

		if (depth > MAX_DEPTH)
			depth = MAX_DEPTH;
		else if (depth < 1)
			depth = 1;

		int area = 0;
		int current_depth = 0;

		while (current_depth < depth){
		
			if (ai_manager.AreaToFlank(area) == flank)
				current_depth++;
		
			area++;
		}

		return area-1;
	}

	private int AreaToDepth(int area)
	{
		int area_counter = 0;
		int current_depth = 1;
		int flank = ai_manager.AreaToFlank(area);
		while (area_counter < area){
			
			if (ai_manager.AreaToFlank(area_counter) == flank) {
			//	Debug.Log(area_counter + " - " + area);
				current_depth++;
			}
			
			area_counter++;
		}
		//Debug.Log(current_depth + " - " + area + " - " + flank);
		//Debug.Log(current_depth);
		return current_depth;

	}

	private void UpdatePossession()
	{

		beliefs.distance_to_ball = FindDistanceToBall();
		beliefs.team_in_possession = ai_manager.GetTeamInPossession();

	//	Debug.Log(beliefs.distance_to_ball + " + " + possession_distance_threshold);

		bool teammate_has_ball = false;
		bool opponent_has_ball = false;
		beliefs.has_ball = ai_manager.HeroHasBall(this);
		if (beliefs.team_in_possession == beliefs.team && !beliefs.has_ball)
			beliefs.teammate_has_ball = true;
		else
			beliefs.teammate_has_ball = false;

//		if (beliefs.distance_to_ball < possession_distance_threshold) {
//			beliefs.has_ball = true;
		//	ai_manager.InsertPlayerInPossession(this);
//		} else {
//			beliefs.has_ball = false;
//			ai_manager.RemovePlayerInPossession(this);
	//	}
//		foreach(Hero hero in ai_manager.GetPlayersInPossession()) {
//			if (hero.GetTeam() == this.team)
//				teammate_has_ball = true;
//			else if (hero.GetTeam() != this.team)
//				opponent_has_ball = true;
//		}

//		beliefs.teammate_has_ball = teammate_has_ball;
//		beliefs.opponent_has_ball = opponent_has_ball;

	}

	private float FindDistanceToBall()
	{
		float x;
		float z;

		x = player.transform.position.x - ball.transform.position.x;
		z = player.transform.position.z - ball.transform.position.z;

		return Mathf.Abs(Mathf.Sqrt(x*x + z*z));
	}

	private bool CheckObstructedPath(int index)
	{
		if(beliefs.has_ball == false) {
			beliefs.is_obstructed_path = false;
			return false;
		
		}
		int layer_mask = 1 << 30;
		//int index =  ai_manager.GetPitchAreaCoords(point);
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
		
//		Debug.DrawRay(ball_vector, ai_manager.GetPitchAreaCoords(index) - ball_vector);
//		Debug.DrawRay(ball_vector, -100*(ai_manager.GetPitchAreaCoords(index) - ball_vector));

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

	public void GoToArea(int index)
	{
		if (current_area == index) {
			ai_manager.AgentResponse(this);
			return;
		}

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
			local_player.player_controller.commands.vertical_direction = MOVE_UP;
		else if (direction == DOWN)
			local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
		else if (direction == LEFT)
			local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
		else if (direction == RIGHT)
			local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
	}

	private void StopMovingVertically()
	{
		local_player.player_controller.commands.vertical_direction = 0;
	}

	private void StopMovingHorizontally()
	{
		local_player.player_controller.commands.horizontal_direction = 0;
	}

	public void DribbleToArea(int index)
	{
		if (ball.GetComponent<Ball_Behaviour>().GetCurrentArea() == index && beliefs.has_ball) {
			ai_manager.AgentResponse(this);
			return;
		}

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
				if (colliderAIPossessionCenter.collider.Raycast(ray, out hit, Mathf.Infinity)) {

					AdjustAccordingToQuadrant();
				}
			}
		}

		if (current_area == index)
			ai_manager.AgentResponse(this);
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
	private void AdjustAccordingToQuadrant()
	{
		int quadrant = GetQuadrant();

		ResetControllers();
		//if ray hit the left collider

		if (quadrant == 14) {
			local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			local_player.player_controller.commands.vertical_direction = 0;
			return;
		} else if (quadrant == 23) {
			local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			local_player.player_controller.commands.vertical_direction = 0;
			return;
		} else if (quadrant == 12) {
			local_player.player_controller.commands.vertical_direction = MOVE_UP;
			local_player.player_controller.commands.horizontal_direction = 0;
		} else if (quadrant == 34) {
			local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
			local_player.player_controller.commands.horizontal_direction = 0;
		}

		//if (left_or_right == LEFT) {

			if(quadrant == 1)
				local_player.player_controller.commands.vertical_direction = MOVE_UP;
			else if (quadrant == 2)
				local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			else if (quadrant == 3)
				local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
			else if (quadrant == 4)
				local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
		} /*else { //if ray hit the right collider
			if (quadrant == 1)
				local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			else if (quadrant == 2)
				local_player.player_controller.commands.vertical_direction = MOVE_UP;
			else if (quadrant == 3)
				local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			else if (quadrant == 4) {
				local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
			Debug.Log("RIGHT");
			}
		}
	}*/

	private int IsAboveOrBellow(Vector3 ball_pos, Vector3 target_pos)
	{

		if (ball_pos.x > target_pos.x)
			return ABOVE;
		else
			return BELOW;

	}


	private void ResetControllers()
	{
		local_player.player_controller.commands.shoot = 0;
		local_player.player_controller.commands.vertical_direction = 0;
		local_player.player_controller.commands.horizontal_direction = 0;
	}

	public void GoToBall()
	{

		if (local_player.transform.position.x > ball.transform.position.x)
			local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
		if (local_player.transform.position.x < ball.transform.position.x)
			local_player.player_controller.commands.vertical_direction = MOVE_UP;
		if (local_player.transform.position.z > ball.transform.position.z)
			local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
		if (local_player.transform.position.z < ball.transform.position.z)
			local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;

		if (current_intention.intent == Actions.GO_TO_BALL && beliefs.has_ball)
			ai_manager.AgentResponse(this);
	}

	private void RotateAroundBallCounterclockwise()
	{
		int quadrant = GetQuadrant();

		if (quadrant == 1 || quadrant == 12) {
		//	Debug.Log(1);
			local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			local_player.player_controller.commands.vertical_direction = 0;
		
		} else if (quadrant == 2 || quadrant == 23) {
		//	Debug.Log(2);
			local_player.player_controller.commands.vertical_direction = MOVE_UP;
			local_player.player_controller.commands.horizontal_direction = 0;
		
		} else if (quadrant == 3 || quadrant == 34) {
		//	Debug.Log(3);
			local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			local_player.player_controller.commands.vertical_direction = 0;
			
		} else if (quadrant == 4 || quadrant == 14) {
		//	Debug.Log(4);
			local_player.player_controller.commands.horizontal_direction = 0;
			local_player.player_controller.commands.vertical_direction = MOVE_DOWN;

		} 


	}

	private void RotateAroundBallClockwise()
	{
		int quadrant = GetQuadrant();

	//	Debug.Log(quadrant);
		
		if (quadrant == 1 || quadrant == 14) {
		//	Debug.Log(1);
			local_player.player_controller.commands.horizontal_direction = 0;
			local_player.player_controller.commands.vertical_direction = MOVE_UP;
			
		} else if (quadrant == 2 || quadrant == 12) {
			local_player.player_controller.commands.vertical_direction = 0;
			local_player.player_controller.commands.horizontal_direction = MOVE_LEFT;
			
		} else if (quadrant == 3 || quadrant == 23) {
		//	Debug.Log(3);
			local_player.player_controller.commands.horizontal_direction = 0;
			local_player.player_controller.commands.vertical_direction = MOVE_DOWN;
			
		} else if (quadrant == 4 || quadrant == 34) {
		//	Debug.Log(4);
			local_player.player_controller.commands.horizontal_direction = MOVE_RIGHT;
			local_player.player_controller.commands.vertical_direction = 0;
			
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
	//	Debug.Log(Xb + " - " + Xp);

		if(Mathf.Abs(Xb - Xp) < 0.1f) {
		
			if (Zb < Zp) 
				return 14;
			else
				return 23;

		} else if (Mathf.Abs(Zb - Zp) < 0.1f) {
		
			if (Xb > Xp) {
				//Debug.Log("12");
				return 12;
			} else {
				//Debug.Log("34");
				return 34;
			}
		
		}

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

	public void GoalScored()
	{	
		hands.gameObject.SetActive(true);
		hand_animator.Play(Celebrate_state);
		current_intention.intent = Actions.NULL;
	}

	public void GoalConceeded()
	{
		GetAngry();
	}




	public void OnGoingToArea(int area)
	{
		current_expression.expression = Expressions.GOING_TO_AREA;
		current_expression.args = area;
		Hashtable data = new Hashtable();
		data["index"] = area;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnGoingToArea", data);
	}

	public IEnumerator GoingToArea(NotificationCenter.Notification notification)
	{
		if (object.ReferenceEquals(this.player, notification.sender)) {
			Debug.Log("Agent 2 - I'm moving to position " + notification.data["index"]);
		} else {
			yield return new WaitForSeconds(1f);
			Debug.Log("Agent 1 - OK!");
		}
		
	}

	public void OnGoingToBall()
	{
		current_expression.expression = Expressions.GOING_TO_BALL;
		NotificationCenter.DefaultCenter.PostNotification(this.player, "OnGoingToBall");
	}

	public void GoingToBall(NotificationCenter.Notification notification)
	{
		if (object.ReferenceEquals(this.player, notification.sender))
			Debug.Log("Agent 1: Going for the ball");
		else {
			//Debug.Log("go");
		//	beliefs.teammate_expression = Expressions.INTEND_TO_PASS;
		}
	}

	public void AnticipateToPass(int area)
	{
		Hashtable data = new Hashtable();
		data["index"] = area;
		data["team"] = team;
		current_expression.expression = Expressions.INTEND_TO_PASS;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"AnticipateToPass", data);
		Point();
		current_intention.args = area;
		ai_manager.AgentResponse(this);
	}

	public IEnumerator AnticipateToPassReaction(NotificationCenter.Notification notification)
	{
		if (!object.ReferenceEquals(this.player, notification.sender) && this.player.team == (int)notification.data["team"]) {
			yield return new WaitForSeconds(0.5f);
			if (current_intention.intent == Actions.RECEIVE_PASS || current_area == (int)notification.data["index"])
				Debug.Log("Agent 2 - Yes");
			else {
				ThumbsUp();
				Debug.Log("Agent 2 - Wait");
			}
			beliefs.teammate_expression = Expressions.INTEND_TO_PASS;
		}
	}


	public void OnRequestPass(int area)
	{
		//expression = Expressions.REQUEST_PASS;
	//	current_expression.expression = Expressions.REQUEST_PASS;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnRequestPass");
		AskForBall();
	}

	public IEnumerator RequestPass(NotificationCenter.Notification notification)
	{
		if (!object.ReferenceEquals(this.player, notification.sender)) {
			yield return new WaitForSeconds(0.5f);
			beliefs.teammate_expression = Expressions.REQUEST_PASS;
			Appraise();
		}
	}

	private void Appraise(EnvironmentMessages message = EnvironmentMessages.NULL)
	{
		if (current_intention.intent == Actions.SCORE && beliefs.teammate_expression == Expressions.OK) {
			if (current_confidence > CONFIDENCE_THRESHOLD) {
				current_confidence += HIGH_CONFIDENCE_VALUE;
			} else {
				current_confidence += LOW_CONFIDENCE_VALUE;
			}
		} else if (current_intention.intent == Actions.SCORE && beliefs.teammate_expression == Expressions.REQUEST_PASS ) {
			if (current_confidence > CONFIDENCE_THRESHOLD) {
				current_confidence -= LOW_CONFIDENCE_VALUE;
			} else {
				GetFrustrated();
				current_confidence -= HIGH_CONFIDENCE_VALUE;
			}
		}

		if (current_expression.expression == Expressions.REQUEST_PASS && beliefs.teammate_expression == Expressions.INTEND_TO_SCORE) {
			AskForBall();
		} else if (current_expression.expression == Expressions.REQUEST_PASS && beliefs.teammate_expression == Expressions.OK) {
			OnSignalOK();
		}

		if (message == EnvironmentMessages.WALL_HIT && team == GlobalConstants.RED) {
			if (current_intention.intent == Actions.SCORE) {
				Debug.Log("I missed!!");
				GetFrustrated();
				StopPointing();
			} else if (beliefs.teammate.current_intention.intent == Actions.SCORE) {
				if (current_expression.expression == Expressions.REQUEST_PASS) {
					StopAskingForBall();
					GetAngry();
				} else if (current_confidence < 50){
					GetAngry();
				}
			}
		} else if (message == EnvironmentMessages.WALL_HIT && team == GlobalConstants.BLUE) {
			player.player_mesh.animation.CrossFade("Celebrate", 0.3f);
			GoalScored();
		}
//		Debug.Log(current_confidence);
	}

	public void OnSignalOK()
	{
		Hashtable data = new Hashtable();
		data["team"] = team;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnSignalOK", data);
	}

	public IEnumerator SignalOK(NotificationCenter.Notification notification)
	{
		yield return new WaitForSeconds(0.5f);
		if (object.ReferenceEquals(this.player, notification.sender)) {
			current_expression.expression = Expressions.OK;
			//AskForBall();
			ThumbsUp();
			Debug.Log("Agent 2 - OK");
		}
		else if (this.player.team == (int)notification.data["team"]) {
			beliefs.teammate_expression = Expressions.OK;
		//	Debug.Log("this is receiver");
		}
	}

	public void AnticipateToScore()
	{
		Hashtable data = new Hashtable();
		data["team"] = team;
		beliefs.teammate_expression = Expressions.NULL;
		current_expression.expression = Expressions.INTEND_TO_SCORE;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"AnticipateToScore", data);
		Point();
	}
	
	public IEnumerator AnticipateToScoreReaction(NotificationCenter.Notification notification)
	{
		if (!object.ReferenceEquals(this.player, notification.sender) && this.player.team == (int)notification.data["team"]) {
			yield return new WaitForSeconds(0.5f);
			//beliefs.teammate_expression = Expressions.OK;
			if (current_expression.expression == Expressions.REQUEST_PASS || desire == Desires.RECEIVE_BALL) {
				Debug.Log("Agent 2 - No!!");
				Debug.Log("asking for ball");
				OnRequestPass(current_area);
				//AskForBall();
			} else {
				beliefs.teammate.beliefs.teammate_expression = Expressions.OK;
				Debug.Log("Agent 1 - Go for it!!");
			}
		}
	
	}

	private void Point()
	{
		hands.gameObject.SetActive(true);
		hand_animator.Play(Point_state);
		/*hands.gameObject.SetActive(true);
		hands.animation["Point"].speed = 1f;
		//hands.animation["Point"].time = hands.animation["Point"].length;
		hands.animation.Play("Point");*/
	}

	private void StopPointing()
	{
		hand_animator.Play(PointEnd_state);
	}

	public void HideHands()
	{

	}

	private void AskForBall()
	{
		hands.gameObject.SetActive(true);
		hand_animator.Play(AskForBallStart_state);
		current_expression.expression = Expressions.REQUEST_PASS;
	}

	private void StopAskingForBall()
	{
		hand_animator.Play(AskForBallEnd_state);
	}

	private void ThumbsUp()
	{
		hands.gameObject.SetActive(true);
		hand_animator.Play(OKStart_state);
	}

	private void ThumbsUpEnd()
	{
		hand_animator.Play(OKEnd_state);
	}

	private void GetAngry()
	{
		foreach (Transform child in angry_steam) {
			child.particleSystem.Play();
		}
	}
	private void GetFrustrated()
	{
		sweat.particleSystem.Play();
	}

	public void OnScore()
	{
		//expression = Expressions.EXPECT_SCORE;
		current_expression.expression = Expressions.EXPECT_SCORE;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnScore");
	}

	public IEnumerator Score(NotificationCenter.Notification notification)
	{
		
		yield return new WaitForSeconds(0.1f);
		
		if (object.ReferenceEquals(this.player, notification.sender)) {
			//	hands.gameObject.SetActive(false);
			
			StopPointing();
		} else {
		//	beliefs.teammate_has_passed = true;
		//	beliefs.ball_z_prediction = PredictBallZPosition();
		}
		//	NotificationCenter.DefaultCenter.PostNotification(this.player,"OnPass");
	}

	public void OnPass()
	{
		Hashtable data = new Hashtable();
		data["team"] = team;
		NotificationCenter.DefaultCenter.PostNotification(this.player,"OnPass", data);
	}


	public IEnumerator Pass(NotificationCenter.Notification notification)
	{

		yield return new WaitForSeconds(0.1f);

		if (object.ReferenceEquals(this.player, notification.sender)) {
		//	hands.gameObject.SetActive(false);
			
			StopPointing();
		} else if (this.player.team == (int)notification.data["team"]) {
			beliefs.teammate_has_passed = true;
			beliefs.ball_z_prediction = PredictBallZPosition();
		}
	//	NotificationCenter.DefaultCenter.PostNotification(this.player,"OnPass");
	}

	public IEnumerator Shoot(NotificationCenter.Notification notification)
	{
		yield return new WaitForSeconds(2f);
		Debug.Log("MISSED GOAL");
	}
	/*
	public IEnumerator Score(NotificationCenter.Notification notification)
	{
		yield return new WaitForSeconds(2);
		if (object.ReferenceEquals(this.player, notification.sender)) {
			Debug.Log("YOU STUPID SHIT!!");
		} else {
			Debug.Log("GODDAMMIT");
		}
	}*/

	public IEnumerator OnWallHit()
	{
		Appraise(EnvironmentMessages.WALL_HIT);	
//		if (current_intention.intent == Actions.SCORE) {
//			Debug.Log("I missed!!");
//			GetFrustrated();
//			StopPointing();
//			//hand_animator.Play(Sad_state);
//			//SetActionNull();
//		} else if (beliefs.teammate.current_intention.intent == Actions.SCORE) {
//			if (current_expression.expression == Expressions.REQUEST_PASS) {
//				StopAskingForBall();
				yield return new WaitForSeconds(0.1f);
//				GetAngry();
//				Debug.Log("I told you to pass!!");
//			} else {
//				Debug.Log("You missed!!");
//			}
//		} 
	}

	private float GetDistanceBetweenTwoCoords(float z1, float z2)
	{
		//Debug.Log( Mathf.Abs( Mathf.Abs(z1) - Mathf.Abs(z2)));
		return Mathf.Abs(z1 - z2);
	}

	public float PredictBallZPosition()
	{

		Vector3 prediction = ball.transform.position;
		Vector3 velocity_normalized = ball.rigidbody.velocity.normalized;
//		Debug.Log(Mathf.Abs(prediction.x - player.transform.position.x));
		float flag = Mathf.Abs(prediction.x - player.transform.position.x);
		while (Mathf.Abs(prediction.x - player.transform.position.x) > 0.1f && flag <= Mathf.Abs(prediction.x - player.transform.position.x)) {
			prediction.x += velocity_normalized.x*0.1f;
			prediction.z += velocity_normalized.z*0.1f;
			flag = Mathf.Abs(prediction.x - player.transform.position.x);
		}
		return prediction.z;

	}

	public void SetTeammate(AI ai)
	{
		beliefs.teammate = ai;
	}

	public void SetConfidenceThreshold(int confidence_threshold)
	{
		CONFIDENCE_THRESHOLD = confidence_threshold;		
	}
	/*
	public void SetCurrentConfidence(int confidence)
	{
		current_confidence = confidence;
	}*/

	public void SetHighConfidenceValue(int confidence)
	{
		HIGH_CONFIDENCE_VALUE = confidence;
	}

	public void SetLowConfidenceValue(int confidence)
	{
		LOW_CONFIDENCE_VALUE = confidence;
	}
}