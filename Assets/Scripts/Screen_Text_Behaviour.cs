using UnityEngine;
using System.Collections;

public class Screen_Text_Behaviour : MonoBehaviour {
	
	public GameObject screen_score;
	public GameObject main_game_object;
	
	private int score_team_1 = 0;
	private int score_team_2 = 0;
	
	public void team_scored(int team)
	{
		if(team == 1)
			score_team_1++;
		else score_team_2++;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		TextMesh score = screen_score.GetComponent<TextMesh>();
		score.text = score_team_1 + "-" + score_team_2;
	}
}
