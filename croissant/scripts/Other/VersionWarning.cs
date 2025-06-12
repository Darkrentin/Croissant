using Godot;
using System;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

public partial class VersionWarning : Window
{
    private string DatabaseLink = "https://shape-glitch-default-rtdb.europe-west1.firebasedatabase.app";
    [Export] private HttpRequest HttpRequest;
    [Export] private RichTextLabel InfoLabel;
    [Export] private RichTextLabel Link;

    [Signal]
    public delegate void VersionRetrievedEventHandler(string version);

    [Signal]
    public delegate void VersionRetrievalFailedEventHandler();

    public override void _Ready()
    {
        HttpRequest.RequestCompleted += OnVersionRequestCompleted;

        VersionRetrieved += HandleRetrievedVersion;
        VersionRetrievalFailed += HandleVersionRetrievalFailure;
        FetchVersion();
    }

    public void FetchVersion()
    {
        string versionUrl = $"{DatabaseLink}/version.json";
        Error error = HttpRequest.Request(versionUrl);

        if (error != Error.Ok)
        {
            GD.PrintErr($"Error attempting HTTP request for version: {error}");
            EmitSignal(SignalName.VersionRetrievalFailed);
        }
    }

    private void OnVersionRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result == (long)HttpRequest.Result.Timeout)
        {
            HandleRequestTimeout();
            return;
        }

        if (result != (long)HttpRequest.Result.Success)
        {
            GD.PrintErr($"Version request failed. Non-success result: {result}");
            EmitSignal(SignalName.VersionRetrievalFailed);
            return;
        }

        if (responseCode < 200 || responseCode >= 300)
        {
            GD.PrintErr($"Version request failed. Response code: {responseCode}");
            EmitSignal(SignalName.VersionRetrievalFailed);
            return;
        }

        try
        {
            string responseBodyString = Encoding.UTF8.GetString(body);
            string version = JsonSerializer.Deserialize<string>(responseBodyString);

            if (version != null)
            {
                EmitSignal(SignalName.VersionRetrieved, version);
            }
            else
            {
                GD.PrintErr("Deserialized version is null.");
                EmitSignal(SignalName.VersionRetrievalFailed);
            }
        }
        catch (JsonException e)
        {
            GD.PrintErr($"JSON deserialization error for version: {e.Message}");
            EmitSignal(SignalName.VersionRetrievalFailed);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Unexpected error while processing version response: {e.Message}");
            EmitSignal(SignalName.VersionRetrievalFailed);
        }
    }

    private void HandleRetrievedVersion(string version)
    {
        GD.Print($"Version retrieved from Firebase: {version}");

        if (version == GameManager.Instance.Version)
        {
            GD.Print("Version matches the current game version.");
            Quit();
        }
        else
        {
            Visible = true;
            InfoLabel.Text = $"[center]Your current game version [color=RED][b]({GameManager.Instance.Version})[/b][/color] is outdated and may contain bugs or missing features. Please update to the latest version [color=RED][b]({version})[/b][/color].[/center]";
        }
    }

    private void HandleVersionRetrievalFailure()
    {
        GD.PrintErr("Failed to retrieve version from Firebase.");
        Quit();
    }

    private void HandleRequestTimeout()
    {
        GD.PrintErr("Version request timed out.");
        EmitSignal(SignalName.VersionRetrievalFailed);
    }

    public void Quit()
    {
        QueueFree();
        GameManager.State = GameManager.GameState.ScreenScaleScreen;
    }

    public void _on_rich_text_label_meta_clicked(string meta)
    {
        OS.ShellOpen(meta);
        GetTree().Quit();
    }
}
