using UnityEngine;
using UnityEditor;

public class AddMeshColliders : MonoBehaviour
{
    [MenuItem("Tools/Colliders/Add MeshColliders To Selection")]
    static void AddToSelection()
    {
        foreach (var t in Selection.transforms)
        {
            foreach (var mf in t.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.sharedMesh == null) continue;
                var go = mf.gameObject;
                if (!go.GetComponent<MeshCollider>())
                {
                    var mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = false; // keep world geometry non-convex
                }
            }
        }
        Debug.Log("MeshColliders added to selection.");
    }
}
