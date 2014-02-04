using UnityEngine;
using System.Collections;

public class Tesla : Hero {

	private Transform magnet;
	private Player_Behaviour player;
	private GameObject ball;
	private bool is_using_power;
	private Vector3 ball_pos;
	private Vector3 original_position;
	private Vector3 power_displacement;
	private bool is_velocity_zeroed = false;

	public Tesla(Player_Behaviour player)
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Tesla.prefab", typeof (GameObject)) as GameObject;
		this.player = player;
		is_using_power = false;
		power_displacement = Vector3.zero;
	}
	
	public override void UsePower(PlayerController.Commands commands)
	{
		if(ball == null)
			ball = GameObject.FindWithTag("ball");

		if (commands.dash != 0 && (Time.time > dash_cooldown) && !is_using_power) {
			is_using_power = true;
			ball_pos = ball.transform.position;
			DrawMagnet();
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
	}

	[RPC]
	void DrawMagnet()
	{
		magnet.particleSystem.Play();
	}

	public override void Start()
	{
		magnet = player.transform.Find("Mesh").Find("Base").Find("Magnet");
		magnet.particleSystem.Stop();
	}
}
