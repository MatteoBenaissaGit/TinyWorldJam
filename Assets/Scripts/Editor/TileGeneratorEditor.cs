using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class TileGeneratorEditor : Editor
{
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (MapManager)target;
        
        if(GUILayout.Button($"Generate {script.GridSize.x}:{script.GridSize.y} tiles"))
        {
            script.GenerateTiles();
        }
        
        if(GUILayout.Button("Delete tiles"))
        {
            script.DeleteAllTiles();
        }
    }
#endif    
}