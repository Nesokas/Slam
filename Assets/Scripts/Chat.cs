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

	public Font chat_font;

	bool using_chat = false;
	bool first_open_chat = false;
	string input_field = "";

	KeyCode last_key_pressed;

	List<ChatMessage> chat_messages;
	Game_Settings settings;

	public TimeSpan display_time;
	public int max_chat_messages;

	// Use this for initialization
	void Start () 
	{
		chat_messages = new List<ChatMessage>();
		display_time = TimeSpan.FromSeconds(10);

		GameObject settings_object = GameObject.FindGameObjectWithTag("settings");
		settings = settings_object.GetComponent<Game_Settings>();

		last_key_pressed = KeyCode.None;
		max_chat_messages = 10;
	}

	void DrawOutline(Rect rect, string text, Color color, GUIStyle style)
	{
		Color last_color = style.normal.textColor;
		style.normal.textColor = Color.black;
		float half_size = 1f;

		rect.x -= half_size;
		GUI.Label(rect, text, style);

		rect.x += half_size*2;
		GUI.Label(rect, text, style);
		
		rect.x -= half_size;
		rect.y -= half_size;
		GUI.Label(rect, text, style);
		
		rect.y += half_size*2;
		GUI.Label(rect, text, style);

		GUI.skin.label.fontStyle = FontStyle.Normal;
		style.normal.textColor = color;
		rect.y -= half_size;
		GUI.Label(rect, text, style);

		style.normal.textColor = last_color;
	}

	// Update is called once per frame
	void OnGUI () 
	{
		List<ChatMessage> display_messages = new List<ChatMessage>();

		for(int i = 0; i < chat_messages.Count; i++) {
			if(chat_messages[i].time_received.Add(display_time) >= DateTime.Now.TimeOfDay)
				if(display_messages.Count <= max_chat_messages)
					display_messages.Add(chat_messages[i]);
			else
				break;
		}
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(GUILayout.ExpandHeight(false));
				GUILayout.FlexibleSpace();
				float label_height = 24;
				GUILayout.BeginVertical(GUILayout.Height((max_chat_messages+1)*label_height), GUILayout.ExpandWidth(false));
					for (int i = 0; i < max_chat_messages - display_messages.Count; i++)
						GUILayout.Space (label_height);
					for(int i = 0; i < display_messages.Count; i++) {
						GUILayout.BeginHorizontal();
							GUIContent label_text = new GUIContent(display_messages[i].sender_name);
							GUIStyle label_style = new GUIStyle();
							label_style.fontSize = 15;
							label_style.font = chat_font;
							Rect rect = GUILayoutUtility.GetRect(label_text, label_style, GUILayout.ExpandWidth(false));
							rect.height = label_height;
							DrawOutline(rect, display_messages[i].sender_name, display_messages[i].sender_color, label_style);
							string text = " says: " + display_messages[i].message;
							label_text = new GUIContent(text);
							Rect message = GUILayoutUtility.GetRect(label_text, label_style);
							message.height = label_height;
							message.x = rect.x + rect.width;
							DrawOutline(message, text, GUI.color, label_style);
						GUILayout.EndHorizontal();
					}
					if(using_chat) {
						GUILayout.BeginHorizontal();
							if(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return){
								ReturnPressed();
							}

							GUI.SetNextControlName("chat");
							input_field = GUILayout.TextField(input_field, 100, GUILayout.MaxWidth(Screen.width*0.3f), GUILayout.Height(20));
							if(GUILayout.Button("Send", GUILayout.Height(20)) && input_field != "") {
								GetComponent<uLink.NetworkView>().RPC("SendMessage", uLink.RPCMode.All, input_field, settings.player_name, "red");
								input_field = "";
							}
						GUILayout.EndHorizontal();
					}
						
				GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if(first_open_chat) {
			GUI.FocusControl("chat");
			first_open_chat = false;
		}
	}

	void ReturnPressed()
	{
		if(using_chat) {
			if(input_field != "")
				GetComponent<uLink.NetworkView>().RPC("SendMessage", uLink.RPCMode.All, input_field, settings.player_name, "red");
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
			ReturnPressed();
		}

		if(Input.GetKeyUp(KeyCode.Escape)) {
			using_chat = false;
		}
	}

	[RPC]
	protected void SendMessage(string message, string sender_name, string sender_color)
	{
		ChatMessage new_message = new ChatMessage();

		new_message.message = message;
		new_message.sender_name = sender_name;
		new_message.time_received = DateTime.Now.TimeOfDay;
		new_message.sender_color = Color.red;

		chat_messages.Add(new_message);
	}
}
