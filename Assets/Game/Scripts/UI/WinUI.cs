using AC.GameTool.Attribute;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : NormalUI
{
    [SerializeField] Button _btnNothank, _btnClaim;
    [SerializeField] Animator _aniStarPlay;
    [SerializeField] List<GameObject> _listStarObj;
    [SerializeField, ReadOnly] int _coinEarn;
    [SerializeField] TextMeshProUGUI _txtCoinEarn;
    [Header("Bonus")]
    [SerializeField] float _timeRunAniBonus;
    [SerializeField] Transform _pointTranf;
    [SerializeField] int _indexBonus;
    [SerializeField] float _zPoinAngle;
    [SerializeField] TextMeshProUGUI[] _txtBonus;
    [SerializeField] Color _bonusActiveColor;
    Tween _aniBonusTween;

    protected override void Awake()
    {
        base.Awake();
        _btnNothank.onClick.AddListener(BtnNothank_Click);
        _btnClaim.onClick.AddListener(BtnClaim_Click);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        _btnNothank.onClick.RemoveListener(BtnNothank_Click);
        _btnClaim.onClick.RemoveListener(BtnClaim_Click);
    }
    public override void OnUiShow()
    {
        base.OnUiShow();
        _mainCanvasGroup.interactable = true;
        int countStar = GameManager.Instance.GetCountStar();
        for(int i=0; i< _listStarObj.Count; i++)
        {
            _listStarObj[i].SetActive(i< countStar);
        }
        _aniStarPlay.Play("starAni");
        _coinEarn = GameManager.Instance.GetWinLevelCoinEarn();
        _txtCoinEarn.SetText(_coinEarn.ToString());
        _aniBonusTween.Kill();
        _zPoinAngle = -30;
        _indexBonus = 2;
        ChangeColorTextBonus(_indexBonus);       
        _aniBonusTween = DOTween.To(() => _zPoinAngle, x => _zPoinAngle = x, 30f, _timeRunAniBonus).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).OnUpdate(() =>
        {
            _pointTranf.transform.localEulerAngles = new Vector3(0, 0, _zPoinAngle);
            int indexBonus = GetIndexBonusWithRotZ(_zPoinAngle);
            if (_indexBonus != indexBonus)
            {
                ChangeColorTextBonus(indexBonus);
                int mul = indexBonus == 1? 4 : 2;
                _txtCoinEarn.SetText("{0}", mul * _coinEarn);
                _indexBonus = indexBonus;
            }
        });
        _btnNothank.transform.DOKill();
        _btnNothank.transform.localScale = Vector3.zero;
        _btnNothank.transform.DOScale(1, 1f).SetDelay(3).OnComplete(() =>
        {
            _btnNothank.transform.localScale = Vector3.one;
        });
    }

    void ChangeColorTextBonus(int index)
    {
        for(int i=0; i< _txtBonus.Length; i++)
        {
            _txtBonus[i].color = (i != index) ? Color.white : _bonusActiveColor;
        }
    }

    int GetIndexBonusWithRotZ(float rotZ)
    {
        if(rotZ > 12f)
        {
            return 0;
        }
        else if(rotZ < -12f)
        {
            return 2;
        }
        return 1;
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
        _aniBonusTween.Kill();
        _btnNothank.transform.DOKill();
        _btnNothank.transform.localScale = Vector3.one;
        _mainCanvasGroup.interactable = true;
    }

    public void BtnNothank_Click()
    {
        _mainCanvasGroup.interactable = false;
        CanvasManager.Instance.GameplayUI.ShowCoinFly(_btnClaim.transform.position, 5, _coinEarn, () =>
        {
            _mainCanvasGroup.interactable = true;
            GameData gameData = SaveManager.Instance.GameData;
            gameData.Coin += _coinEarn;
            SaveManager.Instance.GameData = gameData;
            GameManager.Instance.ChangeGameState(GameState.Load);
            OnCloseUI();
        });
        
    }

    public void BtnClaim_Click()
    {
        _aniBonusTween.Kill();
        int mutilCoin = 2;
        if (_indexBonus == 1) mutilCoin = 4;
        int coinEarn = _coinEarn * mutilCoin;
        _mainCanvasGroup.interactable = false;
        CanvasManager.Instance.GameplayUI.ShowCoinFly(_btnClaim.transform.position, 5, coinEarn, () =>
        {
            _mainCanvasGroup.interactable = true;
            GameData gameData = SaveManager.Instance.GameData;
            gameData.Coin += _coinEarn * mutilCoin;
            SaveManager.Instance.GameData = gameData;
            GameManager.Instance.ChangeGameState(GameState.Load);
            OnCloseUI();
        });
        
    }
}
