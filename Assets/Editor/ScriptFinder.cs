using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ScriptFinder : EditorWindow
{
    private Vector2 scrollPos;
    private string keyword = "Input.";
    private Dictionary<string, List<int>> results = new Dictionary<string, List<int>>();

    [MenuItem("Tools/Keyword Scanner")]
    private static void ShowWindow()
    {
        GetWindow<ScriptFinder>("Keyword Scanner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find Keyword in Scripts", EditorStyles.boldLabel);
        keyword = EditorGUILayout.TextField("Keyword", keyword);

        if (GUILayout.Button("Find"))
        {
            FindKeywordInScripts(keyword);
        }

        if (results.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("Results", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (var entry in results)
            {
                EditorGUILayout.BeginVertical("box");

                if (GUILayout.Button($"{Path.GetFileName(entry.Key)} — Lines: {string.Join(", ", entry.Value)}"))
                {
                    var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(entry.Key);
                    if (scriptAsset != null)
                        EditorGUIUtility.PingObject(scriptAsset);
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void FindKeywordInScripts(string word)
    {
        results.Clear();

        if (string.IsNullOrWhiteSpace(word))
        {
            Debug.LogWarning("Keyword is empty. Please enter a valid keyword.");
            return;
        }

        var files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (var filePath in files)
        {
            List<int> hitLines = new();
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(word))
                {
                    hitLines.Add(i + 1); // +1 to match actual line numbers
                }
            }

            if (hitLines.Count > 0)
            {
                string relativePath = "Assets" + filePath.Replace(Application.dataPath, "").Replace('\\', '/');
                results[relativePath] = hitLines;
            }
        }
        Debug.Log($"Scan complete. Found '{word}' in {results.Count} file(s).");
    }
}
