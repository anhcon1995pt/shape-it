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
    [SerializeField] Button _btnContinue, _btnRetry;
    [SerializeField] float _timeDelayShowNoThank;
    Sequence _sequenceBtnRetry;
    protected override void Awake()
    {
        base.Awake();
        _btnContinue.onClick.AddListener(BtnContinue_Click);
        _btnRetry.onClick.AddListener(LosePanel_Click);
    }

    

    private void OnDestroy()
    {
        _btnContinue.onClick.RemoveListener(BtnContinue_Click);
        _btnRetry.onClick.RemoveListener(LosePanel_Click);
    }

    private void OnDisable()
    {
        _btnRetry.transform.DOKill();
        if (_sequenceBtnRetry != null)
            _sequenceBtnRetry.Kill();
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
        if(_sequenceBtnRetry != null ) 
            _sequenceBtnRetry.Kill();
        _sequenceBtnRetry = DOTween.Sequence(_btnRetry.transform);
        
        _sequenceBtnRetry.Append(_btnRetry.transform.DOShakePosition(1.2f, new Vector3(0, 15, 0), 8, randomness: 90));
        _sequenceBtnRetry.SetDelay(2.5f, true);
        _sequenceBtnRetry.SetLoops(-1);
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
        if (_sequenceBtnRetry != null)
            _sequenceBtnRetry.Kill();
    }

    public void LosePanel_Click()
    {
        GameManager.Instance.ChangeGameState(GameState.Load);
        OnCloseUI();
    }

    void BtnContinue_Click()
    {
        //GameData gameData = SaveManager.Instance.GameData;
        //gameData.Level += 1;
        //SaveManager.Instance.GameData = gameData;
        GameManager.Instance.ChangeGameState(GameState.Revive);
        OnCloseUI();
    }


}
