using System;
using DG.Tweening;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    #region Editor Variables

    [Header("Referencing"), SerializeField] private GameObject _selection;
    [SerializeField] private Building _arrivalPrefab;
    [SerializeField] private Building _departurePrefab;
    [SerializeField] private Building _waterPrefab;
    [SerializeField] private Building _pathPrefab;

    [Space(10), ReadOnly] public bool IsOccupied;
    [ReadOnly] public Building OccupierBuilding;
    [ReadOnly] public bool IsSelected;

    #endregion

    #region Private Variables

    private Vector3 _basePosition;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        _selection.SetActive(false);
    }

    #endregion

    #region Selection

    public void Select()
    {
        if (IsSelected)
        {
            return;
        }
        
        _selection.SetActive(true);
        IsSelected = true;
    }

    public void Unselect()
    {
        if (IsSelected == false)
        {
            return;
        }
        
        _selection.SetActive(false);
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

    #endregion
}