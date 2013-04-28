using System.Collections.Generic;

public struct Action
{
	public string name;
	public int start;
	public int end;
}

public static class Read_Actions 
{
	static void read_actions(string filename)
	{
		string[] lines = System.IO.File.ReadAllLines(filename);
		
		int i = 0;
		Action action = new Action();
		List<Action> actions = new List<Action>();
        foreach (string line in lines)
        {
            switch (i){
			case 0:
				action = new Action();
				action.name = line;
				break;
			case 1:
				action.start = System.Convert.ToInt32(line);
				break;
			case 2:
				action.end = System.Convert.ToInt32(line);
				actions.Add(action);
				break;
			}
			
			i = (i + 1) % 3;
        }
	}

}
