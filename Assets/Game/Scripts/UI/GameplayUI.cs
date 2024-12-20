using AC.GameTool.Attribute;
using AC.GameTool.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        if (_currentPuzzleItemUiSelected == null) return;
        RaycastHit2D[] hit2Ds = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(pointPos), Mathf.Infinity, _puzzleLayer);
        for(int i = 0; i < hit2Ds.Length; i++)
        {
            RaycastHit2D hit2D = hit2Ds[i];
            if (hit2D.collider != null)
            {
                PuzzleItem puzzle = hit2D.collider.GetComponent<PuzzleItem>();
                if (puzzle != null)
                {
                    if (puzzle.Id == _currentPuzzleItemUiSelected.Id)
                    {
                        puzzle.SetPuzzleItemIsCorrect();
                        _currentPuzzleItemUiSelected.OnPuzzleCorrect?.Invoke(_currentPuzzleItemUiSelected);
                        break;
                    }

                }
            }
        }
        if(_currentPuzzleItemUiSelected != null)
        {
            _currentPuzzleItemUiSelected.ShowSelected(false);
            _currentPuzzleItemUiSelected = null;
        }        
    }

    public override void OnUiShow()
    {
        base.OnUiShow();
        GameManager.Instance.OnAfterGameStateChanged += OnChangeGameState;
        ChangeGamePlayOrPause(_isPause);

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
        PuzzleItemUI puzzleItemUI = _puzzleItemUIs.Find(x => x.IsActive);
        if (puzzleItemUI != null)
        {
            puzzleItemUI.RunAnimationFadeItem();
            puzzleItemUI.Puzzle.RunAnimationFadeItem();
        }
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
        _currentPuzzleItemUiSelected = null;
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
        _currentPuzzleItemUiSelected = null;
        puzzleItem.ShowSelected(false);
        _btnFreeze.interactable = GameManager.Instance.GameState == GameState.Play && !_isPause;
    }

    void PuzzleItemUI_CLick(PuzzleItemUI puzzleItemUi)
    {
        if(_currentPuzzleItemUiSelected != puzzleItemUi)
        {
            if(_currentPuzzleItemUiSelected != null)
            {
                _currentPuzzleItemUiSelected.ShowSelected(false);
            }
            _currentPuzzleItemUiSelected = puzzleItemUi;
            _currentPuzzleItemUiSelected.ShowSelected(true);
        }
    }

    public void ShowPuzzlePanel(bool isShow)
    {
        _puzzleObj.SetActive(isShow);
        ShowFreezeTimeer(false);
        ShowTimeBar(isShow);
        //_txtTime.transform.parent.gameObject.SetActive(isShow);
    }
}
