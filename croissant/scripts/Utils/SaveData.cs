using Godot;
using System.Globalization;

public partial class SaveData : Resource
{
    [Export] public bool FakeDesktop = false;
    [Export] public bool DebugMode = false;
    [Export] public bool HaveFinishTheGameAtLeastOneTime = false;
    [Export] public double PersonalBestTime = 21387599f;
    private const string SaveFolderName = "ShapeGlitch";
    private string GetSaveFilePath(string fileName = "Save.tres")
    {
        return $"user://{SaveFolderName}/{fileName}";
    }

    public void GetData()
    {
        FakeDesktop = GameManager.MenuWindow.FakeDesktop;
        DebugMode = GameManager.MenuWindow.DebugMode;
        HaveFinishTheGameAtLeastOneTime = GameManager.HaveFinishTheGameAtLeastOneTime;
        PersonalBestTime = GameManager.PersonalBestTime;
        Lib.Print("Successfully retrieved general game settings for SaveData instance");
    }

    public Error Save(string fileName = "Save.tres")
    {
        GetData();
        Lib.Print("Data to save:");
        Lib.Print("FakeDesktop: " + FakeDesktop);
        Lib.Print("DebugMode: " + DebugMode);
        Lib.Print("HaveFinishTheGameAtLeastOneTime: " + HaveFinishTheGameAtLeastOneTime);
        Lib.Print("PersonalBestTime: " + (PersonalBestTime == double.MaxValue ? "N/A" : PersonalBestTime.ToString(CultureInfo.InvariantCulture)));

        string path = GetSaveFilePath(fileName);
        string dirPath = $"user://{SaveFolderName}/";

        Error makeDirError = DirAccess.MakeDirRecursiveAbsolute(dirPath);
        if (makeDirError != Error.Ok)
        {
            return makeDirError;
        }

        var error = ResourceSaver.Save(this, path);
        if (error != Error.Ok)
        {
            //GD.PrintErr($"Error saving resource to {path}: {error}");
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
                Lib.Print("FakeDesktop: " + loadedResource.FakeDesktop);
                Lib.Print("DebugMode: " + loadedResource.DebugMode);
                Lib.Print("HaveFinishTheGameAtLeastOneTime: " + loadedResource.HaveFinishTheGameAtLeastOneTime);
                Lib.Print("PersonalBestTime: " + (loadedResource.PersonalBestTime == double.MaxValue ? "N/A" : loadedResource.PersonalBestTime.ToString(CultureInfo.InvariantCulture)));

                return loadedResource;
            }
            else
            {
                //GD.PrintErr($"Loaded resource at {path} is not of type SaveData or failed to load.");
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
