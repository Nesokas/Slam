using UnityEngine;
using System.Collections;

public class Physic : MonoBehaviour {

	Vector2 velocity;
	Vector2 max_velocity;
	float friction;
	Vector2 direction;
	Vector2 acceleration;
	float force_strenght;
	bool apply_force;

	// Use this for initialization
	void Start () {
		friction = 5f;

		max_velocity = new Vector2(200, 200);
		velocity = Vector2.zero;
	}

	// Update is called once per frame
	void Update () {


	}
}
