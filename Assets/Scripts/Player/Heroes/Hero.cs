using UnityEngine;
using System.Collections;

public abstract class Hero {

	protected GameObject hero_prefab;
	protected Player_Behaviour player;
	public abstract void UsePower(PlayerController.Commands commands);
	protected float power_cooldown;

	public abstract void Start();

	public abstract void EmmitPowerFX(string type = "none");

	public void InstantiateMesh(Transform player)
	{
		GameObject hero = (GameObject)MonoBehaviour.Instantiate(hero_prefab);
		hero.transform.parent = player;

		hero.transform.localPosition = Vector3.zero;
		hero.transform.localScale = Vector3.one;

		hero.transform.name = "Mesh";
	}

	public bool IsCooldownOver()
	{
		return player.IsCooldownOver();
	}



}
