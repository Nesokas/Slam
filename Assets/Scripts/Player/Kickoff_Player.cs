using UnityEngine;
using System.Collections;

public class Kickoff_Player : Player_Behaviour {
	
	public int team = 1;
	public int player_num = 1;
	
	public Material normal_team_2_material;
	public Material shoot_team_2_material;
	
	protected GameObject center_circle_left;
	protected GameObject center_circle_right;
	protected GameObject[] center_planes;

	public Vector3 initial_position;
	protected GameObject controller_object;



	public void DisableGotoCenter(int scored_team)
	{
		player_base = transform.Find("Mesh").Find("Base");
		Transform base_collider = transform.Find("Collider");
		Transform shoot_collider = transform.Find("ColliderShoot");
		Transform colliderAIPossession = transform.Find("ColliderAIPossession");
		
		if (scored_team == 0){
			if(team == 1){
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider);
			} else {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider);
			}
		} else if (team == 1) {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider);
			}
		} else {
			if (scored_team == 1) {
				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider);
			} else {
				Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider, false);
				Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider, false);

				Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
				Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider);
			}
		}
		for(int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider, false);
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider, false);
			Physics.IgnoreCollision(center_planes[i].collider, colliderAIPossession.collider, false);
		}
	}
	
	/* Only one team should kickoff, the other cannot go through the midfield circle or opposing side */
	public void DisableGotoCenter(NotificationCenter.Notification notification)
	{
		int scored_team = (int)notification.data["scored_team"];	
		DisableGotoCenter(scored_team);
	}
	
	public void ReleasePlayers()
	{
		player_base = transform.Find("Mesh").Find("Base");
		Transform base_collider = transform.Find("Collider");
		Transform shoot_collider = transform.Find("ColliderShoot");
		Transform colliderAIPossession = transform.Find("ColliderAIPossession");

		for (int i = 0; i < center_planes.Length; i++) {
			Physics.IgnoreCollision(center_planes[i].collider, base_collider.collider);
			Physics.IgnoreCollision(center_planes[i].collider, shoot_collider.collider);
			Physics.IgnoreCollision(center_planes[i].collider, colliderAIPossession.collider);
		}
		Physics.IgnoreCollision(center_circle_left.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_left.collider, shoot_collider.collider);
		Physics.IgnoreCollision(center_circle_left.collider, colliderAIPossession.collider);

		Physics.IgnoreCollision(center_circle_right.collider, base_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, shoot_collider.collider);
		Physics.IgnoreCollision(center_circle_right.collider, colliderAIPossession.collider);
	}
	
	public void Awake() 
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "InitializePosition");
		NotificationCenter.DefaultCenter.AddObserver(this, "ReleasePlayers");
		NotificationCenter.DefaultCenter.AddObserver(this, "DisableGotoCenter");
		NotificationCenter.DefaultCenter.AddObserver(this, "ChangeReaction");
		NotificationCenter.DefaultCenter.AddObserver(this, "StopCelebration");
		
		center_planes = GameObject.FindGameObjectsWithTag("center-plane");
		center_circle_left = GameObject.FindGameObjectWithTag("center-circle-left");
		center_circle_right = GameObject.FindGameObjectWithTag("center-circle-right");

		base.Awake();
	}
	
	new public void Start () {
		base.Start();
				
		if(team == 1) {
			normal_material = normal_team_1_material;
			shoot_material = shoot_team_1_material;
		} else {
			normal_material = normal_team_2_material;
			shoot_material = shoot_team_2_material;
		}
		player_base.renderer.material = normal_material;
	}	
	
	public void InitializePosition()
	{
		transform.position = initial_position;
	}
	
	public int GetTeam()
	{
		return team;
	}

	protected void InstantiateHero(int hero_index)
	{
		switch(hero_index) {
		case 0:
			hero = new Sam(this);
			break;
		case 1:
			hero = new Tesla(this);
			break;
		case 2:
			hero = new AI(this);
			break;
		}
		hero.InstantiateMesh(this.transform);
		
		hero.Start();
	}
	
}
