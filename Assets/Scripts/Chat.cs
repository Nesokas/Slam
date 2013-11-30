using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chat : MonoBehaviour {

	struct ChatMessage {
		public string message;
		public TimeSpan time_received;
		public string sender_name;
		public Color sender_color;
	};

	bool using_chat = false;
	bool first_open_chat = false;
	string input_field = "";

	KeyCode last_key_pressed;

	List<ChatMessage> chat_messages;
	Game_Settings settings;

	public TimeSpan display_time;

	// Use this for initialization
	void Start () 
	{
		chat_messages = new List<ChatMessage>();
		display_time = TimeSpan.FromSeconds(10);

		GameObject settings_object = GameObject.FindGameObjectWithTag("settings");
		settings = settings_object.GetComponent<Game_Settings>();

		last_key_pressed = KeyCode.None;
	}

	// Update is called once per frame
	void OnGUI () 
	{
		List<ChatMessage> display_messages = new List<ChatMessage>();

		for(int i = 0; i < chat_messages.Count; i++) {
			if(chat_messages[i].time_received.Add(display_time) >= DateTime.Now.TimeOfDay)
				display_messages.Add(chat_messages[i]);
			else
				break;
		}

		GUILayout.BeginVertical();
			for(int i = display_messages.Count - 1; i >= 0; i--) {
				GUILayout.Label(display_messages[i].sender_name + " says: " + display_messages[i].message);
			}
			if(using_chat) {
				GUILayout.BeginHorizontal();
					if(Event.current.keyCode == KeyCode.None && last_key_pressed == KeyCode.Return){
						Debug.Log("Event triggered");
						ReturnPressed();
					}

					GUI.SetNextControlName("chat");
					input_field = GUILayout.TextField(input_field, 100, GUILayout.MaxWidth(Screen.width*0.3f));
					if(GUILayout.Button("Send") && input_field != "") {
						networkView.RPC("SendMessage", RPCMode.All, input_field, settings.player_name, "red");
						input_field = "";
					}
				GUILayout.EndHorizontal();

				last_key_pressed = Event.current.keyCode;
			}
				
		GUILayout.EndVertical();

		if(first_open_chat) {
			GUI.FocusControl("chat");
			first_open_chat = false;
		}
	}

	void ReturnPressed()
	{
		if(using_chat) {
			if(input_field != "")
				networkView.RPC("SendMessage", RPCMode.All, input_field, settings.player_name, "red");
			input_field = "";
			using_chat = false;
		} else {
			using_chat = true;
			first_open_chat = true;
		}
	}

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Return)) {
			Debug.Log("Enter pressed: " + using_chat);
			ReturnPressed();
		}

		if(Input.GetKeyUp(KeyCode.Escape)) {
			using_chat = false;
		}
	}

	[RPC]
	void SendMessage(string message, string sender_name, string sender_color)
	{
		ChatMessage new_message = new ChatMessage();

		new_message.message = message;
		new_message.sender_name = sender_name;
		new_message.time_received = DateTime.Now.TimeOfDay;
		new_message.sender_color = Color.red;

		chat_messages.Add(new_message);
	}
}
