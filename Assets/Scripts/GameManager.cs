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

    [Header("Referencing"), SerializeField] private MapManager _mapManager;
    
    [Header("Debug")]
    [ReadOnly] public GameState CurrentGameState = GameState.Start;
    [ReadOnly] public Tile SelectedTile;
    
    [HideInInspector] public Tile[,] TileArray = new Tile[,]{};

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        //tile list creation
        Tile[] array = FindObjectsOfType<Tile>();
        TileArray = new Tile[(int)_mapManager.GridSize.x, (int)_mapManager.GridSize.y];
        foreach (Tile tile in array)
        {
            Vector3 position = tile.transform.position;
            //TODO look for the real position and the position in the array it get placed to
            TileArray[(int)position.x, (int)position.z] = tile;
        }
        
        //state
        ChangeState(GameState.PlacingPath);
    }

    private void Update()
    {
        SelectTiles();
    }

    #endregion

    #region StateMachine

    public void ChangeState(GameState gameState)
    {
        if (CurrentGameState == gameState)
        {
            return;
        }

        switch (gameState)
        {
            case GameState.Start:
                break;
            case GameState.PlacingPath:
                break;
            case GameState.ManagingDefense:
                break;
            case GameState.Attack:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }

        CurrentGameState = gameState;
    }

    #endregion
    
    #region Methods

    private void SelectTiles()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Tile rayTile = hit.transform.gameObject.GetComponent<Tile>();
            if (rayTile != null)
            {
                rayTile.Select();
                SelectedTile = rayTile;
                foreach (Tile tile in TileArray)
                {
                    if (tile != rayTile)
                    {
                        tile.Unselect();
                    }
                }
            }
            else
            {
                foreach (Tile tile in TileArray)
                {
                    SelectedTile = null;
                    tile.Unselect();
                }
            }
        }
        else
        {
            foreach (Tile tile in TileArray)
            {
                SelectedTile = null;
                tile.Unselect();
            }
        }
    }
    
    public List<Tile> Neighbours(Tile tile)
    {
        print("getting neighbours");
        
        Vector2[] directions = new[]
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0)
        };
        List<Tile> neighbours = new List<Tile>();

        foreach (Vector2 direction in directions)
        {
            Vector3 tilePosition = tile.transform.position;
            Vector2 position = new Vector2(tilePosition.x + direction.x, tilePosition.y + direction.y);
            
            print($"looking for position : {position}");
            if (position.x >= 0 && position.x < (int)_mapManager.GridSize.x && position.y >= 0 &&
                position.y < (int)_mapManager.GridSize.y)
            {
                Tile neighbour = TileArray[(int)position.x,(int)position.y];
                if (neighbour != null)
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    #endregion
}

public enum GameState
{
    Start,
    PlacingPath,
    ManagingDefense,
    Attack,
    Win,
    Lose
}