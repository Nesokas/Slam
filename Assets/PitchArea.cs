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
		Debug.Log(index);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("player_collider"))

			AIManager.InsertPlayerInList(collider.gameObject.GetComponent<Player_Behaviour>(), index);

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
