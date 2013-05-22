using UnityEngine;
using System.Collections;

public class Screen_Text_Behaviour : MonoBehaviour {
	
	public GameObject screen_score;
	public GameObject main_game_object;
	
	public GameObject team_score_1;
	public GameObject team_score_2;
	
	public Material team1_color;
	public Material team2_color;
	
	public GameObject score_red;
	public GameObject score_blue;
	
	public AudioClip winning;
	
	private int score_team_1 = 0;
	private int score_team_2 = 0;
	
	private Game_Behaviour game_behaviour;
	private bool is_celebrating = false;
	private int team_celebrating;
	
	private float starting_position_1 = -5.5f;
	private float starting_position_2 = 5.4f;
	private float final_position = -0.1f;
	private float speed = 0.1f;
	private bool text_in = false;
	private int wait = 100;
	private int time_to_stop = 0;
	
	public void TeamScored(int team)
	{
		if(!is_celebrating){
			if(team == 1) {
				score_team_1++;
				ChangeScoreText("Red Team Scores", team1_color.color);
				game_behaviour.ScoreTeam(1);
				team_celebrating = 1;
			} else {
				score_team_2++;
				ChangeScoreText("Blue Team Scores", team2_color.color);
				game_behaviour.ScoreTeam(2);
				team_celebrating = 2;
			}
			
			is_celebrating = true;
		}
	}
	
	void ChangeScoreText(string text, Color color)
	{
		TextMesh scoreText1 = team_score_1.GetComponent<TextMesh>();;
		TextMesh scoreText2 = team_score_2.GetComponent<TextMesh>();;
		
		scoreText1.text = text;
		scoreText1.renderer.material.color = color;
		scoreText2.text = text;
		scoreText2.renderer.material.color = color;
		
		text_in = true;
	}
	
	public int StopCelebration()
	{
		is_celebrating = false;
		if(score_team_1 == 5) {
			ChangeScoreText("Red Team WINS", team1_color.color);
			time_to_stop = 0;
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(winning, Vector3.zero);
			return 1;
		} else if(score_team_2 == 5) {
			ChangeScoreText("Blue Team WINS", team2_color.color);
			time_to_stop = 0;
			is_celebrating = true;
			AudioSource.PlayClipAtPoint(winning, Vector3.zero);
			return 2;
		}
		
		return 0;
	}
	
	// Use this for initialization
	void Start () {
		score_red.renderer.material.color = team1_color.color;
		score_blue.renderer.material.color = team2_color.color;
		game_behaviour = main_game_object.GetComponent<Game_Behaviour>();
	}
	
	void PushScreenText()
	{
		if(text_in) {
			if(team_score_1.transform.position.z < final_position) {
				Vector3 new_position = team_score_1.transform.position;
				new_position.z = new_position.z + speed;
				team_score_1.transform.position = new_position;
				
			} else if(team_score_1.transform.position.z >= final_position){
				Vector3 new_position = team_score_1.transform.position;
				new_position.z = final_position;
				team_score_1.transform.position = new_position;
			}
			
			if(team_score_2.transform.position.z > final_position) {
				Vector3 new_position = team_score_2.transform.position;
				new_position.z = new_position.z - speed;
				team_score_2.transform.position = new_position;
				
			} else if(team_score_2.transform.position.z <= final_position){
				Vector3 new_position = team_score_2.transform.position;
				new_position.z = final_position;
				team_score_2.transform.position = new_position;
			}
			
			if(team_score_1.transform.position.z == team_score_2.transform.position.z)
				text_in = false;
			
		} else if(wait != time_to_stop) {
			time_to_stop++;
		} else if(wait == time_to_stop) {
			if(team_score_1.transform.position.z > starting_position_1) {
				Vector3 new_position = team_score_1.transform.position;
				new_position.z = new_position.z - speed;
				team_score_1.transform.position = new_position;
				
			} else if(team_score_1.transform.position.z <= starting_position_1){
				Vector3 new_position = team_score_1.transform.position;
				new_position.z = starting_position_1;
				team_score_1.transform.position = new_position;
			}
			
			if(team_score_2.transform.position.z < starting_position_2) {
				Vector3 new_position = team_score_2.transform.position;
				new_position.z = new_position.z + speed;
				team_score_2.transform.position = new_position;
				
			} else if(team_score_2.transform.position.z >= starting_position_2){
				Vector3 new_position = team_score_2.transform.position;
				new_position.z = starting_position_2;
				team_score_2.transform.position = new_position;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		TextMesh text_score_team_1 = score_red.GetComponent<TextMesh>();
		TextMesh text_score_team_2 = score_blue.GetComponent<TextMesh>();
		
		text_score_team_1.text = score_team_1.ToString();
		text_score_team_2.text = score_team_2.ToString();
		
		if(is_celebrating)
			PushScreenText();
		else time_to_stop = 0;
		
	}
}
