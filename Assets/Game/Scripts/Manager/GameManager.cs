using AC.GameTool.Attribute;
using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField, ReadOnlly]
    GameState _gameState;
    [SerializeField, ReadOnlly] int _levelId;
    [ReadOnlly]
    public bool IsWinGame;
    [Header("Time")]
    [SerializeField] float _timePlay;
    [SerializeField] float _currentTimePlay;
    [Range(0f, 1f)]
    [SerializeField, ReadOnlly] float _timePercent;
    //Tween _timeplatTween;

    public Action<GameState> OnBeforeGameStateChanged;
    public Action<GameState> OnAfterGameStateChanged;
    public GameState GameState => _gameState;

    public bool IsPauseGame;
    
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
        if(_gameState == GameState.Play)
        {
            if(_currentTimePlay > 0)
            {
                _timePercent = _currentTimePlay / _timePlay;
                CanvasManager.Instance.GameplayUI.UpdateTimebar(_currentTimePlay, _timePercent);
                _currentTimePlay -= Time.deltaTime;
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
        UIManager.Instance.TryShowUI(UITypeName.StartUI);
        _levelId = SaveManager.Instance.GameData.Level;

        _timePlay = AssetManager.Instance.GameLevelDatas.ListLevelDatas[_levelId % AssetManager.Instance.CountLevel].TimeLevel;
        _currentTimePlay = _timePlay;
        IsWinGame = false;
        CanvasManager.Instance.GameplayUI.SetLevel(_levelId);
        CanvasManager.Instance.GameplayUI.ShowPuzzlePanel(false);

        MapManager.Instance.LoadLevel(_levelId, () =>
        {
            MapManager.Instance.CurrentLevelCtrl.ShowLevelInStart(true);
        });        
    }

    void GameStart()
    {
        CanvasManager.Instance.GameplayUI.ShowPuzzlePanel(true);
        MapManager.Instance.CurrentLevelCtrl.ShowLevelInStart(false);
        ChangeGameState(GameState.Play);
    }

    void GamePlay()
    {
        //_timeplatTween = DOVirtual.Float(1f, 0f, _timePlay, percent =>
        //{
        //    _timePercent = percent;
        //    CanvasManager.Instance.GameplayUI.UpdateTimebar(_timePlay * percent,percent);
        //}).SetEase(Ease.Linear).OnComplete(() =>
        //{
        //    _timePercent = 0;
        //    CanvasManager.Instance.GameplayUI.UpdateTimebar(0, 0);
        //    if (_gameState == GameState.Play)
        //    {
        //        IsWinGame = false;
        //        ChangeGameState(GameState.End);
        //    }
        //});
    }

    void GameEnd()
    {
        //if (_timeplatTween != null)
        //    _timeplatTween.Kill();
        if(IsWinGame)
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
    }

    void LoseGame()
    {
        CanvasManager.Instance.ShowLosePanel();
    }
    void PlayerRevive()
    {

    }

    public int GetCountStar()
    {

        if (_timePercent <= 0f) return 0;
        if (_timePercent < 0.5f) return 1;
        if (_timePercent < 0.8f) return 2;
        return 3;
    }
}
