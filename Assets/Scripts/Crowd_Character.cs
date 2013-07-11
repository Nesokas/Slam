using UnityEngine;
using System.Collections;

public class Crowd_Character : MonoBehaviour {
	
	private GameObject ball;
	
	void UpdateRotation()
	{
		if(!ball)
			ball = GameObject.FindGameObjectWithTag("ball");
		else {
			var rotation = Quaternion.LookRotation(ball.transform.position - transform.position);
		    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 1000);
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRotation();
	}
}
