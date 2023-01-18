using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathManager : MonoBehaviour
{
    #region Variables

    [Header("Referencing"), SerializeField] private Building _pathPrefab;
    [SerializeField] private Button _resetPathButton;
    [SerializeField] private Button _confirmPathButton;
    [SerializeField] private TextMeshProUGUI _numberOfPathUsableText;

    [Header("Path"), SerializeField, Range(0,50)] private int _numberOfPathUsable = 20;

    [ReadOnly] public List<Tile> TilePath = new List<Tile>();
    
    [HideInInspector] public Arrival Arrival;
    [HideInInspector] public Departure Departure;
    private int _currentNumberOfPath;

    #endregion

    #region Methods

    private void Start()
    {
        //reference
        Arrival = FindObjectOfType<Arrival>();
        Departure = FindObjectOfType<Departure>();

        //path
        TilePath.Add(Departure.TileOccupied); //departure = first tile you have to place on
        _currentNumberOfPath = _numberOfPathUsable;
        GameManager.Instance.Neighbours(TilePath[0]).ForEach(x => x.Suggest());

        //ui
        _numberOfPathUsableText.text = _currentNumberOfPath.ToString();
        _confirmPathButton.interactable = false;
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
        if (GameManager.Instance.SelectedTile == null || 
            Input.GetMouseButton(0) == false || 
            TilePath.Count == 0 ||
            TilePath.Last() == GameManager.Instance.SelectedTile)
        {
            return;
        }
        
        Tile tile = GameManager.Instance.SelectedTile;
        Path tilePath = tile.IsOccupied ? tile.OccupierBuilding.GetComponent<Path>() : null;
        
        if ( (tile.IsOccupied == false || tilePath != null) && //if not occupied or occupied by path
             GameManager.Instance.Neighbours(TilePath.Last()).Contains(tile) && //isn't the last where tile were placed
             _currentNumberOfPath > 0) //have tiles left to place
        {
            if (TilePath.Count >= 2 && tile == TilePath[TilePath.Count - 2]) //isn't the last 2 where tiles were placed
            {
                //anim if click
                if (Input.GetMouseButtonDown(0))
                {
                    GameManager.Instance.ShakeSelectionAnimation();
                }
                return;
            }
            
            if (tile.IsOccupied == false)
            {
                tile.SetBuilding(_pathPrefab, -0.4f);
                tile.OccupierBuilding.ScaleAnimation();
            }
            TilePath.Add(tile);
            _currentNumberOfPath--;
            
            //path direction setup
            foreach (Tile tileInTilePath in FindObjectsOfType<Tile>())
            {
                // values setup
                Path path = tileInTilePath.IsOccupied ? tileInTilePath.OccupierBuilding.GetComponent<Path>() : null;
                if (path == null)
                {
                    continue;
                }
                List<Direction> directionList = new List<Direction>();
                
                //neighbours check
                foreach (Tile t in GameManager.Instance.Neighbours(tileInTilePath))
                {
                    if (t.OccupierBuilding == null) continue;
                    
                    Path p = t.OccupierBuilding.GetComponent<Path>();
                    Arrival a = t.OccupierBuilding.GetComponent<Arrival>();
                    Departure d = t.OccupierBuilding.GetComponent<Departure>();
                    if (p == null && a == null && d == null)
                    {
                        continue;
                    }

                    Vector3 neighbourPos = t.transform.position;
                    Vector3 pos = tileInTilePath.transform.position;
                    if (neighbourPos.x < pos.x) directionList.Add(Direction.Left);
                    else if (neighbourPos.x > pos.x) directionList.Add(Direction.Right);
                    if (neighbourPos.z < pos.z) directionList.Add(Direction.Down);
                    else if (neighbourPos.z < pos.z) directionList.Add(Direction.Up);
                }
                
                //set tile
                if (path != null)
                {
                    path.SetTile(directionList);
                }
            }
            
            //ui
            _numberOfPathUsableText.text = _currentNumberOfPath.ToString();
            _confirmPathButton.interactable = CanPathBeConfirmed();
            
            //suggestion
            foreach (Tile tileInArray in GameManager.Instance.TileArray)
            {
                if (GameManager.Instance.Neighbours(TilePath.Last()).Contains(tileInArray) == false)
                {
                    tileInArray.Unsuggest();
                }
            }
            foreach (Tile neighbourTile in GameManager.Instance.Neighbours(TilePath.Last()))
            {
                if (CanPathBePlacedOnTile(neighbourTile)) //have tiles left to place
                {
                    neighbourTile.Suggest();
                }
                else
                {
                    neighbourTile.Unsuggest();
                }
            }
        }
    }

    private bool CanPathBePlacedOnTile(Tile tile)
    {
        Path tilePath = tile.IsOccupied ? tile.OccupierBuilding.GetComponent<Path>() : null;
        return (tile.IsOccupied == false || tilePath != null) && //if not occupied or occupied by path
               GameManager.Instance.Neighbours(TilePath.Last()).Contains(tile) && //isn't the last where tile were placed
               tile != TilePath[^2] &&
               _currentNumberOfPath > 0 && 
               TilePath.Count > 0;
    }

    public void ResetPath()
    {
        //suggest
        foreach (Tile tile in GameManager.Instance.TileArray)
        {
            tile.Unsuggest();
        }
        GameManager.Instance.Neighbours(TilePath[0]).ForEach(x => x.Suggest());
        
        //path clear
        foreach (Tile tile in TilePath)
        {
            if (tile != Departure.TileOccupied)
            {
                tile.SelectParticle.Play();
                tile.RemoveAnyBuilding();
            }
        }
        TilePath.Clear();
        TilePath.Add(Departure.TileOccupied);
        _currentNumberOfPath = _numberOfPathUsable;
        
        //ui
        _numberOfPathUsableText.text = _currentNumberOfPath.ToString();
        _confirmPathButton.interactable = false;
    }
    
    public void GoBack()
    {
        if (TilePath.Count <= 1)
        {
            return;
        }

        //path clear
        TilePath[^1].RemoveAnyBuilding();
        TilePath[^1].SelectParticle.Play();
        TilePath.Remove(TilePath[^1]);
        _currentNumberOfPath ++;
        
        //suggest 
        foreach (Tile tile in GameManager.Instance.TileArray)
        {
            tile.Unsuggest();
        }
        foreach (Tile tile in GameManager.Instance.Neighbours(TilePath[^1]))
        {
            if (tile != TilePath[^2])
            {
                tile.Suggest();
            }
        }

        //ui
        _numberOfPathUsableText.text = _currentNumberOfPath.ToString();
        _confirmPathButton.interactable = CanPathBeConfirmed();
    }
    
    public void ConfirmPath()
    {
        if (CanPathBeConfirmed() == false)
        {
            return;
        }
        TilePath.Add(Arrival.TileOccupied);
        foreach (Tile tile in TilePath)
        {
            GameManager.Instance.Neighbours(tile).ForEach(x => x.Unsuggest());
            tile.SelectParticle.Play();
        }
        GameManager.Instance.ChangeState(GameState.ManagingDefense);
    }

    private bool CanPathBeConfirmed()
    {
        List<Tile> neighbours = GameManager.Instance.Neighbours(Arrival.TileOccupied);
        return neighbours.Contains(TilePath.Last());
    }

    #endregion
}
