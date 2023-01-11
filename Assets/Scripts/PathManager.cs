using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    #region Variables

    [Header("Referencing"), SerializeField] private Building _pathPrefab;

    [ReadOnly] public List<Tile> TilePath = new List<Tile>();
    
    private Arrival _arrival;
    private Departure _departure;

    #endregion

    #region Methods

    private void Start()
    {
        _arrival = FindObjectOfType<Arrival>();
        _departure = FindObjectOfType<Departure>();

        TilePath.Add(_departure.TileOccupied); //departure = first tile you have to place on
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.PlacingPath)
        {
            return;
        }
        
        PlacePath();
    }

    private void PlacePath()
    {
        if (GameManager.Instance.SelectedTile == null || Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        
        Tile tile = GameManager.Instance.SelectedTile;
        if (tile.IsOccupied == false && 
            GameManager.Instance.Neighbours(TilePath.Last()).Contains(tile))
        {
            tile.SetBuilding(_pathPrefab, -0.4f);
            TilePath.Add(tile);
            print("path placed");
        }
    }

    public void ResetPath()
    {
        
    }

    #endregion
}
