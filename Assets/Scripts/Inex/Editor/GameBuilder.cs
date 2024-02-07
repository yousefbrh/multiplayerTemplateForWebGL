#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;

public class GameBuilder : MonoBehaviour
{
    [MenuItem("Build/Build WebGL")]
    public static void PerformWebGLBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnabledEditorScenes();
        buildPlayerOptions.locationPathName = "build/WebGL";
        buildPlayerOptions.target = BuildTarget.WebGL;


        AssetDatabase.Refresh();
        // webgl settings
        PlayerSettings.WebGL.template = "APPLICATION:INEX";
        PlayerSettings.WebGL.wasmArithmeticExceptions = WebGLWasmArithmeticExceptions.Ignore;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.dataCaching = true;
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new GraphicsDeviceType[] {GraphicsDeviceType.OpenGLES2});
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, ScriptingImplementation.IL2CPP);
        // PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WebGL,ApiCompatibilityLevel.NET_4_6);


        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }

    [MenuItem("Build/Build Android")]
    public static void PerformAndroidBuild()
    {
        // BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        // buildPlayerOptions.scenes = FindEnabledEditorScenes();
        // buildPlayerOptions.target = BuildTarget.Android;
        // buildPlayerOptions.locationPathName = $"build/Android/{PlayerSettings.productName}.apk";
        // buildPlayerOptions.options = BuildOptions.Development;


        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnabledEditorScenes();
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.locationPathName = $"build/Android/{PlayerSettings.productName}.apk";
        // buildPlayerOptions.options = BuildOptions.Development;


        // AssetDatabase.Refresh();
        // // android settings
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
#if UNITY_PRO_LICENSE
        PlayerSettings.SplashScreen.show = false;
#endif
        PlayerSettings.companyName = "Inex";
        PlayerSettings.applicationIdentifier = $"com.{PlayerSettings.companyName}.{PlayerSettings.productName}";
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        // // PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,ApiCompatibilityLevel.NET_4_6);
        // // PlayerSettings.Android
        //
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }


    [MenuItem("Build/Build Windows")]
    public static void PerformWindowsBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnabledEditorScenes();
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.locationPathName = $"build/Windows/{PlayerSettings.productName}.exe";


        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            if (scene.enabled)
                editorScenes.Add(scene.path);

        return editorScenes.ToArray();
    }
}
#endif