using UnityEngine;
using System.Collections;

public class Network_Ball : Ball_Behaviour {
	
	public Transform observed_transform;
	
	private Predictor predictor;
	private PhotonView photonView;
	
	void Awake()
	{
//		base.Awake();
		observed_transform = transform;
		photonView = PhotonView.Get(this);
	}
	
	new void Start()
	{
		predictor = new Predictor(transform);
		base.Start();
	}
	
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(predictor == null)
			predictor = new Predictor(transform);
		predictor.OnSerializePhotonViewBall(stream, info);
	}
	
	new void Update()
	{
		base.Update();
			
		predictor.PredictBall(photonView);
		
		transform.position = predictor.getPredictedTransform().position;
			
	}
}
