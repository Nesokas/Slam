using UnityEngine;
using System.Collections;

public class NetworkPreLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject loading = GameObject.Find("NetworkLoading(Clone)");
		NetworkLoading network_loading = loading.GetComponent<NetworkLoading>();

		network_loading.StartLoading();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
