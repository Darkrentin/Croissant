using Godot;
using Godot.Collections;
using System;
using FileAccess = Godot.FileAccess;

public partial class DialogueWindow : FloatWindow
{
	[Export] public Npc ParentWindow;
	[Export] public RichTextLabel label;
	[Export] public Timer timer;
	[Export] public Timer cursorTimer;
	[Export] public bool CanSkip = false;
	[Export] public bool LockSkip = false;
	[Export] public Vector2I Margin { get => _margin; set { _margin = (Vector2I)Lib.GetScreenRatio() * value; } }
	private Vector2I _margin = new Vector2I(0, 0);
	public bool cursorVisible = false;
	public bool isTyping = false;
	public Dictionary DialogueData;
	public Dictionary ActualDialogue;
	public string ActualDialogueName;
	public Action<string> OnDialogueFinished;
	public int index = 0;
	public bool isDialogue = false;
	public int CharacterBetweenSounds = 1;

	public static int DialogueCount = 0;
	public static int Dialogueid = 0;

	[Export] public bool LeftSide = true;

	public Vector2I? FixedPosition = null;

	public override void _Ready()
	{
		base._Ready();
		const string dialoguePath = "res://assets/texts/dialogues/Dialogue.json";
		LoadJson(dialoguePath);

		timer.Timeout += ShowNextCharacter;
		ShowNextCharacter();
		label.Text = "";
		cursorTimer.Timeout += _ProcessCursor;
		cursorTimer.Start();

		OnDialogueFinished += ParentWindow.DialogueFinished;
		DialogueCount++;
		Dialogueid = DialogueCount;
		//StartDialogue("Virus", "sleep");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("debug"))
			ParentWindow.Skip();
	}

	public void ShowNextCharacter()
	{
		if (label.GetTotalCharacterCount() - 1 > label.GetVisibleCharacters())
		{
			if (CharacterBetweenSounds == 0)
			{
				ParentWindow.DialogueSound.PitchScale = Lib.GetRandomNormal(0.9f, 1.1f);
				ParentWindow.DialogueSound.Play();
				CharacterBetweenSounds = 1;
			}
			else
			{
				CharacterBetweenSounds--;
			}

			isTyping = true;
			label.VisibleCharacters++;
			timer.WaitTime = Lib.rand.NextDouble() / 11f;
			timer.Start();
		}
		else
			isTyping = false;
	}

	public void _ProcessCursor()
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
		{
			return;
		}
		if (isTyping)
		{
			label.VisibleCharacters = label.GetTotalCharacterCount() - 1;
			return;
		}
		Dictionary dialogue = ((Dictionary)ActualDialogue[$"{index}"]);
		string text = (String)dialogue["text"];
		string anim = (String)dialogue["anim"];
		if (text == "")
		{
			isDialogue = false;
			text = ((Dictionary)ActualDialogue[$"{index - 1}"])["text"].ToString();
			label.Text = "";
			if (anim != "")
				ParentWindow.PlayAnimation(anim);
			OnDialogueFinished(ActualDialogueName);
			FixedPosition = null;
			return;
		}
		label.Text = label.Text.Replace("|", "");
		label.Text += "\n> ";
		label.Text += text;
		label.Text += " |";
		ParentWindow.PlayAnimation(anim);
		index++;
		ShowNextCharacter();
		PlaceDialogueWindow();
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

	public void StartDialogue(string character, string id, Vector2I? fixedPosition = null)
	{
		FixedPosition = fixedPosition;
		ActualDialogue = GetDialogue(character, id);
		ActualDialogueName = id;
		index = 0;
		label.Text = "";
		label.VisibleCharacters = 0;
		isDialogue = true;
		isTyping = false;
		cursorVisible = false;
		NextLine();
	}


	public void PlaceDialogueWindow(bool force = false)
	{
		Size = (Vector2I)Lib.GetScreenRatio() * Size;

		if (FixedPosition != null)
		{
			SetWindowPosition(FixedPosition.Value, skipVerification: true);
		}
		else
		{
			int side = 3;
			if (LeftSide)
				side = 1;
			SetWindowPosition(new Vector2I(ParentWindow.Position.X + (ParentWindow.Size.X / 2) * side - (Size.X / 2),
										ParentWindow.Position.Y - Size.Y / 2) + Margin, skipVerification: force);
		}

		Visible = true;

	}
}
