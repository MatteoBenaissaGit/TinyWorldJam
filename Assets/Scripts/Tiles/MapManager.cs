// ReSharper disable All

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    [Space(10)] public Vector2 GridSize;

    public void GenerateTiles()
    {
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                GameObject tile = Instantiate(_tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                tile.transform.SetParent(transform);
            }
        }
    }

    public void DeleteAllTiles()
    {
        Tile[] array = GameObject.FindObjectsOfType<Tile>();
        foreach (Tile tile in array)
        {
            DestroyImmediate(tile.gameObject.transform.parent.gameObject);
        }
    }
}
