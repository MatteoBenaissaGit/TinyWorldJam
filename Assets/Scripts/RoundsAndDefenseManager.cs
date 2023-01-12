using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundsAndDefenseManager : MonoBehaviour
{
    #region Variables

    [Header("Referencing"), SerializeField]
    private TextMeshProUGUI _leafNumberText;

    [SerializeField] private TextMeshProUGUI _antNumberText;
    [SerializeField] private List<GameObject> _roundsUIImagesList;
    [SerializeField] private List<Card> _cards;
    [SerializeField] private List<CardInfo> _cardsInfos;
    [SerializeField] private GameObject _cancelCardButton;
    [SerializeField] private Button _startRoundButton;

    [Header("Parameters"), Range(0, 3)] public float EnemiesSpeedToWalkATile;
    [Range(0, 10)] public int NumberOfAnts;
    [ReadOnly] public int NumberOfAvailaibleAnts;
    [Range(0, 10)] public int NumberOfLeafs;
    [ReadOnly] private int _currentRound = 0;

    [Space(10), ReadOnly, Header("Debug")] public bool IsPlacingCard;
    public Card SelectedCard;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        NumberOfAvailaibleAnts = NumberOfAnts;
        RefreshUI();
    }

    private void Update()
    {
        if (IsPlacingCard == false)
        {
            AddOrRemoveAnts();
        }

        ManageCardSelection();
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
        if (IsPlacingCard)
        {
            return;
        }

        _currentRound++;
        RefreshUI();
    }

    #endregion

    #region Defense Methods

    private void AddOrRemoveAnts()
    {
        Tile tile = GameManager.Instance.SelectedTile;
        if (Input.GetMouseButtonDown(0) &&
            tile != null &&
            tile.OccupierBuilding != null &&
            tile.OccupierBuilding.CanBeUsed)
        {
            Building building = tile.OccupierBuilding;
            switch (building.IsUsed)
            {
                case true:
                    if (building.MaxAntsInBuilding <= 1)
                    {
                        building.Unuse();
                        NumberOfAvailaibleAnts++;
                    }
                    else
                    {
                        CheckUseOrUnuseForBuildingsWithMoreAnts(building);
                    }
                    
                    break;
                
                case false:
                    if (NumberOfAvailaibleAnts > 0)
                    {
                        if (building.MaxAntsInBuilding <= 1)
                        {
                            building.Use();
                            NumberOfAvailaibleAnts--;
                        }
                        else
                        {
                            CheckUseOrUnuseForBuildingsWithMoreAnts(building);
                        }
                    }

                    break;
            }
        }

        RefreshUI();
    }

    private void CheckUseOrUnuseForBuildingsWithMoreAnts(Building building)
    {
        if (building.HaveToUse(NumberOfAvailaibleAnts))
        {
            building.Use();
            NumberOfAvailaibleAnts--;
        }
        else
        {
            building.Unuse();
            NumberOfAvailaibleAnts++;
        }
    }

    private void ManageCardSelection()
    {
        _startRoundButton.interactable = IsPlacingCard == false;
        _cancelCardButton.SetActive(IsPlacingCard);

        Tile tile = GameManager.Instance.SelectedTile;
        if (tile != null &&
            tile.IsOccupied == false &&
            IsPlacingCard && SelectedCard != null & SelectedCard.CardInfoData.CardBuilding != null &&
            SelectedCard.CardInfoData.Cost <= NumberOfLeafs &&
            Input.GetMouseButtonDown(0))
        {
            NumberOfLeafs -= SelectedCard.CardInfoData.Cost;
            RefreshUI();
            tile.SetBuilding(SelectedCard.CardInfoData.CardBuilding, -0.4f);
            UseCard(SelectedCard);
        }
    }

    public void UnselectCard()
    {
        SelectedCard.AnimateDown();
        IsPlacingCard = false;
        SelectedCard = null;
    }

    private void UseCard(Card card)
    {
        UnselectCard();
        //change card
        card.CardInfoData = GetRandomCard();
        card.SetupCard();
    }

    private CardInfo GetRandomCard()
    {
        System.Random rnd = new System.Random();
        int randomNumber = rnd.Next(0, _cardsInfos.Count);
        return _cardsInfos[randomNumber];
    }

    #endregion
}