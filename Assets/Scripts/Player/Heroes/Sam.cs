using UnityEngine;
using System.Collections;

public class Sam : Hero {
	
	private Transform dash_smoke;
	private Transform dash_bar_fill;
	private float DASH_COOLDOWN = 1f;
	private float DASH_STRENGTH = 12f;
	private float last_dash;

	public Sam(Player_Behaviour player)
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Sam.prefab", typeof (GameObject)) as GameObject;
		this.player = player;

		player.setDashCooldown(DASH_COOLDOWN);
		player.setPowerActivatedTimer(0f);
	}

	public override void Start()
	{
		dash_smoke = player.transform.Find("Mesh").Find("Dash_Smoke");
	}

	public override void UsePower(PlayerController.Commands commands)
	{
		if (commands.dash != 0 && player.IsCooldownOver() && (commands.horizontal_direction != 0 || commands.vertical_direction != 0)) {
//			power_cooldown =  DASH_COOLDOWN + Time.time;
			player.transform.rigidbody.velocity *= DASH_STRENGTH;
			Debug.Log("here");
			player.resetPowerBar();
			
			// if networkView == null means localplay so we can't make an RPC
			if (player.networkView != null)
				player.networkView.RPC("EmmitDashSmoke",RPCMode.All);
			else
				EmmitDashSmoke();	
		}

		last_dash = commands.dash;
	}

	[RPC]
	void EmmitDashSmoke()
	{
		dash_smoke.particleSystem.Play();
	}



//	protected void Dash(float dash, float horizontal_direction, float vertical_direction)
//	{		
//		if (dash != 0 && (Time.time > dash_cooldown) && (horizontal_direction != 0 || vertical_direction != 0)) {
//			dash_cooldown =  DASH_COOLDOWN + Time.time;
//			rigidbody.velocity *= DASH_STRENGTH;
//			dash_bar_fill.renderer.material.color = Color.red;
//			
//			// if networkView == null means localplay so we can't make an RPC
//			if (networkView != null)
//				networkView.RPC("EmmitDashSmoke",RPCMode.All);
//			else
//				EmmitDashSmoke();	
//		}
//	}
//
//	protected virtual void VerifyDash()
//	{
//		Dash(commands.dash, commands.horizontal_direction, commands.vertical_direction);
//	}
}
