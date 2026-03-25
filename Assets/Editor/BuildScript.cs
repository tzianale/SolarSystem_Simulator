using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript
{
    private static readonly string[] Scenes =
    {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/ExplorerMode.unity",
        "Assets/Scenes/SandboxMode.unity"
    };

    [MenuItem("Build/Build Linux")]
    public static void BuildLinux()
    {
        Build(BuildTarget.StandaloneLinux64, "Build/Linux/SolarSystemSimulator");
    }

    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows64, "Build/Windows/SolarSystemSimulator.exe");
    }

    [MenuItem("Build/Build macOS")]
    public static void BuildMacOS()
    {
        Build(BuildTarget.StandaloneOSX, "Build/macOS/SolarSystemSimulator.app");
    }

    private static void Build(BuildTarget target, string path)
    {
        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = path,
            target = target,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {report.summary.totalSize} bytes at {path}");
        }
        else
        {
            Debug.LogError($"Build failed: {report.summary.result}");
            EditorApplication.Exit(1);
        }
    }
}
