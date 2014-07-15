using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManager : MonoBehaviour {

	//The pitch is divided in 18 areas. This list relates the players to each area of the pitch
	private List<Player_Behaviour>[] pitch_area_list = new List<Player_Behaviour>[18];

	//in which of the 18 areas is the disk
	private int disk_area;

	void Start () {

		for (int i = 0; i <= 17; i++) {

			pitch_area_list[i] = new List<Player_Behaviour>();
		
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	public void InsertPlayerInList(Player_Behaviour player, int index)
	{
		pitch_area_list[index].Add(player);
		//Debug.Log("added " + index);
	}

	public void RemovePlayerFromList(Player_Behaviour player, int index)
	{
		pitch_area_list[index].Add(player);
		//Debug.Log("removed " + index);
	}

	public void SetDiskArea(int index)
	{
		disk_area = index;
		Debug.Log(index);
	}
}
