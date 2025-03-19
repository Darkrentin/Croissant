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
    // Called when the node enters the scene tree for the first time.

    [Export] public RichTextLabel label;
    [Export] public NinePatchRect background;
    [Export] public Timer timer;

    [Export] public Window Parent;

    [Export] public Button SkipButton;

    public Dictionary Dialogue;

    public override void _Ready()
    {
        base._Ready();
        if (Parent == null)
        {
            Parent = GetParent() as FloatWindow;
        }

        label.Theme = new Theme();
        label.Theme.DefaultFontSize = Lib.GetScreenSize(0.01f, 0).X;

        const string dialoguePath = "res://assets/Dialogue/Dialogue.json";
        LoadJson(dialoguePath);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("debug"))
        {
            ShowDialogueBox("Virus", 1);
        }

    }

    public void LoadJson(string path)
    {
        Json json = new Json();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var json_string = Json.ParseString(file.GetAsText());
        file.Close();

        Dialogue = (Dictionary)json_string;
    }

    public void InitDialogeBox()
    {
        Vector2I DialogueBoxSize = Lib.GetScreenSize(0.2f, 0.1f);
        Size = Lib.GetScreenSize(0.2f, 0.12f);

        background.Size = DialogueBoxSize;

        label.Size = new Vector2I((int)(DialogueBoxSize.X * 0.96f), (int)(DialogueBoxSize.Y * 0.7f));
        label.Position = ((Godot.Vector2)DialogueBoxSize) * 0.02f;

        SkipButton.Size = Size * new Godot.Vector2(0.3f, 0.2f);
        SkipButton.Position = Size * new Godot.Vector2(0.6f, 0.7f);

        int x, y;

        x = (int)(Parent.Position.X - (Size.X / 4));
        y = (int)(Parent.Position.Y + Parent.Size.Y - (Size.Y / 2));

        x = Mathf.Clamp(x, 0, GameManager.ScreenSize.X - Size.X);
        y = Mathf.Clamp(y, 0, GameManager.ScreenSize.Y - Size.Y);

        Position = new Vector2I(x, y);
    }

    public void ShowDialogueBox(string character, int id)
    {
        InitDialogeBox();
        Visible = true;
        GrabFocus();
        label.VisibleCharacters = 0;
        label.Text = (string)((Dictionary)Dialogue[character])[$"{id}"];
        timer.Start();
    }

    public void _on_timer_timeout()
    {
        if (label.VisibleCharacters < label.GetTotalCharacterCount())
        {
            label.VisibleCharacters += 1;
            timer.Start();
        }
        else
        {
            timer.Stop();
            LineFinished();
        }
    }

    private void LineFinished()
    {
    }
}
