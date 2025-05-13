using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class ScoreboardWindow : FloatWindow
{
	[Export] private string DatabaseLink = "https://shape-glitch-default-rtdb.europe-west1.firebasedatabase.app/scores";
	[Export] private HttpRequest HttpRequest;

	[Export] private RichTextLabel TimeLabel;
	[Export] private Label PersonalBestLabel;
	[Export] private LineEdit UsernameEntry;
	[Export] private RichTextLabel ScoreboardText;
	[Export] private Button SubmitButton;
	[Export] private Button ShowScoreboardButton;
	[Export] private Control EndResultsContainer;
	[Export] private Control ScoreboardContainer;
	[Export] private Control WaitingScreenContainer;
	private string EntryPlayerName = "";
	private double RunTime;
	private string FormattedScoreboard = "";
	private bool AlreadyAsked = false;

	public override void _Ready()
	{
		base._Ready();
		Title = "Scoreboard";
		EndResultsContainer.Visible = true;
		ScoreboardContainer.Visible = false;
		WaitingScreenContainer.Visible = false;
		Size = new Vector2I(500, 500);

		RunTime = 2232.22f;

		string formattedCurrentRunTime = FormatTime(RunTime);

		GD.Print("Current run time: " + GameManager.PersonalBestTime);
		if (RunTime < GameManager.PersonalBestTime)
		{
			StartShake(0.5f, 10);
			PersonalBestLabel.Text = "New personal best time!";
			TimeLabel.Text = $"[wave amp=15 freq=5]{formattedCurrentRunTime}[/wave]\n";
			GameManager.PersonalBestTime = RunTime;
			GameManager.SaveData.Save();
		}
		else
		{
			PersonalBestLabel.Text = $"Personal best time : {FormatTime(GameManager.PersonalBestTime)}";
			TimeLabel.Text = $"[wave amp=15 freq=5]{formattedCurrentRunTime}[/wave]\n";
		}

		//AddRunEntry("TestPlayer", 123.456f);
		//GetScoreboard();
	}

	public override void OnClose()
	{
		GetTree().Quit();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
			SubmitButton.Disabled = true;
		else
			SubmitButton.Disabled = false;

		if (SubmitButton.ButtonPressed)
		{
			EntryPlayerName = Sanitize(UsernameEntry.Text);
			UsernameEntry.Text = "";

			ShowLoadingScreen();
			AlreadyAsked = true;
			AddRunEntry(EntryPlayerName, RunTime);
		}
		else if (ShowScoreboardButton.ButtonPressed)
		{
			ShowLoadingScreen();
			AlreadyAsked = false;
			GetScoreboard();
		}
	}

	private void AddRunEntry(string playerName, double time)
	{
		string sanitizedPlayerName = Sanitize(playerName);
		string urlEncodedPlayerName = System.Uri.EscapeDataString(sanitizedPlayerName);
		string url = $"{DatabaseLink}/{urlEncodedPlayerName}.json";
		var scoreData = new Dictionary<string, double> { { "time", Math.Round(time, 2) } };
		string Json = JsonSerializer.Serialize(scoreData);
		string[] headers = { "Content-Type: application/json" };
		HttpRequest.Request(url, headers, HttpClient.Method.Put, Json);
	}


	private void GetScoreboard()
	{
		HttpRequest.Request($"{DatabaseLink}.json");
	}

	private void ShowLoadingScreen()
	{
		EndResultsContainer.Visible = false;
		ScoreboardContainer.Visible = false;
		WaitingScreenContainer.Visible = true;
	}

	private void _on_http_request_request_completed(long result, long responseCode, string[] headers, byte[] body)
	{
		if (AlreadyAsked)
		{
			AlreadyAsked = false;
			if (result == (long)HttpRequest.Result.Success && responseCode >= 200 && responseCode < 300)
				GetScoreboard();
			else
			{
				GD.PrintErr($"Failed to add score. Result: {result}, Response Code: {responseCode}");
				WaitingScreenContainer.Visible = false;
				EndResultsContainer.Visible = true;
				AlreadyAsked = false;
			}
			return;
		}

		string Result = "";
		if (result == (long)HttpRequest.Result.Success && responseCode >= 200 && responseCode < 300)
		{
			string responseBodyString = Encoding.UTF8.GetString(body);
			var scoresData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, double>>>(responseBodyString);

			if (scoresData == null)
				Result = "Error: Could not load scores.";
			else
			{
				List<KeyValuePair<string, Dictionary<string, double>>> sortedScoresList = scoresData.ToList();
				sortedScoresList.Sort((pair1, pair2) => pair1.Value["time"].CompareTo(pair2.Value["time"]));
				for (int rank = 1; rank <= sortedScoresList.Count; rank++)
				{
					var scoreEntry = sortedScoresList[rank - 1];
					string playerName = scoreEntry.Key;
					double time = scoreEntry.Value["time"];
					string formattedTime = FormatTime(time);

					string rankDisplay = (rank.ToString() + ".").PadRight(3, ' ');
					string paddedPlayerName = FormatPlayerNameForDisplay(playerName, 20);
					string line = $"{rankDisplay} {paddedPlayerName}  {formattedTime}";

					if (!string.IsNullOrEmpty(EntryPlayerName) && playerName == EntryPlayerName)
						Result += $"[wave amp=15 freq=5][b]{line}[/b][/wave]\n";
					else if (rank == 1)
						Result += $"[shake rate=20.0 level=3][color=RED][b]{line}[/b][/color][/shake]\n";
					else if (rank == 2)
						Result += $"[shake rate=15.0 level=2][color=GREEN][b]{line}[/b][/color][/shake]\n";
					else if (rank == 3)
						Result += $"[shake rate=10.0 level=1][color=BLUE][b]{line}[/b][/color][/shake]\n";
					else
						Result += $"{line}\n";
				}
			}
		}
		else
		{
			Result = "Error: Could not retrieve scores.";
		}
		FormattedScoreboard = Result;
		WaitingScreenContainer.Visible = false;
		ScoreboardContainer.Visible = true;
		ScoreboardText.Text = FormattedScoreboard;
	}

	// Returns the estimated number of monospaced cells a character occupies.
	// Assumes CJK and similar full-width characters occupy 2 cells for alignment, others 1.
	private int GetCharacterCellWidth(char c)
	{
		if (c <= 127) return 1; // Standard ASCII characters

		// Heuristic for CJK and other potentially wider characters
		// Noto Sans Mono CJK should render these as double-width.
		if ((c >= 0x4E00 && c <= 0x9FFF) || // CJK Unified Ideographs
			(c >= 0x3000 && c <= 0x303F) || // CJK Symbols and Punctuation
			(c >= 0xAC00 && c <= 0xD7AF) || // Hangul Syllables
			(c >= 0xFF00 && c <= 0xFFEF))   // Halfwidth and Fullwidth Forms
		{
			return 2; // Assume these take up two cells for alignment
		}
		// Other Unicode characters (e.g., Latin extended with accents) are often 1 cell in monospaced fonts.
		return 1;
	}

	private string FormatPlayerNameForDisplay(string playerName, int targetCellCount)
	{
		StringBuilder sb = new StringBuilder();
		int currentCellCount = 0;

		foreach (char c in playerName)
		{
			int charCellWidth = GetCharacterCellWidth(c);
			if (currentCellCount + charCellWidth <= targetCellCount)
			{
				sb.Append(c);
				currentCellCount += charCellWidth;
			}
			else
			{
				// Character does not fit, truncate here
				break;
			}
		}

		// Pad with spaces to fill the remaining cells
		if (currentCellCount < targetCellCount)
		{
			sb.Append(' ', targetCellCount - currentCellCount);
		}

		return sb.ToString();
	}

	private string FormatTime(double totalSeconds)
	{
		TimeSpan RunTime = TimeSpan.FromSeconds(totalSeconds);
		string FormattedTime = "";
		if (RunTime.TotalHours >= 1)
			FormattedTime += $"{(int)RunTime.TotalHours}h";
		if (RunTime.Minutes > 0 || RunTime.TotalHours >= 1)
			FormattedTime += $"{RunTime.Minutes:D2}m";
		FormattedTime += (RunTime.Seconds + RunTime.Milliseconds / 1000.0).ToString("00.00", CultureInfo.InvariantCulture) + "s";
		return FormattedTime;
	}

	private string Sanitize(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
			return "NullProPlayer";
		return Regex.Replace(input, @"[\.\$#\[\]\/ ]", "_");
	}
}
