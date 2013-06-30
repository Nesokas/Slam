using UnityEngine;
using System.Collections;

public class TriggerBox : MonoBehaviour {
	
	public GameObject SPC;
	private SPC1 spc1;
	
	void Start()
	{
		spc1 = SPC.GetComponent<SPC1>();
	}
	
	void OnTriggerEnter(Collider collider)
	{
		spc1.enterTriggerBox(collider, transform.gameObject);
	}
	
	void OnTriggerStay(Collider collider)
	{
		spc1.stayTriggerBox(collider, transform.gameObject);
	}
	
}
