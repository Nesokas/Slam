using UnityEngine;
using System.Collections;

public class Hero_Selection : MonoBehaviour {

	public int min_hero_circle = 4;
	public float radius = 5f;
	public GameObject[] heroes;

	private int poistion = 0;
	private int num_heroes;
	private int[] heroes_positions; // heroes positions that are ocupied in the circle 
									// (relevant when the number of heroes is less than the min_hero_circle variable)
	private GameObject[] hero_instances;
	private int rotations;

	void Start () 
	{
		num_heroes = heroes.Length;
		if(num_heroes < min_hero_circle) {
			num_heroes = min_hero_circle;
		}

		heroes_positions = new int[num_heroes];
		for (int i = 0; i < num_heroes; i++) {
			if(i < heroes.Length)
				heroes_positions[i] = 1;
			else heroes_positions[i] = 0;
		}



		hero_instances = new GameObject[heroes.Length];

		for(int i = 0; i < heroes.Length; i++) {

			GameObject hero = (GameObject)Instantiate(heroes[i], Vector3.zero, transform.rotation);
			hero.transform.parent = transform.Find("Heroes");
			hero.transform.localPosition = CirclePosition(i, num_heroes);
			hero.transform.animation.Play("Idle");
			hero.transform.animation["Idle"].time = Random.Range(0.0f, hero.transform.animation["Idle"].length);
			hero_instances[i] = hero;
		}

		rotations = 0;
	}

	Vector3 CirclePosition(int k, int num_positions)
	{
		return new Vector3(radius * Mathf.Cos(Mathf.PI/2 + (2 * k * Mathf.PI) / num_positions ),
		                   0,
		                   radius * Mathf.Sin(Mathf.PI/2 + (2 * k * Mathf.PI) / num_positions ));
	}

	void Rotate()
	{
		for(int i = 0; i < hero_instances.Length; i++) {
			Vector3 final_position = CirclePosition(i + rotations, num_heroes);
			hero_instances[i].transform.localPosition = Vector3.Lerp(hero_instances[i].transform.localPosition,
			                                                          final_position,
			                                                          0.1f);

			if(hero_instances[i].transform.localPosition == final_position)
				CancelInvoke("Rotate");
		}
	}

	void ShiftLeft()
	{
		int first_value = heroes_positions[0];

		for(int i = 0; i < heroes_positions.Length - 1; i++)
			heroes_positions[i] = heroes_positions[i+1];

		heroes_positions[heroes_positions.Length - 1] = first_value;
	}

	void ShiftRight()
	{
		int last_value = heroes_positions[heroes_positions.Length - 1];
		
		for(int i = heroes_positions.Length-1; i > 0; i--)
			heroes_positions[i] = heroes_positions[i-1];
		
		heroes_positions[0] = last_value;

		string values = "";
		
		for(int i = 0; i < heroes_positions.Length; i++)
			values += " " + heroes_positions[i] + ",";
		
		Debug.Log(values);
	}

	void Update () 
	{
		if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
			Debug.Log(heroes_positions[1]);
			if(Input.GetKeyUp(KeyCode.A) && heroes_positions[1] == 1) {
				rotations--;
				ShiftLeft();
			} else if (Input.GetKeyUp(KeyCode.D) && heroes_positions[heroes_positions.Length - 1] == 1) {
				rotations++;
				ShiftRight();
			}

			InvokeRepeating("Rotate", 0, 0.01f);
		}
	}
}
