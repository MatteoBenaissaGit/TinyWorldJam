using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundsAndDefenseManager : MonoBehaviour
{
    #region Variables

    [Header("Referencing"), SerializeField] private TextMeshProUGUI _leafNumberText;
    [SerializeField] private TextMeshProUGUI _antNumberText;
    [SerializeField] private List<GameObject> _roundsUIImagesList;
    
    [Header("Parameters") ,Range(0, 3)] public float EnemiesSpeedToWalkATile;
    [Range(0, 10)] public int NumberOfAnts;
    [ReadOnly] public int NumberOfAvailaibleAnts;
    [Range(0, 10)] public int NumberOfLeafs;
    [ReadOnly] private int _currentRound = 0;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        NumberOfAvailaibleAnts = NumberOfAnts;
        RefreshUI();
    }

    private void Update()
    {
        AddOrRemoveAnts();
    }

    #endregion

    #region Methods

    private void RefreshUI()
    {
        _leafNumberText.text = NumberOfLeafs.ToString();
        _antNumberText.text = $"{NumberOfAvailaibleAnts}/{NumberOfAnts}";
        for (int i = 0; i < _roundsUIImagesList.Count; i++)
        {
            _roundsUIImagesList[i].SetActive(i < _currentRound);
        }
    }

    public void StartRound()
    {
        _currentRound++;
        RefreshUI();
    }

    #endregion

    #region Defense Methods

    private void AddOrRemoveAnts()
    {
        Tile tile = GameManager.Instance.SelectedTile;
        if (tile != null &&
            tile.OccupierBuilding != null &&
            tile.OccupierBuilding.canBeUsed &&
            Input.GetMouseButtonDown(0))
        {
            Building building = tile.OccupierBuilding;
            switch (building.isUsed)
            {
                case true:
                    building.Unuse();
                    NumberOfAvailaibleAnts++;
                    break;
                case false:
                    if (NumberOfAvailaibleAnts > 0)
                    {
                        building.Use();
                        NumberOfAvailaibleAnts--;
                    }
                    break;
            }
        }
        
        RefreshUI();
    }

    #endregion
}
