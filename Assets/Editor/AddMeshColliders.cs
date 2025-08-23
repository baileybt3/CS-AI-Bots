using UnityEngine;
using UnityEditor;

public class AddMeshColliders : EditorWindow
{
    [MenuItem("Tools/Add Mesh Colliders to Children")]
    static void AddColliders()
    {
        // Loop through selected objects in the scene
        foreach (GameObject go in Selection.gameObjects)
        {
            MeshRenderer[] meshRenderers = go.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                GameObject child = mr.gameObject;

                //Add MeshCollider if not already there
                if (child.GetComponent<MeshCollider>() == null)
                {
                    child.AddComponent<MeshCollider>();
                }
            }
        }

        Debug.Log("Added MeshColliders to children of selected objects.");
    }
}