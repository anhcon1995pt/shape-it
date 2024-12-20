using AC.GameTool.Attribute;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleItem : MonoBehaviour
{
    [ReadOnlly]
    public int Id;
    [SerializeField, ReadOnlly] SpriteRenderer _mainSpriteRen;
    [ReadOnlly]
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
        ShowLineObj(false);
        ChangePuzzleAlpha(1f, 0);
        MapManager.Instance.CurrentLevelCtrl.CheckWinGame();
    }
    public void RunAnimationFadeItem()
    {
        _mainSpriteRen.DOKill();
        Color color = _mainSpriteRen.color;
        color.a = 0f;
        _mainSpriteRen.color = color;
        _mainSpriteRen.DOFade(1f, 1f).SetEase(Ease.InOutFlash, 10, 1).OnComplete(() =>
        {
            Color color = _mainSpriteRen.color;
            color.a = 0f;
            _mainSpriteRen.color = color;
        });
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
