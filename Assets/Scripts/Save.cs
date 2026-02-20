using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public partial class Save : Node
{
	[Export] private Godot.Collections.Dictionary<int, int> leaderboard = new();
    private List<int> times;
	public static Save Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
    public void LoadLeaderboard(RichTextLabel timesLabel)
    {
        if (FileAccess.FileExists("user://leaderboard.save"))
        {
            
            var leaderboardFile = FileAccess.Open("user://leaderboard.save", Godot.FileAccess.ModeFlags.Read);
            
            String jsonString = leaderboardFile.GetAsText();

            var json = new Json();
            var parseResult = json.Parse(jsonString);
            if (parseResult != Error.Ok) 
            { 
                GD.PrintErr("Error parsing leaderboard: " + json.GetErrorMessage() + " at line " +
                                                           json.GetErrorLine());
            }
            
            leaderboard = JsonSerializer.Deserialize<Godot.Collections.Dictionary<int, int>>(jsonString);
        
            leaderboardFile.Close();
        }

        int newEntryValue = GameManager.Instance.totalTime;
        int position = 1;

        //check if we are on leaderboard and replace 
        foreach (var entry in leaderboard)
        {
            if (newEntryValue >= entry.Value)
            {
                position++;
            }
        }

        if (position <= 10)
        {
            leaderboard = PlaceEntry(leaderboard, new KeyValuePair<int, int>(position, newEntryValue));
        }

        times = new List<int>();
        foreach (var entry in leaderboard)
        {
            times.Add(-1);
        }


        foreach (var entry in leaderboard)
        {
            times[entry.Key-1] = entry.Value;
        }

        for (int i = 0; i < times.Count; i++)
        {
            String textToAdd = GameManager.Instance.IntToTime(times[i]);
            if (i == position - 1)
                textToAdd = " [color=green] " + textToAdd + " [/color] ";
            timesLabel.Text += "\n" + textToAdd;
        }

        //fill the remaining unfilled ranks with "-"
        for (int i = 0; i < 10 - times.Count; i++)
        {
            timesLabel.Text += "\n-";
        }
    }

    private Godot.Collections.Dictionary<int, int> PlaceEntry(
        Godot.Collections.Dictionary<int, int> currentDictionary, KeyValuePair<int, int> newEntry)
    {
        if(currentDictionary.ContainsKey(10))
            currentDictionary.Remove(10);
        
        int entryToReplace = newEntry.Key;
        Godot.Collections.Dictionary<int, int> newDictionary = new Godot.Collections.Dictionary<int, int>();

        //going backwards from last place to first and systematically adding them one position lower and deleting the previously placed entry
        for (int i = currentDictionary.Count; i > 0; i--)
        {
            currentDictionary.TryGetValue(i, out int entry);
            int newKey = i;
            if (newKey >= entryToReplace)
                newKey++;
            
            if(newKey <= 10)
                newDictionary.Add(newKey, entry);
        }

        newDictionary.Add(newEntry.Key, newEntry.Value);

        return newDictionary;
    }

	public void SaveLeaderboard()
	{
		var saveFile = FileAccess.Open("user://leaderboard.save", FileAccess.ModeFlags.Write);

		var jsonString = JsonSerializer.Serialize(leaderboard);

		saveFile.StoreLine(jsonString);

		saveFile.Close();
	}

    public void DeleteLeaderboard()
    {
        DirAccess.RemoveAbsolute("user://leaderboard.save");
    }
}
