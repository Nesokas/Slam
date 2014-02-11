using UnityEngine;
using System.Collections;

public class PublicityBoard : MonoBehaviour {

	void Update() {

		Vector2 texture = renderer.material.mainTextureOffset;
		texture.x += 0.001f;
		renderer.material.mainTextureOffset = texture;
	}
	
}
