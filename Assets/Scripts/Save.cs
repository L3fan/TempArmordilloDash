using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public partial class Save : Node
{
	private Leaderboard leaderboard = new Leaderboard();
	public static Save Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
        LoadLeaderboard();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


    public void LoadLeaderboard()
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

            leaderboard = JsonSerializer.Deserialize<Leaderboard>(jsonString);

            leaderboardFile.Close();
        }
    }

    public void ShowLeaderboard(RichTextLabel namesLabel, RichTextLabel timesLabel, int newPosition = -1)
    {
        
        for (int i = 0; i < leaderboard.Count; i++)
        {
            String nameToAdd = leaderboard.names[i];
            String timeToAdd = GameManager.Instance.IntToTime(leaderboard.times[i]);

            if (i == newPosition)
            {
                nameToAdd = " [color=green] " + nameToAdd + " [/color] ";
                timeToAdd = " [color=green] " + timeToAdd + " [/color] ";
            }

            namesLabel.Text += "\n" + nameToAdd;
            timesLabel.Text += "\n" + timeToAdd;
        }

        //fill the remaining unfilled ranks with "-"
        for (int i = 0; i < 10 - leaderboard.Count; i++)
        {
            namesLabel.Text += "\n-";
            timesLabel.Text += "\n-";
        }
    }

    public int PlaceEntryOnLeaderboard(string givenName, int givenTime)
    {
        if (leaderboard.times.Count >= 10)
        {
            leaderboard.RemoveAt(10);
        }
        
        Leaderboard newLeaderboard = new Leaderboard();

        //start at last position of leaderboard and place one step higher for each already existing entry worse than the new one
        int newPlacement = leaderboard.Count;
        foreach (int entry in leaderboard.times)
        {
            if (givenTime < entry)
                newPlacement--;
        }
        
        for (int i = 0; i < newPlacement; i++)
        {
            string entryName = leaderboard.names[i];
            int entryTime = leaderboard.times[i];
            newLeaderboard.Add(entryName, entryTime);
        }
        
        newLeaderboard.Add(givenName, givenTime);
        
        for (int i = newPlacement; i < leaderboard.Count; i++)
        {
            string entryName = leaderboard.names[i];
            int entryTime = leaderboard.times[i];
            newLeaderboard.Add(entryName, entryTime);
        }

        leaderboard = newLeaderboard;

        return newPlacement;
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

[Serializable]
public class Leaderboard
{
    
    public List<string> names {get; set;}
    public List<int> times {get; set;}
    public int Count {get; set;}

    public Leaderboard()
    {
        names = new List<string>();
        times = new List<int>();
        Count = 0;
    }
    
    public Leaderboard(List<string> givenNames, List<int> givenTimes)
    {
        names = givenNames;
        times = givenTimes;
        Count = names.Count;
    }

    public void RemoveAt(int index)
    {
        names.RemoveAt(index);
        times.RemoveAt(index);
        Count--;
    }

    public void Add(string givenName, int givenTime)
    {
        names.Add(givenName);
        times.Add(givenTime);
        Count++;
    }
}
