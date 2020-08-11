using UnityEngine;
using System.Collections;

public class PublicityBoard : MonoBehaviour {

	void Update() {

		Vector2 texture = GetComponent<Renderer>().material.mainTextureOffset;
		texture.x += 0.001f;
		GetComponent<Renderer>().material.mainTextureOffset = texture;
	}
	
}
