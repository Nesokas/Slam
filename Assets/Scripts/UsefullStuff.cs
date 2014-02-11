using UnityEngine;
using System.Collections;
using System.Net;

public static class UsefullStuff {

	public static void SetMSF()
	{
		IPHostEntry host;
		
		host = Dns.GetHostEntry("soccerpucks.com");
		string ip = host.AddressList[0].ToString();
		
		MasterServer.ipAddress = ip;
		MasterServer.port = 23466;
		
		Network.natFacilitatorIP = ip;
		Network.natFacilitatorPort = 50005;
	}
}
