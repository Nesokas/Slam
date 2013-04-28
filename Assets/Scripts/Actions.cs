using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct Action
{
	public float start;
	public float end;
}

public class Actions : MonoBehaviour
{
	Dictionary<string, Action> _actions;
	Animation _animation;
	
	Action _current_action;
	
	public TextAsset actions_data;
	public string default_action;
	
	void Start()
	{
		_actions = new Dictionary<string, Action>();
		_animation = transform.parent.GetComponent<Animation>();
		
		read_actions ();
		
		_current_action = _actions[default_action];
		_animation["Default Take"].time = _current_action.start;
	}
	
	void read_actions()
	{
		string text = actions_data.text;
		string[] lines = text.Split('\n');
		
		int i = 0;
		Action action = new Action();
		string action_name = "";
		
		
        foreach (string line in lines)
        {
            switch (i){
			case 0:
				action = new Action();
				action_name = line;
				break;
			case 1:
				action.start = System.Convert.ToSingle(line);
				action.start = action.start / _animation["Default Take"].clip.frameRate;
				break;
			case 2:
				action.end = System.Convert.ToSingle(line);
				action.end = action.end / _animation["Default Take"].clip.frameRate;
				_actions.Add(action_name, action);
				break;
			}
			
			i = (i + 1) % 3;
        }
	}
	
	public void play(string action_name)
	{
		if(_actions.ContainsKey(action_name)) {
			_current_action = _actions[action_name];
			_animation["Default Take"].time = _current_action.start;
		}
	}
	
	void Update() 
	{
		if(_animation["Default Take"].time > _current_action.end) {
			_animation["Default Take"].time = _current_action.start;
		}
	}

}
