using UnityEngine;
using System.Collections;

public class Sam : Hero {


	public Sam()
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Sam.prefab", typeof (GameObject)) as GameObject;
	}

	public override void UsePower()
	{
	}
}
