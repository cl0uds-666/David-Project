#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshTools
{
    [MenuItem("Tools/Clear All NavMeshes")]
    public static void ClearAllNavMeshes()
    {
        foreach (var surface in GameObject.FindObjectsOfType<NavMeshSurface>())
        {
            surface.RemoveData();
        }

        Debug.Log("All NavMesh surfaces cleared.");
    }
}
#endif
