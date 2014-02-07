using UnityEngine;
using System.Collections;

public class Hero_Selection : MonoBehaviour {

	public int min_hero_circle = 4;
	public float radius = 5f;
	public GameObject[] heroes;

	public Material team_1_material;
	public Material team_2_material;

	private int poistion = 0;
	private int num_heroes;
	private int[] heroes_positions; // heroes positions that are ocupied in the circle 
									// (relevant when the number of heroes is less than the min_hero_circle variable)
	private GameObject[] hero_instances;
	private int rotations;
	public GameObject player_controller_prefab;
	private GameObject controller_object;
	private Transform ready_light;

	private Player player;

	private Lobby lobby;

	private Game_Settings game_settings;
	private bool disable_keys = false;

	private PlayerController player_controller;

	public struct Player {
		public int hero_index;
		public string player_name;
		public int texture_id;
		public int team;

		public int controller; // used for local games
		public PhotonPlayer photon_player; // used for network games
	}

	int team;
	Vector3 initial_position;

	int texture_id;
	bool change_color = false;

	protected PlayerController.Commands commands;

	void Start () 
	{
		game_settings = GameObject.Find("Settings(Clone)").GetComponent<Game_Settings>();
		player_controller = null;

		num_heroes = heroes.Length;
		if(num_heroes < min_hero_circle) {
			num_heroes = min_hero_circle;
		}

		heroes_positions = new int[num_heroes];
		for (int i = 0; i < num_heroes; i++) {
			if(i < heroes.Length)
				heroes_positions[i] = 1;
			else heroes_positions[i] = 0;
		}
		
		hero_instances = new GameObject[heroes.Length];

		for(int i = 0; i < heroes.Length; i++) {

			GameObject hero = (GameObject)Instantiate(heroes[i], Vector3.zero, transform.rotation);
			hero.transform.parent = transform.Find("Heroes");
			hero.transform.localPosition = CirclePosition(i, num_heroes);
			hero.transform.animation.Play("Idle");
			hero.transform.animation["Idle"].time = Random.Range(0.0f, hero.transform.animation["Idle"].length);
			hero_instances[i] = hero;
		}

		rotations = 0;

		ready_light = transform.Find("ready_led").Find("Light");

	}

	public void InitializeLocalPlayer(int team, string name, int texture_id, int input_num, Lobby lobby) 
	{
		player = InitializePlayer(team, name, texture_id, lobby);
		player.controller = input_num;

		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
		player_controller.setInputNum(input_num);
	}

	public void InitializeNetworkPlayer(int team, string name, int texture_id, PhotonPlayer photon_player, Lobby lobby)
	{
		player = InitializePlayer(team, name, texture_id, lobby);
		player.photon_player = photon_player;

		PlayerController player_controller = controller_object.GetComponent<PlayerController>();
		player_controller.setInputNum(0);
	}

	public Player InitializePlayer(int team, string name, int texture_id, Lobby lobby)
	{
		Player new_player = new Player();
		new_player.player_name = name;
		new_player.texture_id = texture_id;
		new_player.team = team;
		new_player.player_name = name;

		controller_object = (GameObject)Instantiate(player_controller_prefab);
		PlayerController player_controller = controller_object.GetComponent<PlayerController>();

		commands = player_controller.GetCommands();
		transform.Find("ready_led").Find("Light").renderer.material.color = Color.red;
		this.lobby = lobby;

		change_color = true;

		return new_player;		
	}

	public void SetTeam(int team)
	{
		player = new Player();
		player.team = team;

		change_color = true;
	}


	Vector3 CirclePosition(int k, int num_positions)
	{
		return new Vector3(radius * Mathf.Cos(Mathf.PI/2 + (2 * k * Mathf.PI) / num_positions ),
		                   0,
		                   radius * Mathf.Sin(Mathf.PI/2 + (2 * k * Mathf.PI) / num_positions ));
	}

	void Rotate()
	{
		for(int i = 0; i < hero_instances.Length; i++) {
			Vector3 final_position = CirclePosition(i - rotations, num_heroes);
			hero_instances[i].transform.localPosition = Vector3.Lerp(hero_instances[i].transform.localPosition,
			                                                          final_position,
			                                                          0.1f);

			if(hero_instances[i].transform.localPosition == final_position){
				CancelInvoke("Rotate");
				lobby.HeroChanged(rotations);
			}
		}
	}

	void ShiftLeft()
	{
		int first_value = heroes_positions[0];

		for(int i = 0; i < heroes_positions.Length - 1; i++)
			heroes_positions[i] = heroes_positions[i+1];

		heroes_positions[heroes_positions.Length - 1] = first_value;
	}

	void ShiftRight()
	{
		int last_value = heroes_positions[heroes_positions.Length - 1];
		
		for(int i = heroes_positions.Length-1; i > 0; i--)
			heroes_positions[i] = heroes_positions[i-1];
		
		heroes_positions[0] = last_value;
	}

	void UpdateCommands()
	{
		if(controller_object != null) {
			if (player_controller == null) {
				 player_controller = controller_object.GetComponent<PlayerController>();
			}

			commands = player_controller.GetCommands();
		}
	}

	void Update () 
	{
		if(change_color) {

			Transform heroes = transform.Find("Heroes");
			
			foreach(Transform hero in heroes) {
				
				Transform hero_base = hero.Find("Base");
				
				if(player.team == 1)
					hero_base.renderer.material = team_1_material;
				else
					hero_base.renderer.material = team_2_material;
			}

			change_color = false;
		}

		if(! disable_keys) {
			if(commands.horizontal_direction < 0 && heroes_positions[1] == 1) {
				rotations++;
				ShiftLeft();
				InvokeRepeating("Rotate", 0, 0.01f);
			} else if (commands.horizontal_direction > 0 && heroes_positions[heroes_positions.Length - 1] == 1) {
				rotations--;
				ShiftRight();
				InvokeRepeating("Rotate", 0, 0.01f);
			} else if (commands.enter == 1) {
				ready_light.renderer.material.color = Color.green;
				((Light)ready_light.parent.Find("Halo").GetComponent<Light>()).color = Color.green;

				player.hero_index = rotations;
				disable_keys = true;

				lobby.PlayerReady(player);
			}
		
			UpdateCommands();
		}
	}
}
