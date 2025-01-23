using AC.GameTool.Attribute;
using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameplayUI : NormalUI
{
    [SerializeField] TextMeshProUGUI _txtLevel;
    [SerializeField] TimeBarUI _timebar;
    [SerializeField] Button _btnPause;
    [SerializeField, ReadOnly] bool _isPause;
    [SerializeField] GameObject _imgPause, _imgPlay;
    [SerializeField] TextMeshProUGUI _txtTime;
    [Header("Panel Puzzle")]
    [SerializeField] GameObject _puzzleObj;
    [SerializeField] List<PuzzleItemUI> _puzzleItemUIs;
    [SerializeField, ReadOnly] PuzzleItemUI _currentPuzzleItemUiSelected;
    [SerializeField]
    List<PuzzleItem> _listPuzzle;
    [SerializeField] LayerMask _puzzleLayer;
    [SerializeField, ReadOnly] List<PuzzleItem> _listPuzzleShow;
    [SerializeField] Button _btnHint, _btnFreeze;
    [Header("Time Freeze")]
    [SerializeField] GameObject _freezeTimeObj;
    [SerializeField] Image _imgFreezeTime;
    [SerializeField] Image _imgButtonFreeze;
    [Header("Combo")]
    [SerializeField] Animator _comboAni;
    [SerializeField] TextMeshProUGUI _txtCombo;
    [Header("Coin")]
    [SerializeField, ReadOnly] int _coin;
    [SerializeField] TextMeshProUGUI _txtCoin;
    [SerializeField] GameObject _iconAdBtnHints,_iconAdsTimeFreeze;
    [SerializeField] GameObject _showCoinBtnHints, _showCoinTimeFreeze;
    
    [SerializeField] Transform _coinBarIcon;
    PuzzleItemUI _puzzleHint;
    public DamageImageController DamageCtrl;

    public PuzzleItemUI CurrentPuzzleItemUiSelected { get => _currentPuzzleItemUiSelected; set => _currentPuzzleItemUiSelected = value; }
    public PuzzleItemUI PuzzleHint { get => _puzzleHint; set => _puzzleHint = value; }

    protected override void Awake()
    {
        base.Awake();
        AddAllListener();
        
    }
    private void OnDestroy()
    {
        RemoveAllListener();
    }

    void AddAllListener()
    {
        _btnPause.onClick.AddListener(BtnPauseGame_Click);
        _btnHint.onClick.AddListener(BtnHint_Click);
        _btnFreeze.onClick.AddListener(BtnFreeze_click);
        for (int i = 0; i < _puzzleItemUIs.Count; i++)
        {
            _puzzleItemUIs[i].PuzzleOnClick += PuzzleItemUI_CLick;
        }
        SaveManager.Instance.OnChangeGameData += ChangeGameDataCallback;
    }

    void RemoveAllListener()
    {
        _btnPause.onClick.RemoveListener(BtnPauseGame_Click);
        _btnHint.onClick.RemoveListener(BtnHint_Click);
        _btnFreeze.onClick.RemoveListener(BtnFreeze_click);
        for (int i = 0; i < _puzzleItemUIs.Count; i++)
        {
            _puzzleItemUIs[i].PuzzleOnClick -= PuzzleItemUI_CLick;
        }
        SaveManager.Instance.OnChangeGameData -= ChangeGameDataCallback;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
#if UNITY_EDITOR
        MouseDownEvent();
#elif UNITY_ANDROID || UNITY_IOS
        TouchDownEvent();
#endif
    }

    void MouseDownEvent()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastItem(Input.mousePosition);
        }
    }
    void TouchDownEvent()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            {
                RaycastItem(touch.position);
            }
        }      
    }


    void RaycastItem(Vector3 pointPos)
    {
        if (CurrentPuzzleItemUiSelected == null) return;
        RaycastHit2D[] hit2Ds = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(pointPos), Mathf.Infinity, _puzzleLayer);
        for(int i = 0; i < hit2Ds.Length; i++)
        {
            RaycastHit2D hit2D = hit2Ds[i];
            if (hit2D.collider != null)
            {
                PuzzleItem puzzle = hit2D.collider.GetComponent<PuzzleItem>();
                if (puzzle != null)
                {
                    if (puzzle.Id == CurrentPuzzleItemUiSelected.Id)
                    {
                        puzzle.SetPuzzleItemIsCorrect();
                        CurrentPuzzleItemUiSelected.OnPuzzleCorrect?.Invoke(CurrentPuzzleItemUiSelected);
                        return;
                    }
                    
                }
            }
        }
        if(CurrentPuzzleItemUiSelected != null)
        {
            VibrateManager.Instance.VibratePuzzleFaile();
            DamageCtrl.StartShowDamage();
            CurrentPuzzleItemUiSelected.ShowSelected(false);
            CurrentPuzzleItemUiSelected.RunAniFaile();
            CurrentPuzzleItemUiSelected = null;
        }        
    }

    public override void OnUiShow()
    {
        base.OnUiShow();
        GameManager.Instance.OnAfterGameStateChanged += OnChangeGameState;
        ChangeGamePlayOrPause(_isPause);
        ChangeGameDataCallback(SaveManager.Instance.GameData);
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
        GameManager.Instance.OnAfterGameStateChanged -= OnChangeGameState;
    }

    void OnChangeGameState(GameState state)
    {
        _btnHint.interactable = state == GameState.Play && !_isPause;
        _btnFreeze.interactable = GameManager.Instance.GameState == GameState.Play && !_isPause;
    }

    public void UpdateTimebar(float time, float percent)
    {
        _timebar.UpdateTimeBarProcess(percent);
        int timeInt = Mathf.CeilToInt(time);
        int min = timeInt / 60;
        int s = timeInt % 60;
        _txtTime.SetText("{0:00}:{1:00}", min, s);
    }

    public void ShowTimeBar(bool isShow)
    {
        _timebar.gameObject.SetActive(isShow);
    }

    void ChangeGamePlayOrPause(bool isPause)
    {
        _imgPlay.SetActive(!isPause);
        _imgPause.SetActive(isPause);
    }

    void BtnPauseGame_Click()
    {
        _isPause = !_isPause;
        ChangeGamePlayOrPause(_isPause);
        Time.timeScale = _isPause ? 0 : 1;
        _btnHint.interactable = GameManager.Instance.GameState == GameState.Play && !_isPause;
        _btnFreeze.interactable = GameManager.Instance.GameState == GameState.Play && !_isPause;
        GameManager.Instance.IsPauseGame = _isPause;
    }


    void BtnHint_Click()
    {
        GameData gameData = SaveManager.Instance.GameData;
        int coin = gameData.Coin;
        if (coin >= 100)
        {
            coin -= 100;
            gameData.Coin = coin;
            SaveManager.Instance.GameData = gameData;
            ActiveHints();
        }
        else
        {
            //SHow Ads
            ActiveHints();
        }

       
    }

    public void ActiveHints()
    {
        PuzzleHint = null;
        PuzzleItemUI puzzleItemUI = _puzzleItemUIs.Find(x => x.IsActive);
        if (puzzleItemUI != null)
        {
            PuzzleHint = puzzleItemUI;
            PuzzleHint.RunAnimationFadeItem();
            PuzzleHint.Puzzle.RunAnimationFadeItem();
        }
    }

    public void DeActiveHints()
    {
        if (PuzzleHint == null) return;
        PuzzleHint.StopAnimationFadeItem();
        PuzzleHint.Puzzle.StopAnimationFadeItem();
        PuzzleHint = null;
    }

    public void ShowFreezeTimeer(bool isShow)
    {
        _freezeTimeObj.SetActive(isShow);
    }

    public void UpdateFreezeTimePercent(float percent)
    {
        _imgFreezeTime.fillAmount = percent;
        _imgButtonFreeze.fillAmount = 1f- percent;
        _btnFreeze.interactable = percent <= 0;
    }

    void BtnFreeze_click()
    {
        //if (_listPuzzle.Count <= 0) return;
        //_listPuzzle.AddRange(_listPuzzleShow);
        //Show3PuzzleInUI();
        GameData gameData = SaveManager.Instance.GameData;
        int coin = gameData.Coin;
        if(coin >= 200)
        {
            coin -= 200;
            gameData.Coin = coin;
            SaveManager.Instance.GameData = gameData;
            ActiveTimeFreeze();
        }
        else
        {
            //SHow Ads
            ActiveTimeFreeze();
        }
        
    }

    void ActiveTimeFreeze()
    {
        GameManager.Instance.ActiveFreezeGame();
    }

    public void SetLevel(int levelID)
    {
        _txtLevel.SetText("LEVEL {0}", levelID + 1);
    }

    public void SetupAllPuzzle(List<PuzzleItem> listPuzzle)
    {
        _listPuzzle = new List<PuzzleItem>(listPuzzle);
        Show3PuzzleInUI();
    }

    void Show3PuzzleInUI()
    {
        _listPuzzleShow.Clear();
        int createCount = Mathf.Min(_listPuzzle.Count, _puzzleItemUIs.Count);
        for(int i=0; i< createCount; i++)
        {
            PuzzleItem puzzle = _listPuzzle[0];
            _puzzleItemUIs[i].SetActivePuzzleUI(true);
            _puzzleItemUIs[i].SetPuzzleData(puzzle, PuzzleCorrect);
            _listPuzzleShow.Add(puzzle);
            _listPuzzle.Remove(puzzle);
        }

        for(int i = createCount; i< _puzzleItemUIs.Count; i++)
        {
            _puzzleItemUIs[i].SetActivePuzzleUI(false);
        }
        for (int i = 0; i < _puzzleItemUIs.Count; i++)
        {
            _puzzleItemUIs[i].ShowSelected(false);
        }
        CurrentPuzzleItemUiSelected = null;
    }

    void PuzzleCorrect(PuzzleItemUI puzzleItem)
    {
        // remove Puzzle in _listPuzzleShow
        PuzzleItem puzzle = puzzleItem.Puzzle;
        if(puzzle != null)
        {
            _listPuzzleShow.Remove(puzzle);
        }
        if(_listPuzzle.Count > 0)
        {
            PuzzleItem puzzleShowNew = _listPuzzle[0];
            puzzleItem.SetActivePuzzleUI(true);
            puzzleItem.SetPuzzleData(puzzleShowNew, PuzzleCorrect);
            _listPuzzleShow.Add(puzzleShowNew);
            _listPuzzle.Remove(puzzleShowNew);
        }
        else
        {
            puzzleItem.SetActivePuzzleUI(false);            
        }
        CurrentPuzzleItemUiSelected = null;
        puzzleItem.ShowSelected(false);
        _btnFreeze.interactable = GameManager.Instance.GameState == GameState.Play && !_isPause;
    }

    void PuzzleItemUI_CLick(PuzzleItemUI puzzleItemUi)
    {
        if(CurrentPuzzleItemUiSelected != puzzleItemUi)
        {
            if(CurrentPuzzleItemUiSelected != null)
            {
                CurrentPuzzleItemUiSelected.ShowSelected(false);
            }
            CurrentPuzzleItemUiSelected = puzzleItemUi;
            CurrentPuzzleItemUiSelected.ShowSelected(true);
            GameData gameData = SaveManager.Instance.GameData;
            if (!gameData.IsTutCompleted)
            {
                if(gameData.TutIndex == 0)
                {
                    gameData.TutIndex = 1;
                    SaveManager.Instance.GameData = gameData;
                    UIManager.Instance.TryShowUI(UiTypeName.Tutorial);
                }                
            }
        }
    }

    public void ShowPuzzlePanel(bool isShow)
    {
        _puzzleObj.SetActive(isShow);
        ShowFreezeTimeer(false);
        ShowTimeBar(isShow);
        //_txtTime.transform.parent.gameObject.SetActive(isShow);
    }

    public void ShowCombo(int combo)
    {
        _txtCombo.SetText($"COMBO X<size=150%>{combo}");
        _comboAni.Play("combo");
    }

    void ChangeGameDataCallback(GameData gameData)
    {
        if(_coin != SaveManager.Instance.GameData.Coin)
        {
            _coin = SaveManager.Instance.GameData.Coin;
            _txtCoin.SetText("{0}", _coin);

            bool showCoinBtnHint = _coin >= 100;
            bool showCoinBtnTimeFreeze = _coin >= 200;
            _iconAdBtnHints.SetActive(!showCoinBtnHint);
            _iconAdsTimeFreeze.SetActive(!showCoinBtnTimeFreeze);
            _showCoinBtnHints.SetActive(showCoinBtnHint);
            _showCoinTimeFreeze.SetActive(showCoinBtnTimeFreeze);
        }       
    }

    public void ShowCoinFly(Vector3 pos, int countFly, int coinEarn, Action completed = null)
    {
        int coin = _coin;
        int deltaCoin = coinEarn / countFly;
        CoinFlyUI coinFlyUI = UIManager.Instance.TryShowUI(UiTypeName.CoinFly) as CoinFlyUI;

        coinFlyUI.ShowCoinFly(pos, _coinBarIcon.position, countFly, coinEarn, indexFly =>
        {
            if (indexFly >= countFly - 1)
            {
                completed?.Invoke();
            }
            else
            {
                coin += deltaCoin;
                _txtCoin.SetText("{0}", coin);
            }
            _txtCoin.transform.DOKill();
            _txtCoin.transform.localScale = Vector3.one;
            _txtCoin.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.5f, 1).OnComplete(() =>
            {
                _txtCoin.transform.localScale = Vector3.one;
            });
        });
        
    }
}
