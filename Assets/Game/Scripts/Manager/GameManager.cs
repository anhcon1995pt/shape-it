using AC.GameTool.Attribute;
using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : Singleton<GameManager>
{
    [SerializeField, ReadOnly]
    GameState _gameState;
    [SerializeField, ReadOnly] int _levelId;
    [SerializeField, ReadOnly] LevelData _currentLevelData;
    [ReadOnly]
    public bool IsWinGame;
    [Header("Time")]
    [SerializeField] float _timePlay;
    [SerializeField] float _currentTimePlay;
    [Range(0f, 1f)]
    [SerializeField, ReadOnly] float _timePercent;
    [Header("Time Freeze")]
    [SerializeField, ReadOnly] bool _isFreezeGame;
    [SerializeField] float _timeToFreeze;
    [SerializeField, ReadOnly] float _curentTimeFreeze;
    [Header("Combo")]
    [SerializeField] int _comboCount = 0;
    [SerializeField] bool _isCombo;
    Coroutine _comboTimer;
    [SerializeField] AssetReference _coinPrefab;
    [SerializeField, ReadOnly] bool _isTutCompleted;
    //Tween _timeplatTween;

    public Action<GameState> OnBeforeGameStateChanged;
    public Action<GameState> OnAfterGameStateChanged;
    public GameState GameState => _gameState;

    public bool IsPauseGame;
    public bool IsFreezeGame => _isFreezeGame;

    public bool IsTutCompleted { get => _isTutCompleted; set => _isTutCompleted = value; }

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Load);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            UIManager.Instance.TryShowUI(UiTypeName.Lose);
        }
        if(_gameState == GameState.Play)
        {
            
            if(!_isFreezeGame)
            {
                if (!IsTutCompleted) return;
                if (_currentTimePlay > 0)
                {                    
                    _timePercent = _currentTimePlay / _timePlay;
                    CanvasManager.Instance.GameplayUI.UpdateTimebar(_currentTimePlay, _timePercent);
                    int mutil = _comboCount <= 5 ? (_comboCount - 1) : 5;
                    _currentTimePlay -= Time.deltaTime * (1 - 0.1f * mutil);
                }
                else
                {
                    _currentTimePlay = 0;
                    _timePercent = 0;
                    CanvasManager.Instance.GameplayUI.UpdateTimebar(0, 0);
                    IsWinGame = false;
                    ChangeGameState(GameState.End);
                }
            }
            else
            {
                if (_curentTimeFreeze > 0)
                {
                    float freezePercent = _curentTimeFreeze / _timeToFreeze;
                    CanvasManager.Instance.GameplayUI.UpdateFreezeTimePercent(freezePercent);
                    _curentTimeFreeze -= Time.deltaTime;
                }
                else
                {
                    _curentTimeFreeze = 0;
                    CanvasManager.Instance.GameplayUI.UpdateFreezeTimePercent(0);
                    CanvasManager.Instance.GameplayUI.ShowFreezeTimeer(false);
                    CanvasManager.Instance.GameplayUI.ShowTimeBar(true);
                    _isFreezeGame = false;
                }
            }
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        if (_gameState == gameState) return;
        OnBeforeGameStateChanged?.Invoke(_gameState);
        _gameState = gameState;
        switch (_gameState)
        {
            case GameState.Load:
                GameLoad();
                break;
            case GameState.Start:
                GameStart();
                break;
            case GameState.Play:
                GamePlay();
                break;
            case GameState.End:
                GameEnd();
                break;
            case GameState.Win:
                WinGame();
                break;
            case GameState.Lose:
                LoseGame();
                break;
            case GameState.Revive:
                PlayerRevive();
                break;
        }
        OnAfterGameStateChanged?.Invoke(_gameState);
    }

    void GameLoad()
    {
        //if(_timeplatTween != null)
        //    _timeplatTween.Kill();
        UIManager.Instance.TryShowUI(UiTypeName.Start);
        _levelId = SaveManager.Instance.GameData.Level;
        _currentLevelData = AssetManager.Instance.GameLevelDatas.ListLevelDatas[_levelId % AssetManager.Instance.CountLevel];
        _timePlay = _currentLevelData.TimeLevel;
        _currentTimePlay = _timePlay;
        IsWinGame = false;
        CanvasManager.Instance.GameplayUI.SetLevel(_levelId);
        CanvasManager.Instance.GameplayUI.ShowPuzzlePanel(false);

        MapManager.Instance.LoadLevel(_levelId, () =>
        {
            MapManager.Instance.CurrentLevelCtrl.ShowLevelInStart(false);
        });        
    }

    void GameStart()
    {
        CanvasManager.Instance.GameplayUI.ShowPuzzlePanel(true);
        MapManager.Instance.CurrentLevelCtrl.ShowLevelInStart(false);
        _comboCount = 0;
        ChangeGameState(GameState.Play);
    }

    void GamePlay()
    {
        _isFreezeGame = false;
        GameData gameData = SaveManager.Instance.GameData;
        IsTutCompleted = gameData.IsTutCompleted;
        if (!gameData.IsTutCompleted)
        {
            gameData.TutIndex = 0;
            SaveManager.Instance.GameData = gameData;
            UIManager.Instance.TryShowUI(UiTypeName.Tutorial);
        }
    }

    void GameEnd()
    {
        //if (_timeplatTween != null)
        //    _timeplatTween.Kill();
        _comboCount = 0;
        if (IsWinGame)
        {
            GameData gameData = SaveManager.Instance.GameData;
            gameData.Level += 1;
            SaveManager.Instance.GameData = gameData;
        }
        DOVirtual.DelayedCall(1.5f, () =>
        {
            ChangeGameState(IsWinGame? GameState.Win : GameState.Lose);
        });
    }
    void WinGame()
    {
        CanvasManager.Instance.ShowWinPanel();
        VibrateManager.Instance.VibrateGameWin();
    }

    void LoseGame()
    {
        CanvasManager.Instance.ShowLosePanel();
        VibrateManager.Instance.VibrateGameLose();
    }
    public void PlayerRevive()
    {
        _currentTimePlay = _timePlay;
        IsWinGame = false;
        ChangeGameState(GameState.Play);
    }

    public int GetCountStar()
    {

        if (_timePercent <= 0f) return 0;
        if (_timePercent < 0.5f) return 1;
        if (_timePercent < 0.8f) return 2;
        return 3;
    }

    public void ActiveFreezeGame()
    {
        _isFreezeGame = true;
        _curentTimeFreeze = _timeToFreeze;
        CanvasManager.Instance.GameplayUI.ShowFreezeTimeer(true);
        CanvasManager.Instance.GameplayUI.ShowTimeBar(false);
    }

    public int GetWinLevelCoinEarn()
    {
        return Mathf.RoundToInt(_currentLevelData.Coin / 3f * GetCountStar());
    }

    public void PuzzleSuccessedToCombo()
    {
        if (_comboTimer != null)
        {
            StopCoroutine(_comboTimer);
            _comboTimer = null;
        }
        _comboTimer = StartCoroutine(ComboAliveTime());
        if (_isCombo)
        {
            _comboCount += 1;
            CanvasManager.Instance.GameplayUI.ShowCombo(_comboCount);
        }
        else
        {
            _comboCount = 1;
        }
        _isCombo = true;
        
    }

    IEnumerator ComboAliveTime()
    {
        yield return new WaitForSeconds(5f);
        _isCombo = false;
    }

    public void ShowEarnCoinCorrect(Vector3 posisiopn)
    {
        GameData gameData = SaveManager.Instance.GameData;
        gameData.Coin += 10;
        SaveManager.Instance.GameData = gameData;
        GameObject coin = PoolManager.Instance.Spawn(_coinPrefab);
        coin.transform.localPosition = posisiopn;
        coin.SetActive(true);
    }
}
