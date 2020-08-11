using UnityEngine;
using System.Collections;

public class OtherHeroChoices : MonoBehaviour {

	public GameObject[] heroes;
	public Material team_1_material;
	public Material team_2_material;
	public int hero_index;

	private GameObject[] hero_instances;

	// Use this for initialization
	void Awake () {

		hero_instances = new GameObject[heroes.Length];
		hero_index = 0;

		Transform heroes_transform = transform.Find("Heroes");

		for(int i = 0; i < heroes.Length; i++) {
			GameObject hero = (GameObject)Instantiate(heroes[i]);

			hero.transform.parent = heroes_transform;
			hero.transform.localPosition = Vector3.zero;

			hero.transform.GetComponent<Animation>().Play("Idle");
			hero.transform.GetComponent<Animation>()["Idle"].time = Random.Range(0.0f, hero.transform.GetComponent<Animation>()["Idle"].length);

			if(i != 0)
				hero.SetActive(false);

			hero_instances[i] = hero;
		}
	}

	public void ChangeTeam(int team)
	{
		Material team_material;
		Transform heroes_transform = transform.Find("Heroes");

		if(team == 1)
			team_material = team_1_material;
		else
			team_material = team_2_material;
		
		foreach(Transform hero in heroes_transform) {
			Transform hero_base = hero.Find("Base");
			hero_base.GetComponent<Renderer>().material = team_material;
		}

	}
	
	public void ChangeHero(int hero_index)
	{
		foreach(GameObject hero in hero_instances) {
			hero.SetActive(false);
		}

		hero_instances[hero_index].SetActive(true);
		hero_instances[hero_index].transform.GetComponent<Animation>().Play("Idle");
		hero_instances[hero_index].transform.GetComponent<Animation>()["Idle"].time = Random.Range(0.0f, hero_instances[hero_index].transform.GetComponent<Animation>()["Idle"].length);
		this.hero_index = hero_index;
	}
}
