using AC.GameTool.Core;
using AC.GameTool.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    public GameplayUI _gameplayUI;

    public GameplayUI GameplayUI => _gameplayUI;
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        _gameplayUI = UIManager.Instance.TryShowUI(UITypeName.Gameplay) as GameplayUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupAllPuzzle(List<PuzzleItem> listPuzzle)
    {
        _gameplayUI.SetupAllPuzzle(listPuzzle);
    }

    public void ShowWinPanel()
    {
        UIManager.Instance.TryShowUI(UITypeName.WinUI);
    }

    public void ShowLosePanel()
    {
        UIManager.Instance.TryShowUI(UITypeName.LoseUI);
    }
}
