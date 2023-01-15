using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RoundsAndDefenseManager : MonoBehaviour
{
    #region Variables

    [Header("Referencing"), SerializeField]
    private TextMeshProUGUI _leafNumberText;

    [SerializeField] private TextMeshProUGUI _antCurrentNumberText;
    [SerializeField] private TextMeshProUGUI _antTotalNumberText;
    [SerializeField] private List<GameObject> _roundsUIImagesList;
    [SerializeField] private List<Card> _cards;
    [SerializeField] private List<CardInfo> _cardsInfos;
    [SerializeField] private GameObject _cancelCardButton;
    [SerializeField] private Button _startRoundButton;
    [SerializeField] public Image LifeBarImage;
    [SerializeField] private Image _lifeBarDefenseImage;
    [SerializeField] private Image _timerCircularImage;
    [SerializeField] private TextMeshProUGUI _enemiesToGoText;
    [SerializeField] private PathManager _pathManager;

    [Header("Parameters")] [Range(0, 20)] public int NumberOfAnts;
    [ReadOnly] public int NumberOfAvailaibleAnts;
    [Range(0, 20)] public int NumberOfLeafs;
    [ReadOnly] private int _currentRound = 0;
    [SerializeField] private List<Wave> _waveList = new List<Wave>();

    [Space(10), ReadOnly, Header("Debug")] public bool IsPlacingCard;
    [ReadOnly] public Card SelectedCard;
    [ReadOnly] public bool OnHoverCard;
    [ReadOnly] public float TimeSinceCardSelected;

    private Wave _currentWave;

    [FormerlySerializedAs("_totalTimeOfWave")] [ReadOnly, SerializeField]
    private float _totalNumberOfEnemyGroupInWave;

    [ReadOnly, SerializeField] private int _enemiesPassed;
    [ReadOnly, SerializeField] private float _spawnEnemyTime;
    [ReadOnly, SerializeField] private float _currentSpawnEnemyTime;
    [ReadOnly, SerializeField] private int _currentEnemyGroup;
    [ReadOnly, SerializeField] private int _currentEnemyInEnemyGroup;
    [ReadOnly, SerializeField] private List<GameObject> _enemyWaveList = new List<GameObject>();
    [ReadOnly, SerializeField] private int _currentEnemy;
    [ReadOnly] public bool IsAttacking;

    #endregion

    #region Unity Engine Methods

    private void Start()
    {
        NumberOfAvailaibleAnts = NumberOfAnts;
        RefreshUI();
        LifeBarImage.fillAmount = 1;
        _lifeBarDefenseImage.fillAmount = 1;
    }

    private void Update()
    {
        TimeSinceCardSelected -= Time.deltaTime;

        if (GameManager.Instance.CurrentGameState != GameState.ManagingDefense &&
            GameManager.Instance.CurrentGameState != GameState.Attack)
        {
            return;
        }

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

    public void RefreshUI()
    {
        _leafNumberText.text = NumberOfLeafs.ToString();
        _antCurrentNumberText.text = NumberOfAvailaibleAnts.ToString();
        _antTotalNumberText.text = NumberOfAnts.ToString();
        for (int i = 0; i < _roundsUIImagesList.Count; i++)
        {
            _roundsUIImagesList[i].SetActive(i < _currentRound);
        }

        if (_pathManager.Arrival != null)
        {
            _lifeBarDefenseImage.fillAmount = _pathManager.Arrival.CurrentLife / _pathManager.Arrival.Life;
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
        if (Input.GetMouseButtonUp(0) && 
            SelectedCard != null && 
            IsPlacingCard && 
            tile != null &&
            TimeSinceCardSelected <= 0)
        {
            
            if (tile.IsOccupied == false && SelectedCard.CardInfoData.CardBuilding != null && SelectedCard.CardInfoData.Cost <= NumberOfLeafs)
            {
                NumberOfLeafs -= SelectedCard.CardInfoData.Cost;
                RefreshUI();
                tile.SetBuilding(SelectedCard.CardInfoData.CardBuilding, -0.4f);
                UseCard(SelectedCard, true);
            }
            else
            {
                GameManager.Instance.SelectionUI.transform.DOShakePosition(0.3f, new Vector3(0.15f, 0, 0.15f), 20, 0f);
            }
        }
    }

    public void UnselectCard()
    {
        SelectedCard.AnimateDown();
        IsPlacingCard = false;
        SelectedCard = null;
    }

    public void UseCard(Card card, bool animate)
    {
        UnselectCard();
        //change card
        card.CardInfoData = GetRandomCard();
        card.SetupCard(animate);
    }

    public CardInfo GetRandomCard()
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

        GameManager.Instance.ChangeState(GameState.Attack);

        //calculate wave
        _currentWave = _waveList[_currentRound];
        //list for each wave to complete the path
        List<float> timeForEnemyToCompletePathList = new List<float>();
        foreach (EnemyGroup enemyGroup in _currentWave.EnemyGroupList)
        {
            timeForEnemyToCompletePathList.Add(enemyGroup.SpeedToCrossOneTile * _pathManager.TilePath.Count);
        }

        //reset enemy wave list
        _currentEnemyInEnemyGroup = 0;
        _currentEnemyGroup = 0;
        int numberOfEnemies = 0;
        _enemiesPassed = 0;
        _enemyWaveList.Clear();
        //_enemyWaveList
        foreach (EnemyGroup enemyGroup in _currentWave.EnemyGroupList)
        {
            numberOfEnemies += enemyGroup.AmountOfEnemies;
            for (int i = 0; i < enemyGroup.AmountOfEnemies; i++)
            {
                _enemyWaveList.Add(enemyGroup.EnemyType);
            }
        }

        //total time (circular fill)
        _totalNumberOfEnemyGroupInWave = _currentWave.EnemyGroupList.Count;
        _enemiesToGoText.text = (_enemyWaveList.Count - _enemiesPassed).ToString();
        //spawn timer
        _spawnEnemyTime = _currentWave.EnemyGroupList[_currentEnemyGroup].SpeedToCrossOneTile;
        _currentSpawnEnemyTime = _spawnEnemyTime;

        //attack towers
        foreach (Building building in GameManager.Instance.BuildingList)
        {
            AttackBuilding attackBuilding = building.GetComponent<AttackBuilding>();
            if (attackBuilding != null)
            {
                attackBuilding.Launch();
            }
        }

        //ui & state
        _currentRound++;
        RefreshUI();
        IsAttacking = true;
        _timerCircularImage.fillAmount = 1;
        float timeToFill = _waveList[_currentRound - 1].EnemyGroupList[_currentEnemyGroup].TimeToSpawnWave();
        _timerCircularImage
            .DOFillAmount(1 - (float)_currentEnemyGroup - 1 / (float)_totalNumberOfEnemyGroupInWave, timeToFill)
            .SetEase(Ease.Linear);
    }

    private void TimerManagement()
    {
        //wave
        if (_currentEnemyGroup == _totalNumberOfEnemyGroupInWave - 1 && FindObjectsOfType<Enemy>().Length == 0)
        {
            print("end of wave");
            EndOfWave();
        }

        //enemy spawn
        _currentSpawnEnemyTime -= Time.deltaTime;
        if (_currentSpawnEnemyTime <= 0 && _currentEnemy < _enemyWaveList.Count && IsAttacking)
        {
            SpawnEnemy(_enemyWaveList[_currentEnemy]);
            _currentEnemy++;

            //check for next enemyGroup
            _currentEnemyInEnemyGroup++;
            if (_currentEnemyInEnemyGroup >= _currentWave.EnemyGroupList[_currentEnemyGroup].AmountOfEnemies &&
                _currentEnemyGroup < _currentWave.EnemyGroupList.Count - 1)
            {
                _currentEnemyGroup++;
                _currentEnemyInEnemyGroup = 0;
                //ui
                EnemyGroup passedGroup = _waveList[_currentRound - 1].EnemyGroupList[_currentEnemyGroup - 1];
                EnemyGroup group = _waveList[_currentRound - 1].EnemyGroupList[_currentEnemyGroup];
                float timeToFill = group.TimeToSpawnWave() + passedGroup.WaitTimeAtEnd + group.SpeedToCrossOneTile;
                _currentSpawnEnemyTime = passedGroup.WaitTimeAtEnd == 0 ? 1 : passedGroup.WaitTimeAtEnd;
                _timerCircularImage
                    .DOFillAmount(1 - (float)_currentEnemyGroup - 1 / (float)_totalNumberOfEnemyGroupInWave, timeToFill)
                    .SetEase(Ease.Linear);
            }

            _spawnEnemyTime = _currentWave.EnemyGroupList[_currentEnemyGroup].SpeedToCrossOneTile;
            if (_currentEnemyInEnemyGroup > 0)
            {
                _currentSpawnEnemyTime = _spawnEnemyTime;
            }
        }

        //ui
        RefreshUI();
    }

    private void SpawnEnemy(GameObject enemy)
    {
        GameObject spawnedEnemy = Instantiate(enemy, _pathManager.Departure.transform.position, Quaternion.identity);
        Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();

        float timeForEnemyToCompletePath = _pathManager.TilePath.Count *
                                           _waveList[_currentRound - 1].EnemyGroupList[_currentEnemyGroup]
                                               .SpeedToCrossOneTile;

        enemyScript.Timer = timeForEnemyToCompletePath;

        enemyScript.TilePath = _pathManager.TilePath;
        enemyScript.MoveTime = _currentWave.EnemyGroupList[_currentEnemyGroup].SpeedToCrossOneTile;

        _enemiesPassed++;
        _enemiesToGoText.text = (_enemyWaveList.Count - _enemiesPassed).ToString();
    }

    public void EndOfWave()
    {
        IsAttacking = false;

        //check win or continue rounds
        if (_currentRound > 5)
        {
            GameManager.Instance.ChangeState(GameState.Win);
            return;
        }

        GameManager.Instance.ChangeState(GameState.ManagingDefense);
        _currentEnemy = 0;
        CheckRessources();
    }

    private void CheckRessources()
    {
        List<Building> list = GameManager.Instance.BuildingList;
        List<Building> alreadySeen = new List<Building>();
        foreach (Building building in list)
        {
            RessourceBuilding ressourceBuilding = building.GetComponent<RessourceBuilding>();
            if (building.CanBeUsed && ressourceBuilding != null && building.IsUsed &&
                alreadySeen.Contains(building) == false)
            {
                ressourceBuilding.LeafParticle.Play();
                NumberOfLeafs++;
            }

            alreadySeen.Add(building);
        }

        RefreshUI();
    }

    #endregion
}

[Serializable]
public struct EnemyGroup
{
    public int AmountOfEnemies;
    public GameObject EnemyType;
    public float SpeedToCrossOneTile;
    public float WaitTimeAtEnd;

    public float TimeToCompleteWave(int tile)
    {
        return tile * SpeedToCrossOneTile + AmountOfEnemies * SpeedToCrossOneTile;
    }

    public float TimeToSpawnWave()
    {
        return SpeedToCrossOneTile * AmountOfEnemies + SpeedToCrossOneTile * 2;
    }
}

[Serializable]
public struct Wave
{
    public List<EnemyGroup> EnemyGroupList;
}