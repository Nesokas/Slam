﻿using UnityEngine;
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
	
	// team scores
	private int score_team_1;
	private int score_team_2;
	
	// finish game only once
	private bool game_finished;
	
	
	// Use this for initialization
	void Start () 
	{
		NotificationCenter.DefaultCenter.AddObserver(this, "ReleasePlayers");
		NotificationCenter.DefaultCenter.AddObserver(this, "OnGoal");
		
		time = 300.0f; // 5 minutes
		update_timer = false;
		game_finished = false;
		
		timer_0_min = timer.transform.Find("0_min").GetComponent<LED>();
		timer_1_min = timer.transform.Find("1_min").GetComponent<LED>();
		timer_0_seg = timer.transform.Find("0_seg").GetComponent<LED>();
		timer_1_seg = timer.transform.Find("1_seg").GetComponent<LED>();
		
		red_0 = red.transform.Find("0").GetComponent<LED>();
		red_1 = red.transform.Find("1").GetComponent<LED>();
		
		blue_0 = blue.transform.Find("0").GetComponent<LED>();
		blue_1 = blue.transform.Find("1").GetComponent<LED>();
		
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
	
	// update team LEDs
	void UpdateScore()
	{
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
		
		if(!game_finished) {
			if(time <= 0.0f) {
				timer_0_min.SetCurrentNumber(0);
				timer_1_min.SetCurrentNumber(0);
				
				timer_0_seg.SetCurrentNumber(0);
				timer_1_seg.SetCurrentNumber(0);
				game_finished = true;
				FinishGame();
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
	
	// Team scored
	void OnGoal(NotificationCenter.Notification notification)
	{
		update_timer = false;
		if((int)notification.data["team"] == 1) {
			score_team_1++;
		}
		else {
			score_team_2++;
		}
		UpdateScore();
	}
	
	// GameFinished
	void FinishGame()
	{
		Game_Behaviour game_behaviour = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game_Behaviour>();
		game_behaviour.TimeFinished();
	}
}