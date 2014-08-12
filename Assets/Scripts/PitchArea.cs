using UnityEngine;
using System.Collections;

public class PitchArea : MonoBehaviour {

	private AIManager AIManager;
	private int index;

	// Use this for initialization
	void Start () {
		GameObject AIManager_object = GameObject.Find("AIManager");
		AIManager = AIManager_object.GetComponent<AIManager>();

		index = int.Parse(name);

		AIManager.InsertPitchAreaCoordinates(index, this.transform.position);
		//Debug.Log(index + " - " +transform.localPosition);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("player_collider")) {

			Player_Behaviour player = collider.transform.parent.gameObject.GetComponent<Player_Behaviour>();
			player.SetCurrentArea(index); //every player knows where it is in the pitch;
		//	Debug.Log("area: " + index + "player_area: " + player.getCurrentArea());
			AIManager.InsertPlayerInList(player, index);

		}
		else if (collider.gameObject.CompareTag("ball"))

		         AIManager.SetDiskArea(index);
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.CompareTag("player_collider")) {
			AIManager.RemovePlayerFromList(collider.gameObject.GetComponent<Player_Behaviour>(), index);
		}
	}
}
