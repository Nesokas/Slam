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
	
	/* Only one team should kickoff, the other cannot go through the midfield circle or opposing side */
	public void DisableGotoCenter(int scored_team)
	{
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
	
	public void InitializePlayerInfo(int num, int team_num, Camera m_camera)
	{
		if(num != 0) {
			gamepad_num = num;
			gamepad = true;
		}

		team = team_num;
		player_num = num;

		Player_Name name_component = transform.Find("Player_name").transform.GetComponent<Player_Name>();
		name_component.m_camera = m_camera;
		name_component.ChangeName("P" + num);
	}
	
	public void Start () {
		base.Start();
		
		GameObject[] goal_detection = GameObject.FindGameObjectsWithTag("goal_detection");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		center_planes = GameObject.FindGameObjectsWithTag("center-plane");
		center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
		center_circle_right = GameObject.FindGameObjectWithTag("center-circle-right");
				
		if(team == 1) {
			normal_material = normal_team_1_material;
			shoot_material = shoot_team_1_material;
		} else {
			normal_material = normal_team_2_material;
			shoot_material = shoot_team_2_material;
		}
		player_base.renderer.material = normal_material;
	}
	

	
	
}
