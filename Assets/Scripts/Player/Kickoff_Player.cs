using UnityEngine;
using System.Collections;

public class Kickoff_Player : Player_Behaviour {
	
	public int team = 1;
	public int player_num = 1;
	
	public Material normal_team_2_material;
	public Material shoot_team_2_material;
	
	private GameObject center_circle_left;
	private GameObject center_circle_right;
	private GameObject[] center_planes;
	
	private Vector3 initial_position;
	private NetworkPlayer owner;
	private GameObject controller_object;
	
	/* Only one team should kickoff, the other cannot go through the midfield circle or opposing side */
	public void DisableGotoCenter(NotificationCenter.Notification notification)
	{
		int scored_team = (int)notification.data["scored_team"];
		player_base = transform.Find("Base");
		Transform base_collider = player_base.Find("Collider");
		Transform shoot_collider = player_base.Find("ColliderShoot");
		if (scored_team == 0){
			if(team == 1){
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			}
		} else if (team == 1) {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			}
		} else {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
			}
		}
		for(int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider, false);
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider, false);
		}
	}
	
	public void EnableGotoCenter()
	{
		player_base = transform.Find("Base");
		Transform base_collider = player_base.Find("Collider");
		Transform shoot_collider = player_base.Find("ColliderShoot");
		for (int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider);
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider);
		}
		Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
	}
	
	public void InitializePlayerInfo(NetworkPlayer network_player, int team_num, string player_name, Vector3 position)
	{
		networkView.RPC("TellInfoToPlayers", RPCMode.All, team_num, player_name, position, network_player);
	}
	
	[RPC]
	void TellInfoToPlayers(int team_num, string name, Vector3 position, NetworkPlayer network_player)
	{
		team = team_num;
		
		owner = network_player;
		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
		name_component.m_camera = (Camera)GameObject.FindGameObjectWithTag("MainCamera").camera;
		name_component.ChangeName(name);
		
		animation.Play("Idle");
		initial_position = position;
		controller_object = GameObject.FindGameObjectWithTag("PlayerController");
	}
	
	public void Awake() 
	{
		Debug.Log("START PLAYER");
		
		NotificationCenter.DefaultCenter.AddObserver(this, "InitializePosition");
		NotificationCenter.DefaultCenter.AddObserver(this, "EnableGotoCenter");
		NotificationCenter.DefaultCenter.AddObserver(this, "DisableGotoCenter");
		
		center_planes = GameObject.FindGameObjectsWithTag("center-plane");
		center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
		center_circle_right = GameObject.FindGameObjectWithTag("center-circle-right");
	}
	
	new public void Start () {
		base.Start();
		
		if (!networkView.isMine) {	
			enabled = false;
		}
				
		if(team == 1) {
			normal_material = normal_team_1_material;
			shoot_material = shoot_team_1_material;
		} else {
			normal_material = normal_team_2_material;
			shoot_material = shoot_team_2_material;
		}
		player_base.renderer.material = normal_material;
	}	
	
	void InitializePosition()
	{
		transform.position = initial_position;
	}
	
	new void Update()
	{
		if(!ball_collision && commands.shoot != 0) {
			networkView.RPC("UpdateMaterial", RPCMode.All, true);
		} else {
			networkView.RPC("UpdateMaterial", RPCMode.All, false);
		}
		
		networkView.RPC("AskCommands", RPCMode.All);
		base.Update();
	}
	
	[RPC] 
	void UpdateMaterial(bool shoot)
	{
		if (shoot)
			player_base.renderer.material = shoot_material;
		else
			player_base.renderer.material = normal_material;
	}
	
	[RPC]
	void AskCommands()
	{
		if(Network.player == owner) {
			PlayerController player_controller = controller_object.GetComponent<PlayerController>();
			commands = player_controller.GetCommands();
			networkView.RPC("UpdateCommands", RPCMode.All, commands.horizontal_direction, commands.vertical_direction, commands.shoot, Network.player);
		}
	}
	
	[RPC]
	void UpdateCommands(float horizontal, float vertical, float shoot, NetworkPlayer network_player)
	{	
		if(network_player == owner && Network.isServer) {
			commands.horizontal_direction = horizontal;
			commands.vertical_direction = vertical;
			commands.shoot = shoot;
		}
	}
	
}
