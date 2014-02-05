using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crowd : MonoBehaviour {

	public GameObject center;
	public int team;
	public GameObject[] heroes;
	public Material team_1_material;
	public Material team_2_material;
	

	private List<GameObject> all_fans;
	private List<GameObject> teslas;
	private bool activate_crowd = true;

	void Start()
	{
		all_fans = new List<GameObject>();
		teslas = new List<GameObject>();

		Material team_material;

		if(team == 1)
			team_material = team_1_material;
		else
			team_material = team_2_material;
			
		foreach(Transform fan in transform) {

			GameObject hero_to_instanciate = heroes[Random.Range(0, heroes.Length)];

			GameObject hero = (GameObject)Instantiate(hero_to_instanciate);
			hero.transform.parent = fan;
			hero.transform.localPosition = Vector3.zero;
			hero.transform.localScale = Vector3.one;
			hero.transform.localRotation = Quaternion.Euler(0, 180, 0);

			hero.animation.Play("Idle");
			hero.transform.animation["Idle"].time = Random.Range(0.0f, hero.transform.animation["Idle"].length);

			Transform hero_object = fan.Find(hero_to_instanciate.name + "(Clone)");
			Transform hero_base = hero_object.Find("Base");

			hero_base.renderer.material = team_material;
			all_fans.Add(fan.gameObject);

			if(hero_to_instanciate.name == "Tesla") {
				DeactivateTeslaEffects(hero_object.gameObject);
				teslas.Add(hero_object.gameObject);
			}

			Fan_Behaviour fan_behaviour = fan.GetComponent<Fan_Behaviour>();
			fan_behaviour.HeroStarted(center);
		}
	}

	void DeactivateTeslaEffects(GameObject tesla)
	{
		// Deactivate Sparkles
		//GameObject sparkles = tesla.transform.Find("Bulb").GetChild(0).gameObject;
		//sparkles.SetActive(false);

		// Deactivate Magnet
		GameObject magnet = tesla.transform.Find("Base/Magnet").gameObject;
		magnet.SetActive(false);
	}
	
	void Update() 
	{
		// randomly chooce a fan to cheer for his team
		if(Random.Range(0,20) == 0) {
			GameObject fan = all_fans[Random.Range(0, all_fans.Count)];
			
			Fan_Behaviour fan_behaviour = fan.GetComponent<Fan_Behaviour>();
			StartCoroutine(fan_behaviour.Celebrate());
		}

		if(Input.GetKeyUp(KeyCode.Comma)) {
			foreach(GameObject tesla in teslas) {
				GameObject sparkles = tesla.transform.Find("Bulb").GetChild(0).gameObject;
				sparkles.SetActive(false);
			}
		}

		if(Input.GetKeyDown(KeyCode.M)) {
			activate_crowd = !activate_crowd;
			foreach(Transform child in transform) {
				child.gameObject.SetActive(activate_crowd);
			}
		}
	}
}
