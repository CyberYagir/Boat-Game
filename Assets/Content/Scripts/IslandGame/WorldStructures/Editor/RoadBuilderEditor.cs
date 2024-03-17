using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.IslandGame.WorldStructures.Editor
{
    [CustomEditor(typeof(RoadsGenerator))]
    public class RoadBuilderEditor : UnityEditor.Editor
    {
        // private void OnSceneGUI()
        // {
        //     var n = target as RoadsGenerator;
        //     
        //     foreach (var vector3Int in n.Points)
        //     {
        //         Handles.Label(vector3Int, vector3Int.ToString());
        //     }
        //     
        // }
    }
}
