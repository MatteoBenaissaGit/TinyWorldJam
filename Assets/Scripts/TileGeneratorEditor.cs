using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGenerator))]
public class TileGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
#if UNITY_EDITOR

        DrawDefaultInspector();
        var script = (TileGenerator)target;
        
        if(GUILayout.Button($"Generate {script.GridSize.x}:{script.GridSize.y} tiles"))
        {
            script.GenerateTiles();
        }
        
        if(GUILayout.Button("Delete tiles"))
        {
            script.DeleteTiles();
        }
    }
#endif    
}