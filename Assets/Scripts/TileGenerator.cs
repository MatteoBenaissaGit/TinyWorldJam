// ReSharper disable All

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [Space(10)] public Vector2 GridSize;

    private List<Tile> _tileList;
    
    public void GenerateTiles()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Tile tile = Instantiate(_tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tile.transform.SetParent(transform);
                _tileList.Add(tile);
            }
        }
    }

    public void DeleteTiles()
    {
        foreach (Tile tile in _tileList)
        {
            DestroyImmediate(tile.gameObject);
        }
        _tileList.Clear();
    }
}
