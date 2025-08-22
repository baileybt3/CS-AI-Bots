using UnityEngine;
using UnityEditor;
using System.IO;

public static class AssetMover
{
    private static void MoveSelected(string targetFolder)
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) continue;

            string fileName = Path.GetFileName(path);
            string destFolder = Path.Combine("Assets", targetFolder);

            // Create target folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(destFolder))
            {
                string[] parts = targetFolder.Split('/');
                string cur = "Assets";
                foreach (string part in parts)
                {
                    string next = Path.Combine(cur, part);
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(cur, part);
                    cur = next;
                }
            }

            string destPath = Path.Combine(destFolder, fileName);
            string error = AssetDatabase.MoveAsset(path, destPath);
            if (!string.IsNullOrEmpty(error))
                Debug.LogError("Failed to move " + path + " -> " + destPath + " : " + error);
            else
                Debug.Log("Moved " + path + " -> " + destPath);
        }
    }

    [MenuItem("Assets/Move To/_Unused")]
    private static void MoveToUnused() => MoveSelected("_Unused");

    [MenuItem("Assets/Move To/_Archive")]
    private static void MoveToArchive() => MoveSelected("_Archive");

    [MenuItem("Assets/Move To/_ThirdParty")]
    private static void MoveToThirdParty() => MoveSelected("_ThirdParty");
}
