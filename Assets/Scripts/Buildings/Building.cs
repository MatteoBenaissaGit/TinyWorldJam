using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private bool _hideGizmos;

    [ReadOnly] public Tile TileOccupied;

    #region Gizmos

    #if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_hideGizmos)
        {
            return;
        }
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        Handles.Label(transform.position, gameObject.name, style);
    }

#endif
    
    #endregion
}
