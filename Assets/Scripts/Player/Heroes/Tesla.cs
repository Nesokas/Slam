using UnityEngine;
using System.Collections;

public class Tesla : Hero {

	private Transform magnet;
	private Player_Behaviour player;
	private GameObject ball;
	
	public Tesla(Player_Behaviour player, GameObject ball)
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Tesla.prefab", typeof (GameObject)) as GameObject;
		this.player = player;
		this.ball = ball;
	}
	
	public override void UsePower(PlayerController.Commands commands)
	{
		if (commands.dash != 0 && (Time.time > dash_cooldown)) {
			DrawMagnet();
		}
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
