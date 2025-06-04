using Godot;
using System.Globalization;

public partial class SaveData : Resource
{
    [Export] public bool FakeDesktop = false;
    [Export] public bool DebugMode = false;
    [Export] public bool HaveFinishTheGameAtLeastOneTime = false;
    [Export] public bool HaveLaunchedTheGameAtLeastOneTime = false;
    [Export] public double PersonalBestTime = 21387599f;
    [Export] public double MainVolume = 100;
    [Export] public double MusicVolume = 100;
    [Export] public double SfxVolume = 100;
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
        HaveLaunchedTheGameAtLeastOneTime = GameManager.HaveLaunchedTheGameAtLeastOneTime;
        PersonalBestTime = GameManager.PersonalBestTime;
        MainVolume = GameManager.MenuWindow.MasterVolumeSlider.Value;
        MusicVolume = GameManager.MenuWindow.MusicVolumeSlider.Value;
        SfxVolume = GameManager.MenuWindow.SFXVolumeSlider.Value;
        Lib.Print("Successfully retrieved general game settings for SaveData instance");
    }

    public Error Save(string fileName = "Save.tres")
    {
        GetData();
        Lib.Print("Data to save:");
        Lib.Print("FakeDesktop: " + FakeDesktop);
        Lib.Print("DebugMode: " + DebugMode);
        Lib.Print("HaveFinishTheGameAtLeastOneTime: " + HaveFinishTheGameAtLeastOneTime);
        Lib.Print("HaveLaunchedTheGameAtLeastOneTime: " + HaveLaunchedTheGameAtLeastOneTime);
        Lib.Print("PersonalBestTime: " + (PersonalBestTime == double.MaxValue ? "N/A" : PersonalBestTime.ToString(CultureInfo.InvariantCulture)));
        Lib.Print("MainVolume: " + MainVolume);
        Lib.Print("MusicVolume: " + MusicVolume);
        Lib.Print("SfxVolume: " + SfxVolume);

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
                Lib.Print("HaveLaunchedTheGameAtLeastOneTime: " + loadedResource.HaveLaunchedTheGameAtLeastOneTime);
                Lib.Print("PersonalBestTime: " + (loadedResource.PersonalBestTime == double.MaxValue ? "N/A" : loadedResource.PersonalBestTime.ToString(CultureInfo.InvariantCulture)));
                Lib.Print("MainVolume: " + loadedResource.MainVolume);
                Lib.Print("MusicVolume: " + loadedResource.MusicVolume);
                Lib.Print("SfxVolume: " + loadedResource.SfxVolume);

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
