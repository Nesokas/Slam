using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	//The pitch is divided in 18 areas. This list relates the players to each area of the pitch
	private List<Player_Behaviour>[] pitch_area_list = new List<Player_Behaviour>[18];
	private List<Hero> ai_list = new List<Hero>();
	private int red_team_bots;
	private int blue_team_bots;

	//in which of the 18 areas is the disk
	private int disk_area;

	private Game_Settings game_settings;

	private GameStarter game_starter;

	protected GameObject AI_prefab;


	void Start () {

		GameObject game_starter_object = GameObject.Find("GameStarter");
		game_starter = game_starter_object.GetComponent<GameStarter>();
		
		game_starter.SetAIManager(this);
		
		AI_prefab = Resources.Load<GameObject>("Heroes/AI");

//		GameObject settings = GameObject.FindGameObjectWithTag("settings");

//		game_settings = settings.GetComponent<Game_Settings>();

		for (int i = 0; i <= 17; i++) {

			pitch_area_list[i] = new List<Player_Behaviour>();
		
		}
	
	}

	public void InsertAI(Hero hero)
	{
		Debug.Log("added hero");
		ai_list.Add(hero);
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach (AI ai in ai_list)
			ai.Update();
	}

	public void InstantiateBot(Vector3 start_position, int team)
	{


	}

	public void test()
	{
		Debug.Log("test");
	}

	public void InsertPlayerInList(Player_Behaviour player, int index)
	{
		pitch_area_list[index].Add(player);
		//Debug.Log("added " + index);
	}

	public void RemovePlayerFromList(Player_Behaviour player, int index)
	{
		pitch_area_list[index].Add(player);
		//Debug.Log("removed " + index);
	}

	public void SetDiskArea(int index)
	{
		disk_area = index;
		//Debug.Log(index);
	}

}
