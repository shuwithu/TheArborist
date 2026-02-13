using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;

public class BuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Absolute path to the JSON file that causes the build to fail
        string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UnityShimmerDataStreaming", "Build", "RuntimeActionBindings.json");

        if (File.Exists(jsonFilePath))
        {
            try
            {
                File.Delete(jsonFilePath);
                Debug.Log($"[BuildPreprocessor] Deleted existing file: {jsonFilePath}");
            }
            catch (IOException ex)
            {
                Debug.LogError($"[BuildPreprocessor] Failed to delete file: {jsonFilePath} - {ex.Message}");
            }
        }
        else
        {
            Debug.Log($"[BuildPreprocessor] No existing file to delete at: {jsonFilePath}");
        }
    }
}