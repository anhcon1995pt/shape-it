using AC.GameTool.SaveData;
using AC.GameTool.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : NormalUI
{
    [SerializeField] GameObject _tut1, _tut2;
    protected override void Awake()
    {
        base.Awake();
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
        _tut1.SetActive(false);
        _tut1.SetActive(false);
        GameData gameData = SaveManager.Instance.GameData;
        if(gameData.IsTutCompleted)
        {
            OnCloseUI();
            return;
        }
        if(gameData.TutIndex > 1)
        {
            gameData.IsTutCompleted = true;
            SaveManager.Instance.GameData = gameData;
            OnCloseUI();
            return;
        }
        switch(gameData.TutIndex)
        {
            case 0:
                _tut1.SetActive(true);
                _tut2.SetActive(false);
                break;
            case 1:
                _tut1.SetActive(false);
                _tut2.SetActive(true);
                break;
            default:
                _tut1.SetActive(false);
                _tut2.SetActive(false);
                break;
        }
    }

    public override void OnCloseUI()
    {
        base.OnCloseUI();
    }
}
