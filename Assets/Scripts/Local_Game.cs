using UnityEngine;
using System.Collections;

public class Local_Game : Game_Behaviour {

	protected override void MovePlayersToStartPositions()
	{
		ball.transform.position = ball_position;
		ball.transform.rigidbody.velocity = Vector3.zero;
		if (scored_team != 0) {
			Ball_Behaviour bb = ball.GetComponent<Ball_Behaviour>();
			bb.GameHasRestarted();
		}
		
		Hashtable data = new Hashtable();
//		Debug.Log(scored_team);
		data["scored_team"] = scored_team;
		NotificationCenter.DefaultCenter.PostNotification(this, "DisableGotoCenter", data);
		NotificationCenter.DefaultCenter.PostNotification(this, "InitializePosition");
	}
	
	private Color setIndicatorColor(int i)
	{
		switch(i)
		{
			case 0:
				return Color.white;
			case 1:
				return Color.red;
			case 2:
				return Color.blue;
			case 3:
				return Color.green;
		}
		return Color.black;
	}
	
	void Awake()
	{
		ball = (GameObject)Instantiate(ball_prefab, ball_position, transform.rotation);
		ball.transform.name = "Ball";
		Color color;
		GameObject settings =  GameObject.FindGameObjectWithTag("settings");
		
		/* if running on test mode (skipping lobby) */
		if(settings == null) {
			GameObject player = (GameObject)Instantiate(player_prefab, new Vector3(0, 0, 7.12416f), transform.rotation);
			Local_Player lp = (Local_Player)player.GetComponent<Local_Player>();
			lp.InitializePlayerInfo(1, "Test", new Vector3(0, 0, 7.12416f), 0, Color.magenta);
		/********************************************/
		} else {
			Game_Settings game_settings = settings.GetComponent<Game_Settings>();
			for(int i = 0; i < game_settings.players.Count; i++) {
				if(game_settings.players[i].team != 0) {
					GameObject player = (GameObject)Instantiate(player_prefab, game_settings.players[i].start_position, transform.rotation);
					color = setIndicatorColor(i);
						
					Local_Player lp = (Local_Player)player.GetComponent<Local_Player>();
					lp.InitializePlayerInfo(
						game_settings.players[i].team, 
						game_settings.players[i].name, 
						game_settings.players[i].start_position,
						game_settings.players[i].controller,
						color
					);
				}
			}
		}
		team_scored_message_xpos = DEFAULT_TEAM_SCORED_MESSAGE_XPOS;
		MovePlayersToStartPositions();
	}
}
