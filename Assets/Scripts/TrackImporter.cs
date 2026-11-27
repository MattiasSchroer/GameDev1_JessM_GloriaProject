using UnityEngine;
using UnityEditor;
using System.IO;

public class TrackImporter
{
    private static string path => "./Tracks/";

    [MenuItem("Tracks/ExpotAllTracks")]
    static void ExportAllTracks()
    {
        if (!Directory.Exists(Path.GetFullPath(path)))
        {
            Directory.CreateDirectory(Path.GetFullPath(path));
        }

        Debug.Log("Exporting...");

        var tracks = Resources.LoadAll<Track>("Tracks/");

        foreach(var track in tracks)
        {
            var currentPath = path + "/" + track.name + ".json";

            Debug.Log("Exporting to: " + Path.GetFullPath(path) + currentPath);

            string json = JsonUtility.ToJson(track, true);
            File.WriteAllText(currentPath, json);
        }

        Debug.Log("Export Done");
    }

    [MenuItem("Tracks/ImportTrack")]
    static void ImportTrack()
    {
        var trackPath = EditorUtility.OpenFilePanel("Please select a track file", "Tracks", "json");
        if (string.IsNullOrEmpty(trackPath))
        {
            return;
        }

        Debug.Log("Importing: " + trackPath);

        var json = File.ReadAllText(trackPath);
        var newScriptableObject = ScriptableObject.CreateInstance<Track>();
        JsonUtility.FromJsonOverwrite(json, newScriptableObject);

        AssetDatabase.CreateAsset(newScriptableObject, $"Assets/Resources/Tracks/{newScriptableObject.Name}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Import Done");
    }
}
