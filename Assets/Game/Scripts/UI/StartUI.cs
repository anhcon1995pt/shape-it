using AC.GameTool.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : NormalUI
{
    [SerializeField] Button _btnPlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void Awake()
    {
        base.Awake();
        _btnPlay.onClick.AddListener(BtnPlay_Click);
    }
    private void OnDestroy()
    {
        _btnPlay.onClick.RemoveListener(BtnPlay_Click);
    }
    public override void OnUiShow()
    {
        base.OnUiShow();
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
    }

    void BtnPlay_Click()
    {
        GameManager.Instance.ChangeGameState(GameState.Start);
        OnCloseUI();
    }
}
