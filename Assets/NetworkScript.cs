using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {
	
	string server_IP = "127.0.0.1";
	string server_port = "8000";
	
	
	void OnGUI() 
	{
		if(Network.peerType == NetworkPeerType.Disconnected) {
			if (GUILayout.Button("Connect")) {
				Network.Connect(server_IP, int.Parse(server_port));
			}
			if (GUILayout.Button("New Server")) {
				Network.InitializeServer(32, int.Parse(server_port),false);
			}
		} else {
			if (GUILayout.Button("Disconnect")) {
				Network.Disconnect();
			}
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
