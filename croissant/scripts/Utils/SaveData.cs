// SaveData.cs
using Godot;
using System.Collections.Generic;
using System.ComponentModel.Design;

public partial class SaveData : Resource
{
    //Variable to Save

    [Export]
    public float Difficulty = 1f;
    [Export]
    public bool FakeDesktop = false;
    [Export]
    public bool DebugMode = false;
    [Export]
    public bool HaveFinishTheGameAtLeastOneTime = false;


    private const string SaveFolderName = "ShapeGlitch";

    private string GetSaveFilePath(string fileName = "Save.tres")
    {
        return $"user://{SaveFolderName}/{fileName}";
    }

    public void GetData()
    {
        Difficulty = GameManager.Difficulty;
        FakeDesktop = GameManager.MenuWindow.FakeDesktop;
        DebugMode = GameManager.MenuWindow.DebugMode;
        HaveFinishTheGameAtLeastOneTime = GameManager.HaveFinishTheGameAtLeastOneTime;
        Lib.Print("Successfully retrieved data");

    }
    public Error Save(string fileName = "Save.tres")
    {
        GetData();
        Lib.Print("Data to save:");
        Lib.Print("Difficulty: " + Difficulty);
        Lib.Print("FakeDesktop: " + FakeDesktop);
        Lib.Print("DebugMode: " + DebugMode);
        Lib.Print("HaveFinishTheGameAtLeastOneTime: " + HaveFinishTheGameAtLeastOneTime);

        string path = GetSaveFilePath(fileName);
        string dirPath = $"user://{SaveFolderName}/";

        Error makeDirError = DirAccess.MakeDirRecursiveAbsolute(dirPath);
        if (makeDirError != Error.Ok)
        {
            GD.PrintErr($"Error creating save directory {dirPath}: {makeDirError}");
            return makeDirError;
        }

        var error = ResourceSaver.Save(this, path);
        if (error != Error.Ok)
        {
            GD.PrintErr($"Error saving resource to {path}: {error}");
        }
        else
        {
            Lib.Print($"Resource saved successfully to {path}");
        }
        return error;
    }

    public static SaveData LoadData(string fileName = "Save.tres")
    {
        string path = $"user://{SaveFolderName}/{fileName}";

        if (ResourceLoader.Exists(path))
        {
            var loadedResource = ResourceLoader.Load<SaveData>(path);
            if (loadedResource != null)
            {
                Lib.Print($"Resource loaded successfully from {path}");
                Lib.Print("Data loaded:");
                Lib.Print("Difficulty: " + loadedResource.Difficulty);
                Lib.Print("FakeDesktop: " + loadedResource.FakeDesktop);
                Lib.Print("DebugMode: " + loadedResource.DebugMode);
                Lib.Print("HaveFinishTheGameAtLeastOneTime: " + loadedResource.HaveFinishTheGameAtLeastOneTime);

                return loadedResource;
            }
            else
            {
                GD.PrintErr($"Loaded resource at {path} is not of type SaveData or failed to load.");
                return null;
            }
        }
        else
        {
            Lib.Print($"Save file not found at {path}");
            return null;
        }
    }
}