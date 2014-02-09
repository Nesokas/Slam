using UnityEngine;
using System.Collections;

public class NetworkPreLoading : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		GameObject loading = GameObject.Find("NetworkLoading(Clone)");
		NetworkLoading network_loading = loading.GetComponent<NetworkLoading>();

		Debug.Log("Pre Loading");
		StartCoroutine(network_loading.StartLoading(this));
		Debug.Log("pos loading");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
