using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AIManager : MonoBehaviour {

	//The pitch is divided in 18 areas. This list relates the players to each area of the pitch
	private List<Hero>[] pitch_area_list = new List<Hero>[18];

	//The list of every hero instantiated. AI in this list will run the update()
	private List<Hero> hero_list = new List<Hero>();
	private List<AI> red_list = new List<AI>();
	private List<AI> blue_list = new List<AI>();

	//which heroes are in each of the three flanks
	private List<Hero>[] top_flank_heroes = new List<Hero>[6];
	private List<Hero>[] mid_flank_heroes = new List<Hero>[6];
	private List<Hero>[] bottom_flank_heroes = new List<Hero>[6];

	//If more than one player is in possession, than the involved players are fighting for possession
	private List<Hero> players_in_possession = new List<Hero>();

	//The player who is intending to grab possession
	private Hero red_going_for_ball;
	private Hero blue_going_for_ball;

	//distance from which the player is considered to be in possession
	private float possession_distance_threshold = 1.25f;

	private GameObject ball;

	//in which of the 18 areas is the disk
	private int disk_area;

	private Game_Settings game_settings;

	//private GameStarter game_starter;

	protected GameObject AI_prefab;

	private Vector3[] pitch_area_coordinates = new Vector3[18];

	private GameObject red_team_goal;

	private GameObject blue_team_goal;

	private Hero red_closer_to_ball;
	private Hero blue_closer_to_ball;

	int current_scene;

	int script_step = -1;

	bool response_1 = false;
	bool response_2 = false;
	bool running_script = false;

	void Awake() {

	//	GameObject game_starter_object = GameObject.Find("GameStarter");
	//	game_starter = game_starter_object.GetComponent<GameStarter>();

	//	game_starter.SetAIManager(this);

		ball = GameObject.FindGameObjectWithTag("ball");

		AI_prefab = Resources.Load<GameObject>("Heroes/AI");

		for (int i = 0; i <= 17; i++) {

			pitch_area_list[i] = new List<Hero>();
		
		}

		for (int i = 0; i < 6; i++) {
			top_flank_heroes[i] = new List<Hero>();
			mid_flank_heroes[i] = new List<Hero>();
			bottom_flank_heroes[i] = new List<Hero>();
		}

		
		red_team_goal = GameObject.Find("Score_Team1");
		blue_team_goal = GameObject.Find("Score_Team2");

		if (Application.loadedLevelName == "scene_1") {
			current_scene = 1;
		}
	}

	public IEnumerator script_1()
	{
		AI a1 = red_list[0];
		AI a2 = red_list[1];

		a1.SetActionDribbleToArea(5);
		a2.SetActionGoToArea(3);
		while(response_1 != true || response_2 != true){//HACK!! WHY THE FUCK IS THIS || BEHAVING LIKE A &&?????
			yield return null;
		}

		response_1 = false;
		response_2 = false;
		a1.SetActionPass();
		a2.SetActionReceivePass();
		while(response_1 != true || response_2 != true) { 
			yield return null;
		}
		response_1 = false;
		response_2 = false;
		a1.SetActionNull();
		a2.SetActionScore();
		while(response_1 != true || response_2 != true) {//HACK!! WHY THE FUCK IS THIS || BEHAVING LIKE A &&?????
			yield return null;
		}
//		response_1 = false;
//		response_2 = false;
//		a1.OnIntentToPass();
//		a2.SetActionScore();
//		while(response_1 != true || response_2 != true) {//HACK!! WHY THE FUCK IS THIS || BEHAVING LIKE A &&?????
//			yield return null;
//		}
	}

	/*public WaitForSeconds WaitForAI()
	{
		while(response_1 != true && response_2 != true){
			return new WaitForSeconds(0.5f);
		}
		response_1 = false;
		response_2 = false;
		return null;
	}*/

	public void AgentResponse(AI agent)
	{
		int team = agent.GetTeam();

		if (team == GlobalConstants.RED) {
			if (object.ReferenceEquals(agent, red_list[0])) {
			    response_1 = true;
			}
			else {
				response_2 = true;
			}
		}

	}

	public void ScriptStepComplete()
	{
		AI a1 = red_list[0];
		AI a2 = red_list[1];

		if (a1.GetCurrentStep() <= script_step && a2.GetCurrentStep() <= script_step)
			script_step--;
	}

	public Vector3 GetRedTeamGoalPosition()
	{
		return red_team_goal.transform.position;
	}

	public float GoalWidth()
	{
		return red_team_goal.transform.lossyScale.x;
	}

	public Vector3 GetBlueTeamGoalPosition()
	{
		return blue_team_goal.transform.position;
	}

	public void SetGoingForBall(Hero hero)
	{
		if (hero.GetTeam() == GlobalConstants.RED)
			red_going_for_ball = hero;
		else
			blue_going_for_ball = hero;
	}

	public void ResetGoingForBall(Hero hero)
	{
		if (hero.GetTeam() == GlobalConstants.RED)
			red_going_for_ball = null;
		else
			blue_going_for_ball = null;
	}

	public bool GetGoingForBall(int team)
	{
		if (red_going_for_ball != null && team == red_going_for_ball.GetTeam())
			return true;
		else if (blue_going_for_ball != null && team == blue_going_for_ball.GetTeam())
			return true;
		else
			return false;

	}


	public void InsertPlayerInPossession(Hero hero)
	{
		if (!players_in_possession.Contains(hero)) {
			players_in_possession.Add(hero);
		}
	//	Debug.Log("inserting");
//		Debug.Log(players_in_possession.Count);
	}

	public void RemovePlayerInPossession(Hero hero)
	{
		players_in_possession.Remove(hero);
	}

	public List<Hero> GetPlayersInPossession()
	{
	//	Debug.Log(players_in_possession.Count);
		return players_in_possession;
	}

	public void InsertHero(Hero hero)
	{
		hero_list.Add(hero);
		if (hero.IsAI()) {
			if (hero.GetTeam() == GlobalConstants.RED)
				red_list.Add((AI)hero);
			else
				blue_list.Add((AI)hero);
		}

		if (red_list.Count == 2) {
			red_list[0].SetTeammate(red_list[1]);
			red_list[1].SetTeammate(red_list[0]);
		}
	}

	public void PrintHeroList()
	{
		foreach(Hero hero in hero_list)
			Debug.Log(hero.GetTeam());
		Debug.Log("----------------------------------");
	}

	void FixedUpdate() 
	{
		int i = 0;
		foreach (Hero hero in hero_list) {
			float distance_to_ball = FindDistanceToBall(hero.GetPosition());
			SetHeroCloserToBall(hero, distance_to_ball);
			if (hero.IsAI()) {
				hero.Update();
			}
			if (distance_to_ball < possession_distance_threshold) {
				InsertPlayerInPossession(hero);
			} else {
				RemovePlayerInPossession(hero);
			}
			i++;
//			if ((hero_closer_to_ball == null) || (distance_to_ball < FindDistanceToBall(hero_closer_to_ball.GetPosition()))) {
//				hero_closer_to_ball = hero;
//			}

		}
		//Debug.Log(red_list.Count + " - " + hero_list.Count);
		if (!running_script) {
			running_script = true;
			StartCoroutine (script_1());
		}
	}

	private void SetHeroCloserToBall(Hero hero, float distance_to_ball)
	{

		if (hero.GetTeam() == GlobalConstants.RED) {
		
			if ((red_closer_to_ball == null) || (distance_to_ball < FindDistanceToBall(red_closer_to_ball.GetPosition())))
				red_closer_to_ball = hero;
		
		} else if (hero.GetTeam() == GlobalConstants.BLUE) {
			if ((blue_closer_to_ball == null) || (distance_to_ball < FindDistanceToBall(blue_closer_to_ball.GetPosition())))
				blue_closer_to_ball = hero;
		
		}
		

	}

	public bool HeroHasBall(Hero hero)
	{
		float distance_to_ball = FindDistanceToBall(hero.GetPosition());

		if (distance_to_ball < possession_distance_threshold)
			return true;
		else
			return false;
	}

	public Hero GetHeroCloserToBall(int team)
	{
		if (team == GlobalConstants.RED)
			return red_closer_to_ball;
		else
			return blue_closer_to_ball;
	}

	public void InsertPitchAreaCoordinates(int index, Vector3 pos)
	{
		pitch_area_coordinates[index] = pos;
	}

	public void InstantiateBot(Vector3 start_position, int team)
	{


	}

	public Vector3 GetPitchAreaCoords(int index)
	{
		return pitch_area_coordinates[index];
	}

	public void PrintPitchAreaCoords()
	{
		for (int i = 0; i < 18; i++)
			Debug.Log(i + " - " + pitch_area_coordinates[i]);
	}

	public void InsertHeroInList(Hero hero, int index)
	{
		pitch_area_list[index].Add(hero);
		SetHeroeFlank(hero, index);
		hero.SetCurrentArea(index);
	//	Debug.Log(index);
	}
	
	public void RemoveHeroFromList(Hero hero, int index)
	{
		pitch_area_list[index].Remove(hero);
		//Debug.Log("removed " + index);
	}

	public List<Hero> GetPlayerListFromArea(int index)
	{
		return pitch_area_list[index];
	}

	public void SetDiskArea(int index)
	{
		disk_area = index;
		//Debug.Log(index);
	}

	private void SetHeroeFlank(Hero hero, int index)
	{
		int flank = AreaToFlank(index);
		int previous_area = hero.GetCurrentArea();
		int previous_flank = AreaToFlank(previous_area);
		
		if (previous_flank == GlobalConstants.BOTTOM_FLANK)
			bottom_flank_heroes[previous_area/3].Remove(hero);

		if (previous_flank == GlobalConstants.MID_FLANK)
			mid_flank_heroes[previous_area/3].Remove(hero);

		if (previous_flank == GlobalConstants.TOP_FLANK)
			top_flank_heroes[previous_area/3].Remove(hero);

		if (flank == GlobalConstants.BOTTOM_FLANK)
			bottom_flank_heroes[index/3].Add(hero);

		else if (flank == GlobalConstants.MID_FLANK)
			mid_flank_heroes[index/3].Add(hero);

		else if (flank == GlobalConstants.TOP_FLANK)
			top_flank_heroes[index/3].Add(hero);
		
	}

	public List<Hero>[] GetTopFlankHeroes()
	{
		return top_flank_heroes;
	}

	public List<Hero>[] GetMidFlankHeroes()
	{
		return mid_flank_heroes;
	}

	public List<Hero>[] GetBottomFlankHeroes()
	{
		return bottom_flank_heroes;
	}

	public int GetTeamInPossession()
	{
		int team = 0;

		foreach(Hero hero in players_in_possession) {
		
			int hero_team = hero.GetTeam();

			if (team == 0)
				team = hero_team;
			else if (hero_team != team)
				return -1; //meaning more than one player is disputing the ball
		}

		return team;
	}

	// The vector it returns will be (RED,0,-1) if the Top flank has at least a red teammate and
	// no opponent in the flank, the Mid has both Red and Blue, and Bottom has no one.
	public Vector3 IsTeammateAloneInFlanks(Hero hero)
	{
		Vector3 flanks = new Vector3(-1,-1,-1);

		flanks.x = IsTeammateAloneInTopFlank(hero);
		flanks.y = IsTeammateAloneInMidFlank(hero);
		flanks.z = IsTeammateAloneInBottomFlank(hero);

		//Debug.Log(flanks);

		return flanks;

	}

	private int IsTeammateAloneInTopFlank(Hero self)
	{
		int flank = -1;

		for (int i = 0; i < 6; i++) {
			foreach (Hero hero in top_flank_heroes[i]) {
				if (!hero.Equals(self)) {
					if (hero.GetTeam() == GlobalConstants.RED) {
						if (flank == -1 || flank == GlobalConstants.RED) {
							flank = GlobalConstants.RED;
						} else
							flank = 0;
					} else if (hero.GetTeam() == GlobalConstants.BLUE)
						if (flank == -1 || flank == GlobalConstants.BLUE)
							flank = GlobalConstants.BLUE;
						else
							flank = 0; 
				}
			} 
		} 
		return flank;

	}

	private int IsTeammateAloneInMidFlank(Hero self) 
	{
		int flank = -1;
		
		for (int i = 0; i < 6; i++)
			foreach (Hero hero in mid_flank_heroes[i]) {
				if (!hero.Equals(self)) {
					if (hero.GetTeam() == GlobalConstants.RED)
						if (flank == -1 || flank == GlobalConstants.RED)
							flank = GlobalConstants.RED;
						else
							flank = 0;
					else if (hero.GetTeam() == GlobalConstants.BLUE)
						if (flank == -1 || flank == GlobalConstants.BLUE)
							flank = GlobalConstants.BLUE;
						else
							flank = 0;
				}

		}
				return flank;
		
	}

	private int IsTeammateAloneInBottomFlank(Hero self)
	{
		int flank = -1;
		
		for (int i = 0; i < 6; i++)
			foreach (Hero hero in bottom_flank_heroes[i])
				if (!hero.Equals(self)) {
					if (hero.GetTeam() == GlobalConstants.RED)
						if (flank == -1 || flank == GlobalConstants.RED)
							flank = GlobalConstants.RED;
						else
							flank = 0;
					else if (hero.GetTeam() == GlobalConstants.BLUE)
						if (flank == -1 || flank == GlobalConstants.BLUE)
							flank = GlobalConstants.BLUE;
						else
							flank = 0;
				}
		
		return flank;
		
	}


	// given an area, it returns the flank
	public int AreaToFlank(int area) 
	{
		int flank;
		
		for (int i = 0; i < 6; i++)
			if (area == 3*i)
				return GlobalConstants.BOTTOM_FLANK;
		else if (area == 3*i+1)
			return GlobalConstants.MID_FLANK;
		else if (area == 3*i+2)
			return GlobalConstants.TOP_FLANK;
		return -1;
		
	}

	private float FindDistanceToBall(Vector3 position)
	{
		float x;
		float z;

		if (ball == null)
			ball = GameObject.FindGameObjectWithTag("ball");
		
		x = position.x - ball.transform.position.x;
		z = position.z - ball.transform.position.z;
		
		return Mathf.Abs(Mathf.Sqrt(x*x + z*z));
	}



}
