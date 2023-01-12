using System;
using DG.Tweening;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    #region Editor Variables

    [Header("Referencing"), SerializeField] private GameObject _suggestion;
    [SerializeField] private Building _arrivalPrefab;
    [SerializeField] private Building _departurePrefab;
    [SerializeField] private Building _waterPrefab;
    [SerializeField] private Building _ressourceBuildingPrefab;

    [Space(10), ReadOnly] public bool IsOccupied;
    [ReadOnly] public Building OccupierBuilding;
    [ReadOnly] public bool IsSelected;
    [ReadOnly] public bool IsSuggested;
    [ReadOnly] public bool IsPreviewed;

    private GameObject _previewBuilding;

    #endregion

    #region Private Variables

    private Vector3 _baseSuggestionScale;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        _baseSuggestionScale = _suggestion.transform.localScale;
    }

    private void Update()
    {
        if ((IsPreviewed && GameManager.Instance.SelectedTile != this) ||
            (IsOccupied && IsPreviewed))
        {
            Destroy(_previewBuilding);
            _previewBuilding = null;
            IsPreviewed = false;
        }
    }

    #endregion

    #region Selection

    public void Select()
    {
        if (IsSelected)
        {
            return;
        }
        
        IsSelected = true;
    }

    public void Unselect()
    {
        if (IsSelected == false)
        {
            return;
        }
        
        IsSelected = false;
    }

    public void RemoveAnyBuilding()
    {
        if (OccupierBuilding != null)
        {
            DestroyImmediate(OccupierBuilding.gameObject);
            IsOccupied = false;
            OccupierBuilding = null;
        }
    }

    #endregion

    #region Suggestion

    public void Suggest()
    {
        if (IsSuggested)
        {
            return;
        }
        
        _suggestion.SetActive(true);
        IsSuggested = true;
        
        //anim
        _suggestion.transform.DOComplete();
        _suggestion.transform.localScale = Vector3.zero;
        _suggestion.transform.DOScale(_baseSuggestionScale, 0.2f).SetEase(Ease.InExpo);
    }
    
    public void Unsuggest()
    {
        if (IsSuggested == false)
        {
            return;
        }
        
        IsSuggested = false;
        
        //anim
        _suggestion.transform.DOComplete();
        _suggestion.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InExpo).OnComplete(DeactivateSuggestion);
    }

    private void DeactivateSuggestion()
    {
        _suggestion.SetActive(false);
    }

    #endregion

    #region Buildings

    public void SetArrival()
    {
        SetBuilding(_arrivalPrefab, 0);
    }
    
    public void SetDeparture()
    {
        SetBuilding(_departurePrefab, 0);
    }
    
    public void SetWater()
    {
        SetBuilding(_waterPrefab, -0.4f);
    }
    
    public void SetRessourceBuilding()
    {
        SetBuilding(_ressourceBuildingPrefab, -0.4f);
    }

    public void SetBuilding(Building buildingToSpawn, float offsetY)
    {
        if (IsOccupied)
        {
            return;
        }
        
        Building building = Instantiate(buildingToSpawn, transform.position + Vector3.up + new Vector3(0,offsetY,0), Quaternion.identity);
        building.transform.SetParent(transform);
        IsOccupied = true;
        OccupierBuilding = building;
        building.TileOccupied = this;
    }

    public void SetPreviewBuilding(GameObject previewBuilding, float offsetY)
    {
        if (IsOccupied || IsPreviewed)
        {
            return;
        }
        
        _previewBuilding = Instantiate(previewBuilding, transform.position + Vector3.up + new Vector3(0,offsetY,0), Quaternion.identity);
        _previewBuilding.transform.SetParent(transform);
        IsPreviewed = true;
    }

    #endregion
}