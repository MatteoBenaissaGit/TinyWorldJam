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
    [SerializeField] public Image LifeBarImage;
    [SerializeField] private Image _timerCircularImage;
    [SerializeField] private PathManager _pathManager;

    [Header("Parameters")]
    [Range(0, 10)] public int NumberOfAnts;
    [ReadOnly] public int NumberOfAvailaibleAnts;
    [Range(0, 10)] public int NumberOfLeafs;
    [ReadOnly] private int _currentRound = 0;
    [SerializeField] private List<Wave> _waveList = new List<Wave>();

    [Space(10), ReadOnly, Header("Debug")] public bool IsPlacingCard;
    public Card SelectedCard;

    private Wave _currentWave;
    [ReadOnly, SerializeField] private float _totalTimeOfWave;
    [ReadOnly, SerializeField] private float _currentTimeOfWave;
    [ReadOnly, SerializeField] private float _spawnEnemyTime;
    [ReadOnly, SerializeField] private float _currentSpawnEnemyTime;
    [ReadOnly, SerializeField] private List<GameObject> _enemyWaveList = new List<GameObject>();
    [ReadOnly, SerializeField] private int _currentEnemy;
    [ReadOnly] public bool IsAttacking;
    
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

        if (IsAttacking)
        {
            TimerManagement();
        }
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

        if (_pathManager.Arrival != null)
        {
            LifeBarImage.fillAmount = _pathManager.Arrival.CurrentLife / _pathManager.Arrival.Life;
        }
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

    #region Attack Methods

    public void StartRound()
    {
        if (IsPlacingCard || _currentRound >= _waveList.Count)
        {
            return;
        }
        
        //calculate wave
        _currentWave = _waveList[_currentRound];
        float timeForEnemyToCompletePath = _pathManager.TilePath.Count * _currentWave.EnemiesSpeedToWalkATile;
        int numberOfEnemies = 0;
        _enemyWaveList.Clear();
        foreach (EnemyGroup group in _waveList[_currentRound].EnemyGroupList)
        {
            numberOfEnemies += group.AmountOfEnemies;
            for (int i = 0; i < group.AmountOfEnemies; i++)
            {
                _enemyWaveList.Add(group.EnemyType);
            }
        }
        _totalTimeOfWave = timeForEnemyToCompletePath + numberOfEnemies * _currentWave.EnemiesSpeedToWalkATile;
        _currentTimeOfWave = _totalTimeOfWave;
        _spawnEnemyTime = _currentWave.EnemiesSpeedToWalkATile;
        _currentSpawnEnemyTime = _spawnEnemyTime;
        
        //ui & state
        _currentRound++;
        RefreshUI();
        GameManager.Instance.ChangeState(GameState.Attack);
        
        IsAttacking = true;
    }

    private void TimerManagement()
    {
        //wave
        _currentTimeOfWave -= Time.deltaTime;
        if (_currentTimeOfWave <= 0)
        {
            EndOfWave();
        }
        
        //enemy spawn
        _currentSpawnEnemyTime -= Time.deltaTime;
        if (_currentSpawnEnemyTime <= 0 && _currentEnemy < _enemyWaveList.Count)
        {
            SpawnEnemy(_enemyWaveList[_currentEnemy]);
            _currentEnemy++;
            _currentSpawnEnemyTime = _spawnEnemyTime;
        }
        
        //ui
        RefreshUI();
        _timerCircularImage.fillAmount = _currentTimeOfWave / _totalTimeOfWave;
    }

    private void SpawnEnemy(GameObject enemy)
    {
        GameObject spawnedEnemy = Instantiate(enemy, _pathManager.Departure.transform.position, Quaternion.identity);
        Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();
        float timeForEnemyToCompletePath = _pathManager.TilePath.Count * _currentWave.EnemiesSpeedToWalkATile;
        enemyScript.Timer = timeForEnemyToCompletePath;
        enemyScript.TilePath = _pathManager.TilePath;
        enemyScript.MoveTime = _currentWave.EnemiesSpeedToWalkATile;
    }

    private void EndOfWave()
    {
        _currentEnemy = 0;
        IsAttacking = false;
        GameManager.Instance.ChangeState(GameState.ManagingDefense);
    }

    #endregion
}

[Serializable]
public struct EnemyGroup
{
    public int AmountOfEnemies;
    public GameObject EnemyType;

}

[Serializable]
public struct Wave
{
    public float EnemiesSpeedToWalkATile;
    public List<EnemyGroup> EnemyGroupList;
}