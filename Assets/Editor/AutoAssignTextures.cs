using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoAssignTextures : EditorWindow
{
    [MenuItem("Tools/Auto Assign Textures")]
    static void AssignTextures()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null) continue;

            // Look for texture with the same name
            string matName = mat.name.ToLower();
            string[] texGUIDs = AssetDatabase.FindAssets(matName + " t:Texture");
            if (texGUIDs.Length > 0)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(texGUIDs[0]));
                mat.SetTexture("_BaseMap", tex); // HDRP/URP
                mat.SetTexture("_MainTex", tex); // Built-in
            }
        }

        Debug.Log("Auto-assign finished!");
    }
}
