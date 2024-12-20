using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoseUI : NormalUI
{
    [SerializeField] Button _btnContinue, _btnNoThank;
    [SerializeField] float _timeDelayShowNoThank;
    protected override void Awake()
    {
        base.Awake();
        _btnContinue.onClick.AddListener(BtnContinue_Click);
        _btnNoThank.onClick.AddListener(LosePanel_Click);
    }

    

    private void OnDestroy()
    {
        _btnContinue.onClick.RemoveListener(BtnContinue_Click);
        _btnNoThank.onClick.RemoveListener(LosePanel_Click);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnUiShow()
    {
        base.OnUiShow();
        _btnNoThank.transform.DOKill();
        _btnNoThank.transform.localScale = Vector3.zero;
        _btnNoThank.transform.DOScale(1f, 0.5f).SetDelay(_timeDelayShowNoThank);
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
    }

    public void LosePanel_Click()
    {
        GameManager.Instance.ChangeGameState(GameState.Load);
        OnCloseUI();
    }

    void BtnContinue_Click()
    {
        GameData gameData = SaveManager.Instance.GameData;
        gameData.Level += 1;
        SaveManager.Instance.GameData = gameData;
        GameManager.Instance.ChangeGameState(GameState.Load);
        OnCloseUI();
    }


}
