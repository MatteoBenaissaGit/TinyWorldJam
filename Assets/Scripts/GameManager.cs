using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
            Instance = this;
        }
    }

    #endregion

    #region Variables

    [HideInInspector] public List<Tile> TileList = new List<Tile>();

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        //tile list creation
        Tile[] array = FindObjectsOfType<Tile>();
        TileList.Clear();
        TileList = array.ToList();
    }

    private void Update()
    {
        SelectTiles();
    }

    #endregion

    #region Methods

    private void SelectTiles()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            print(hit.transform.gameObject.name);
            Tile rayTile = hit.transform.gameObject.GetComponent<Tile>();
            if (rayTile != null)
            {
                rayTile.Select();
                foreach (Tile tile in TileList)
                {
                    if (tile != rayTile)
                    {
                        tile.Unselect();
                    }
                }
            }
            else
            {
                foreach (Tile tile in TileList)
                {
                    tile.Unselect();
                }
            }
        }
        else
        {
            foreach (Tile tile in TileList)
            {
                tile.Unselect();
            }
        }
    }

    #endregion
}