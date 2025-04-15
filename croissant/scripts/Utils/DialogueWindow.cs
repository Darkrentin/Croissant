using Godot;
using Godot.Collections;
using System;
using FileAccess = Godot.FileAccess;

public partial class DialogueWindow : FloatWindow
{
	[Export] public Virus ParentWindow;
	[Export] public RichTextLabel label;
	[Export] public Timer timer;
	[Export] public Timer cursorTimer;
	[Export] public bool CanSkip = false;

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

	public string ActualDialogueName;

	public Action<string> OnDialogueFinished;
	public int index = 0;
	public bool isDialogue = false;

	public override void _Ready()
	{
		const string dialoguePath = "res://assets/texts/dialogues/Dialogue.json";
		LoadJson(dialoguePath);

		timer.Timeout += ShowNextCharacter;
		ShowNextCharacter();
		label.Text = "";
		cursorTimer.Timeout += ProcessCursor;
		cursorTimer.Start();

		OnDialogueFinished += DialogueFinished;

		//StartDialogue("Virus", "sleep");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug"))
			ParentWindow._on_button_pressed();
	}

	public void ShowNextCharacter()
	{
		if (label.GetTotalCharacterCount() - 1 > label.GetVisibleCharacters())
		{
			isTyping = true;
			label.VisibleCharacters++;
			timer.WaitTime = Lib.rand.NextDouble() / 4f;
			timer.Start();
		}
		else
			isTyping = false;

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
			label.VisibleCharacters = label.GetTotalCharacterCount() - 1;
			cursorVisible = false;
		}
		else
		{
			label.VisibleCharacters = label.GetTotalCharacterCount();
			cursorVisible = true;
		}
		cursorTimer.Start();
	}

	public void NextLine()
	{
		if (isTyping && !CanSkip)
			return;
		PlaceDialogueWindow();
		Dictionary dialogue = ((Dictionary)ActualDialogue[$"{index}"]);
		string text = (String)dialogue["text"];
		string anim = (String)dialogue["anim"];
		label.Text.Replace("|", "");
		label.Text += "\n> ";
		label.Text += text;
		label.Text += "|";
		if (text == "")
		{
			isDialogue = false;
			label.Text = "";
			OnDialogueFinished(ActualDialogueName);
			return;
		}
		GameManager.virus.AnimationScreen.Travel(anim);
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
		ActualDialogueName = id;
		index = 0;
		label.Text = "";
		label.VisibleCharacters = 0;
		PlaceDialogueWindow();
		isDialogue = true;
		NextLine();
	}

	public static void DialogueFinished(string id)
	{
		Lib.Print("Dialogue Finished");
	}

	public void PlaceDialogueWindow()
	{
		Size = (Vector2I)Lib.GetScreenRatio() * Size;
		Position = new Vector2I(ParentWindow.Position.X + ParentWindow.Size.X / 2 - Size.X / 2, ParentWindow.Position.Y - Size.Y / 2) + Margin;
	}
}
