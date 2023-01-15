using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region Singleton & Awake

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
        
        //tile list creation
        Tile[] array = FindObjectsOfType<Tile>();
        TileArray = new Tile[(int)_mapManager.GridSize.x, (int)_mapManager.GridSize.y];
        foreach (Tile tile in array)
        {
            Vector3 position = tile.transform.position;
            tile.gameObject.transform.parent.gameObject.name = $"{(int)position.x}.{(int)position.z}";
            TileArray[(int)position.x, (int)position.z] = tile;
        }
        //build list creation
        foreach (Building building in FindObjectsOfType<Building>())
        {
            BuildingList.Add(building);
        }
    }

    #endregion

    #region Variables

    [Header("Referencing"), SerializeField] private MapManager _mapManager;
    [SerializeField] public PathManager PathManager;
    [SerializeField] public RoundsAndDefenseManager RoundsAndDefenseManager;
    [SerializeField] private GameObject _pathCreationUI;
    public GameObject SelectionUI;
    [SerializeField] private GameObject _defenseUI;
    [SerializeField] private GameObject _roundsAttackUI;
    [SerializeField] private GameObject _winUI;
    [SerializeField] private GameObject _loseUI;

    [Header("Debug")]
    [ReadOnly] public GameState CurrentGameState = GameState.Start;
    [ReadOnly] public Tile SelectedTile;
    
    [HideInInspector] public Tile[,] TileArray = new Tile[,]{};
    [ReadOnly] public List<Building> BuildingList = new List<Building>();

    private GameObject _previewBuilding;
    private Tween _reduceSizeTween;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        //state
        ChangeState(GameState.PlacingPath);
    }

    private void Update()
    {
        SelectTiles();
        SelectionOnOffCheck();
        ShowAntUIBuilding();
        SetPreviewBuilding();
    }

    #endregion

    #region StateMachine

    public void ChangeState(GameState gameState)
    {
        if (CurrentGameState == gameState ||
            CurrentGameState is GameState.Win or GameState.Lose)
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
        
        //list update
        //build list creation
        foreach (Building building in FindObjectsOfType<Building>())
        {
            BuildingList.Add(building);
        }

        
        //UIs
        _pathCreationUI.SetActive(CurrentGameState == GameState.PlacingPath);
        _defenseUI.SetActive(CurrentGameState == GameState.ManagingDefense);
        _roundsAttackUI.SetActive(CurrentGameState == GameState.Attack);
        _winUI.SetActive(CurrentGameState == GameState.Win);
        _loseUI.SetActive(CurrentGameState == GameState.Lose);
        if (CurrentGameState != GameState.ManagingDefense)
        {
            BuildingList.Clear();
            BuildingList = FindObjectsOfType<Building>().ToList();
            BuildingList.ForEach(x => x.HideAntUI());
        }
    }

    #endregion
    
    #region Tile Methods

    private void SelectTiles()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && (CurrentGameState is GameState.Win or GameState.Lose == false))
        {
            Tile rayTile = hit.transform.gameObject.GetComponent<Tile>();
            if (rayTile != null)
            {
                rayTile.Select();
                if (SelectedTile != rayTile)
                {
                    SelectedTile = rayTile;
                    Vector3 rayPosition = rayTile.transform.position;
                    MoveSelectionTo(new Vector2(rayPosition.x, rayPosition.z));
                }
                foreach (Tile tile in TileArray)
                {
                    if (tile != rayTile)
                    {
                        tile.Unselect();
                    }
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
            Vector2 position = new Vector2(tilePosition.x + direction.x, tilePosition.z + direction.y);
            
            if (position.x >= 0 && position.x < (int)_mapManager.GridSize.x && position.y >= 0 &&
                position.y < (int)_mapManager.GridSize.y)
            {
                Tile neighbour = TileArray[(int)position.x,(int)position.y];
                if (neighbour != null && (neighbour.IsOccupied == false || neighbour.OccupierBuilding.GetComponent<Path>() 
                                                                        || neighbour.OccupierBuilding.GetComponent<Arrival>()
                                                                        || neighbour.OccupierBuilding.GetComponent<Departure>()))
                {
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    private void SelectionOnOffCheck()
    {
        if (SelectedTile != null && SelectionUI.activeInHierarchy == false && RoundsAndDefenseManager.OnHoverCard == false 
            && Instance.CurrentGameState != GameState.Attack)
        {
            SelectionUI.SetActive(true);
            SelectionUI.transform.localScale = Vector3.zero;
            SelectionUI.transform.DOComplete();
            SelectionUI.transform.DOScale(Vector3.one, 0.2f);
        }
        if ((SelectedTile == null || RoundsAndDefenseManager.OnHoverCard || GameManager.Instance.CurrentGameState == GameState.Attack ) 
            && SelectionUI.activeInHierarchy)
        {
            SelectionUI.transform.DOComplete();
            SelectionUI.transform.DOScale(Vector3.zero, 0.1f).OnComplete(DeactivateSelection);
        }
    }

    private void DeactivateSelection()
    {
        SelectionUI.SetActive(false);
    }

    private void MoveSelectionTo(Vector2 position)
    {
        SelectionUI.transform.DOMove(new Vector3(position.x, 0, position.y), 0.1f);
    }

    public void ShakeSelectionAnimation()
    {
        SelectionUI.transform.DOComplete();
        SelectionUI.transform.DOShakePosition(0.25f, new Vector3(0.2f,0,0.2f), 25, 0);
    }

    private void ShowAntUIBuilding()
    {
        if (SelectedTile != null && 
            SelectedTile.OccupierBuilding != null &&
            SelectedTile.OccupierBuilding.CanBeUsed)
        {
            SelectedTile.OccupierBuilding.ShowAntUI();
        }
    }

    #endregion

    #region Defense Method

    public void SetPreviewBuilding()
    {
        if (_previewBuilding != null)
        {
            float offsetY = 0;
            if (RoundsAndDefenseManager.SelectedCard != null)
            {
                offsetY = RoundsAndDefenseManager.SelectedCard.CardInfoData.PreviewBuildingOffsetY;
            }
            _previewBuilding.transform.position = SelectionUI.transform.position + new Vector3(0,offsetY,0);
        }
        
        
        //guard
        if (CurrentGameState != GameState.ManagingDefense ||
            SelectedTile == null || 
            RoundsAndDefenseManager.IsPlacingCard == false)
        {
            if (_previewBuilding != null)
            {
                // _previewBuilding.transform.DOComplete();
                // _reduceSizeTween = _previewBuilding.transform.DOScale(Vector3.zero, 0.3f).OnComplete(DestroyPreviewBuilding);
                Destroy(_previewBuilding);
            }
            return;
        }

        //set preview
        if (_previewBuilding == null && SelectedTile.IsOccupied == false)
        {
            float offsetY = RoundsAndDefenseManager.SelectedCard.CardInfoData.PreviewBuildingOffsetY;
            _previewBuilding = Instantiate(RoundsAndDefenseManager.SelectedCard.CardInfoData.PreviewBuilding,
                transform.position + Vector3.up + new Vector3(0,offsetY,0), Quaternion.identity);
            _previewBuilding.transform.position = SelectedTile.transform.position;
            Vector3 scale = _previewBuilding.transform.localScale;
            _previewBuilding.transform.localScale = Vector3.zero;
            _previewBuilding.transform.DOScale(scale, 0.3f);
            
            _previewBuilding.transform.SetParent(transform);
        }
        
        //preview despawn
        if ((SelectedTile.IsOccupied || SelectedTile == null) && 
            _previewBuilding != null && _reduceSizeTween == null)
        {
            _previewBuilding.transform.DOComplete();
            _reduceSizeTween = _previewBuilding.transform.DOScale(Vector3.zero, 0.3f).OnComplete(DestroyPreviewBuilding);
        }

        SelectedTile.IsPreviewed = true;
        foreach (Tile tile in TileArray)
        {
            if (SelectedTile != tile)
            {
                tile.IsPreviewed = false;
            }
        }
    }

    private void DestroyPreviewBuilding()
    {
        if (_previewBuilding != null)
        {
            Destroy(_previewBuilding);
            _previewBuilding = null;
            _reduceSizeTween = null;
        }
    }

    #endregion

    #region Round Methods

    public void EnemyAttainArrival(Enemy enemy)
    {
        PathManager.Arrival.SetLife(-enemy.Damage);
        RoundsAndDefenseManager.LifeBarImage.DOFillAmount(PathManager.Arrival.CurrentLife / (float)PathManager.Arrival.Life, 0.2f);
        RoundsAndDefenseManager.RefreshUI();
    }

    #endregion

    #region Menu methods

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
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