using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {
	
	public Transform observed_transform;
	
	private Predictor predictor;
	
	void Awake()
	{
//		base.Awake();
		observed_transform = transform;
	}
	
	new void Start()
	{
		predictor = new Predictor(transform);
		base.Start();
	}
	
	public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		if(predictor == null)
			predictor = new Predictor(transform);
		predictor.OnSerializeNetworkViewBall(stream, info);
	}
	
	new void Update()
	{
		base.Update();
			
		predictor.Predict(networkView);
		
		transform.position = predictor.getPredictedTransform().position;
			
	}
}
