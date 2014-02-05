using UnityEngine;
using System.Collections;

public class Tesla : Hero {

	private Transform magnet;
	private GameObject ball;
	private bool is_using_power;
	private Vector3 ball_pos;
	private Vector3 original_position;
	private Vector3 power_displacement;
	private bool is_velocity_zeroed = false;
	private float POWER_TIMER = 2f;
	private float POWER_COOLDOWN = 16f;

	private float last_dash;

	public Tesla(Player_Behaviour player)
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Tesla.prefab", typeof (GameObject)) as GameObject;
		this.player = player;
		is_using_power = false;
		power_displacement = Vector3.zero;
		player.setDashCooldown(POWER_COOLDOWN);
	}
	
	public override void UsePower(PlayerController.Commands commands)
	{
		if(ball == null)
			ball = GameObject.FindWithTag("ball");

		if(last_dash != commands.dash) {

			if (commands.dash != 0 && player.IsCooldownOver() && !is_using_power) {
				power_cooldown = POWER_COOLDOWN + Time.time;
//				player.resetPowerBar();
				is_using_power = true;
				player.setPowerActivatedTimer(POWER_TIMER);
				ball_pos = ball.transform.position;
				DrawMagnet();
			} else if (commands.dash != 0 && is_using_power) {
				StopPower();
			} 
		}
		if(player.IsPowerTimerOver()) {
			StopPower();
			
		}

		if (is_using_power && player.IsCollidingWithBall()) {
			if (!is_velocity_zeroed) {
				ball.transform.rigidbody.velocity = Vector3.zero;
				is_velocity_zeroed = true;
			}
			power_displacement = player.transform.position - original_position;
			//			Debug.Log(power_displacement + "-" + original_position + "-" + ball.transform.position);
			ball.transform.position += power_displacement;
		} else {
			is_velocity_zeroed = false;
		}
		
		original_position = player.transform.position;
		last_dash = commands.dash;

	}

	private void StopPower()
	{
		is_using_power = false;
		ball.transform.rigidbody.velocity = player.rigidbody.velocity;
		EraseMagnet();
		player.setPowerActivatedTimer(0f);
		player.resetPowerBar();
	}

	[RPC]
	void DrawMagnet()
	{
		magnet.particleSystem.Play();
	}

	[RPC]
	void EraseMagnet()
	{
		magnet.particleSystem.Stop();
	}
	public override void Start()
	{
		magnet = player.transform.Find("Mesh").Find("Base").Find("Magnet");
		magnet.particleSystem.Stop();
	}
}
