using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
#if UNITY_EDITOR

        DrawDefaultInspector();
        var script = (Tile)target;
        
        if(GUILayout.Button($"Remove Building"))
        {
            script.RemoveAnyBuilding();
        }
        
        if(GUILayout.Button($"Set Arrival"))
        {
            script.SetArrival();
        }
        
        if(GUILayout.Button($"Set Departure"))
        {
            script.SetDeparture();
        }
        
        if(GUILayout.Button($"Set Water"))
        {
            script.SetWater();
        }
    }
#endif    
}