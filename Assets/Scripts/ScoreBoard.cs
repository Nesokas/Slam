using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public GameObject timer;
	public GameObject red; // red team score leds
	public GameObject blue; // blue team score leds
	
	// timer LEDs
	private LED timer_0_min;
	private LED timer_1_min;
	private LED timer_0_seg;
	private LED timer_1_seg;
	
	// Red team LEDs
	private LED red_0;
	private LED red_1;
	
	// Blue team LEDs
	private LED blue_0;
	private LED blue_1;
	
	// only update time when players touch ball
	private float time;
	private bool update_timer;
	
	// finish game only once
	private bool time_finished;
	
	
	// Use this for initialization
	void Start () 
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "ReleasePlayers");
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
		
		time = 360f; // 5 minutes
		update_timer = false;
		time_finished = false;
		
		timer_0_min = timer.transform.Find("Min_Tens").GetComponent<LED>();
		timer_1_min = timer.transform.Find("Min_Units").GetComponent<LED>();
		timer_0_seg = timer.transform.Find("Seg_Tens").GetComponent<LED>();
		timer_1_seg = timer.transform.Find("Seg_Units").GetComponent<LED>();
		
		red_0 = red.transform.Find("Tens").GetComponent<LED>();
		red_1 = red.transform.Find("Units").GetComponent<LED>();
		
		blue_0 = blue.transform.Find("Tens").GetComponent<LED>();
		blue_1 = blue.transform.Find("Units").GetComponent<LED>();
		
		UpdateTimer();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(update_timer) {
			time -= Time.deltaTime;
		}
		UpdateTimer();
	}
	
	// Players touched the ball
	void ReleasePlayers()
	{
		update_timer = true;
	}
	
	// update team score LEDs
	public void UpdateScore(int score_team_1, int score_team_2)
	{
		update_timer = false;
		
		red_0.SetCurrentNumber((int)score_team_1/10);
		red_1.SetCurrentNumber((int)score_team_1%10);
		
		blue_0.SetCurrentNumber((int)score_team_2/10);
		blue_1.SetCurrentNumber((int)score_team_2%10);
	}
		
	// updates timer LEDs
	void UpdateTimer()
	{
		float minutes = Mathf.Floor(time / 60);
		float seconds = Mathf.RoundToInt(time%60);
		
		if(!time_finished) {
			if(time <= 0.0f) {
				timer_0_min.SetCurrentNumber(0);
				timer_1_min.SetCurrentNumber(0);
				
				timer_0_seg.SetCurrentNumber(0);
				timer_1_seg.SetCurrentNumber(0);
				time_finished = true;
				TimeFinished();
			} else {
			
				if((int)seconds == 60) {
					timer_0_min.SetCurrentNumber(0);
					timer_1_min.SetCurrentNumber((int) minutes%10 + 1);
				} else {
					timer_0_min.SetCurrentNumber((int) minutes/10);
					timer_1_min.SetCurrentNumber((int) minutes%10);
				}
				
				if((int)seconds == 60) {
					timer_0_seg.SetCurrentNumber(0);
					timer_1_seg.SetCurrentNumber(0);
				} else {
					timer_0_seg.SetCurrentNumber((int) seconds/10);
					timer_1_seg.SetCurrentNumber((int) seconds%10);
				}
			}
		}
	}
	
	// GameFinished
	void TimeFinished()
	{
		Game_Behaviour game_behaviour = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game_Behaviour>();
		game_behaviour.TimeFinished();
	}
}
