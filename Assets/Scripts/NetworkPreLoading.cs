using UnityEngine;
using System.Collections;

public class NetworkPreLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		GameObject loading = GameObject.Find("NetworkLoading(Clone)");
		NetworkLoading network_loading = loading.GetComponent<NetworkLoading>();

		StartCoroutine(network_loading.StartLoading(this));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
