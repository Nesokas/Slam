using UnityEngine;
using System.Collections;

public class Tesla : Hero {
	
	
	public Tesla()
	{
		hero_prefab = Resources.LoadAssetAtPath("Assets/Models/Prefab/Heroes/Tesla.prefab", typeof (GameObject)) as GameObject;
	}
	
	public override void UsePower()
	{
	}
}
