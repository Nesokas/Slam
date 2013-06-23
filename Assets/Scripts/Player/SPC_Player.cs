using UnityEngine;
using System.Collections;

public class SPC_Player : Player_Behaviour {

	public GameObject SPC;
	private SPC1 spc1;
	private bool inside_trigger_box = false;
	
	void Start()
	{
		base.Start();
		spc1 = SPC.GetComponent<SPC1>();
	}
	
	void OnTriggerEnter(Collider collider)
	{
		base.OnTriggerEnter(collider);
		
		if(collider.tag == "trigger_box")
			inside_trigger_box = true;
		
		if(collider.tag == "ball" && !inside_trigger_box) {
			spc1.playerTouchedBall();
			Debug.Log("intriggerbox");
		}
	}
	
	/*void OnTriggerStay(Collider collider)
	{
		base.OnTriggerStay(collider);
		
		if(collider.tag == "trigger_box")
			inside_trigger_box = true;
		
		if(collider.tag == "ball" && !inside_trigger_box) {
			spc1.playerTouchedBall();
			Debug.Log("intriggerbox");
		}
	}*/
	
	void OnTriggerExit(Collider collider)
	{
		base.OnTriggerExit(collider);
		if(collider.tag == "trigger_box")
			inside_trigger_box = false;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "ball" && !inside_trigger_box)
			spc1.playerTouchedBall();
	}
}
