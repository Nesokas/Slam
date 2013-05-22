using UnityEngine;
using System.Collections;

public class Crowd : MonoBehaviour {
	
	public void Celebrate()
	{
		foreach (Transform child in transform) {
			child.animation.Play("Celebrate");
			child.animation["Celebrate"].time = Random.Range(0.0f, child.animation["Celebrate"].length);
		}
	}
	
	public void Sad()
	{
		foreach (Transform child in transform) {
			child.animation.Play("Sad");
			child.animation["Sad"].time = Random.Range(0.0f, child.animation["Sad"].length);
		}
	}
	
	public void Idle()
	{
		foreach (Transform child in transform) {
			child.animation.Play("Idle");
			child.animation["Idle"].time = Random.Range(0.0f, child.animation["Idle"].length);
		}
	}
}
