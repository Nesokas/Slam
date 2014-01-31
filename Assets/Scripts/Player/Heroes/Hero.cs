using UnityEngine;
using System.Collections;

public abstract class Hero {

	protected GameObject hero_prefab;

	public abstract void UsePower();
	 
	public void InstantiateMesh(Transform player)
	{
		GameObject hero = (GameObject)MonoBehaviour.Instantiate(hero_prefab);
		hero.transform.parent = player;

		hero.transform.localPosition = Vector3.zero;
		hero.transform.localScale = Vector3.one;

		hero.transform.name = "Mesh";
	}

}
