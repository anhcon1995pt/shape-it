using AC.GameTool.Attribute;
using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleItem : MonoBehaviour
{
    [ReadOnly]
    public int Id;
    [SerializeField, ReadOnly] SpriteRenderer _mainSpriteRen;
    [ReadOnly]
    public bool IsCorrect;
    [SerializeField] GameObject _lineObj, _backObj;

    
    public SpriteRenderer MainSpriteRen => _mainSpriteRen;
    public Sprite MainSprite => _mainSpriteRen.sprite;
    private void Awake()
    {
        _mainSpriteRen = GetComponent<SpriteRenderer>();
    }
    private void OnDestroy()
    {
        _mainSpriteRen.DOKill();
    }
    private void OnDisable()
    {
        _mainSpriteRen.DOKill();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangePuzzleAlpha(float alpha, float time)
    {

        _mainSpriteRen.DOKill();
        if (time <= 0f)
        {
            Color spriteColor = _mainSpriteRen.color;
            spriteColor.a = alpha;
            _mainSpriteRen.color = spriteColor;
        }
        else
        {
            _mainSpriteRen.DOFade(alpha, time);
        }
    }

    public void SetPuzzleItemIsCorrect()
    {
        IsCorrect = true;
        if(CanvasManager.Instance.GameplayUI.PuzzleHint != null && Id == CanvasManager.Instance.GameplayUI.PuzzleHint.Id)
        {
            CanvasManager.Instance.GameplayUI.DeActiveHints();
        }
        
        ShowLineObj(false);
        ChangePuzzleAlpha(1f, 0);
        GameManager.Instance.PuzzleSuccessedToCombo();
        MapManager.Instance.CurrentLevelCtrl.CheckWinGame();
        GameManager.Instance.ShowEarnCoinCorrect(transform.position);
        GameData gameData = SaveManager.Instance.GameData;
        if (!gameData.IsTutCompleted)
        {
            if (gameData.TutIndex == 1)
            {
                gameData.TutIndex = 2;
                gameData.IsTutCompleted = true;
                GameManager.Instance.IsTutCompleted = true;
                SaveManager.Instance.GameData = gameData;
                UIManager.Instance.TryCloseUIJustOpen(UiTypeName.Tutorial);
            }
        }
        
        VibrateManager.Instance.VibratePuzzleCorrect();
    }
    public void RunAnimationFadeItem()
    {
        _mainSpriteRen.DOKill();
        Color color = _mainSpriteRen.color;
        color.a = 0f;
        _mainSpriteRen.color = color;
        _mainSpriteRen.DOFade(1f, 0.35f).SetEase(Ease.Linear, 10, 1).SetLoops(-1, LoopType.Yoyo).OnComplete(() =>
        {
            Color color = _mainSpriteRen.color;
            color.a = 0f;
            _mainSpriteRen.color = color;
        });
    }
    public void StopAnimationFadeItem()
    {
        _mainSpriteRen.DOKill();
        Color color = _mainSpriteRen.color;
        color.a = 0f;
        _mainSpriteRen.color = color;
    }
    public void ShowLineObj(bool isShow)
    {
        if(_lineObj != null)
            _lineObj.SetActive(isShow);
        if(_backObj != null)
        {
            _backObj.SetActive(isShow);
        }

    }
}
