
// C# example.
using UnityEditor;
using System.Diagnostics;

public class ScriptBatch
{
    [MenuItem("MyTools/Windows Build With Postprocess")]
    public static void BuildGame()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

        // Build player.
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path + "/Overrider.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/LevelData", path + "/Overrider_Data/LevelData");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "/Overrider.exe";
        proc.Start();
    }
}
