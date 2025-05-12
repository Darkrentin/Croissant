using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class FirebaseManager : Node
{
	[Export] private HttpRequest HttpRequest;

	private class ScoreTimeData { public double time { get; set; } }

	public override void _Ready()
	{
		//AddSpeedrunEntry("TestPlayer", 123.456f);
		PrintTimesToConsole();
	}

	public void AddSpeedrunEntry(string playerName, float time)
	{
		string url = $"https://shape-glitch-default-rtdb.europe-west1.firebasedatabase.app/scores/{playerName}.json";
		string Json = JsonSerializer.Serialize(new ScoreTimeData { time = Math.Round(time, 2) });
		string[] headers = { "Content-Type: application/json" };
		HttpRequest.Request(url, headers, HttpClient.Method.Put, Json);
	}

	public void PrintTimesToConsole()
	{
		HttpRequest.Request("https://shape-glitch-default-rtdb.europe-west1.firebasedatabase.app/scores.json?orderBy=\"time\"");
	}

	private void _on_http_request_request_completed(long result, long responseCode, string[] headers, byte[] body)
	{
		if (result == (long)HttpRequest.Result.Success && responseCode >= 200 && responseCode < 300)
		{
			string responseBodyString = Encoding.UTF8.GetString(body);
			var Scores = JsonSerializer.Deserialize<Dictionary<string, ScoreTimeData>>(responseBodyString);
			var SortedScores = Scores.OrderBy(Score => Score.Value.time);
			int Rank = 1;
			foreach (var Score in SortedScores)
			{
				GD.Print($"{Rank}. {Score.Key} : {FormatTime(Score.Value.time)}");
				Rank++;
			}
		}
	}

	private string FormatTime(double totalSeconds)
	{
		TimeSpan Time = TimeSpan.FromSeconds(totalSeconds);
		string FormattedTime = "";
		if (Time.TotalHours >= 1)
			FormattedTime += $"{(int)Time.TotalHours}h";
		if (Time.Minutes > 0 || Time.TotalHours >= 1)
			FormattedTime += $"{Time.Minutes:D2}m";
		FormattedTime += (Time.Seconds + Time.Milliseconds / 1000.0).ToString("00.00", CultureInfo.InvariantCulture) + "s";
		return FormattedTime;
	}

	private string Sanitize(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return "NullProPlayer";
		return Regex.Replace(input, @"[\.\$#\[\]\/ ]", "_");
	}
}