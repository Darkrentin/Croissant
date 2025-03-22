using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using FileAccess = Godot.FileAccess;

public partial class DialogueWindow : FloatWindow
{
	[Export] public FloatWindow ParentWindow;
	[Export] public RichTextLabel label;
	[Export] public Timer timer;
	[Export] public Timer cursorTimer;

	private Vector2I _margin = new Vector2I(0, 0);
	[Export]
	public Vector2I Margin
	{
		get => _margin;
		set
		{
			_margin = (Vector2I)Lib.GetScreenRatio() * value;
		}
	}
	public bool cursorVisible = false;
	public bool isTyping = false;

	public Dictionary DialogueData;
	public Dictionary ActualDialogue;
	public int index = 0;
	public bool isDialogue = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		const string dialoguePath = "res://assets/dialogues/Dialogue.json";
		LoadJson(dialoguePath);

		timer.Timeout += ShowNextCharacter;
		ShowNextCharacter();
		label.Text = "";
		cursorTimer.Timeout += ProcessCursor;
		cursorTimer.Start();

		StartDialogue("Virus", "1");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug"))
		{
			if (isDialogue)
			{
				NextLine();
			}
		}
	}

	public void ShowNextCharacter()
	{
		if (label.GetTotalCharacterCount() - 1 > label.GetVisibleCharacters())
		{
			isTyping = true;
			label.VisibleCharacters++;
			timer.WaitTime = Lib.GetRandomNormal(0.02f, 0.05f);
			timer.Start();
		}
		else
		{
			isTyping = false;
		}

	}
	public void ProcessCursor()
	{
		if (isTyping)
		{
			cursorTimer.Start();
			return;
		}

		if (cursorVisible)
		{
			label.VisibleCharacters--;
			cursorVisible = false;
		}
		else
		{
			label.VisibleCharacters++;
			cursorVisible = true;
		}
		cursorTimer.Start();
	}

	public void NextLine()
	{
		PlaceDialogueWindow();
		string dialogue = (string)ActualDialogue[$"{index}"];
		label.Text += "\n> ";
		label.Text += dialogue;
		label.Text += "|";
		if (dialogue == "")
		{
			isDialogue = false;
			label.Text = "";
			DialogueFinished();
			return;
		}
		index++;
		ShowNextCharacter();

	}

	public void LoadJson(string path)
	{
		Json json = new Json();
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var json_string = Json.ParseString(file.GetAsText());
		file.Close();

		DialogueData = (Dictionary)json_string;
	}

	public Dictionary GetDialogue(string character, string id)
	{
		return (Dictionary)((Dictionary)DialogueData[character])[id];
	}

	public void StartDialogue(string character, string id)
	{
		ActualDialogue = GetDialogue(character, id);
		index = 0;
		PlaceDialogueWindow();
		isDialogue = true;
		NextLine();
	}

	public static void DialogueFinished()
	{
		Lib.Print("Dialogue Finished");
	}

	public void PlaceDialogueWindow()
	{
		Size = (Vector2I)Lib.GetScreenRatio() * Size;
		Position = new Vector2I(ParentWindow.Position.X + ParentWindow.Size.X / 2 - Size.X / 2, ParentWindow.Position.Y - Size.Y / 2) + Margin;
		Lib.Print(Margin.ToString());
	}
}
