using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelController : MonoBehaviour
{
    [SerializeField] List<PuzzleItem> _listPuzzle;
    [SerializeField] SpriteRenderer _mainSprite;
    [SerializeField] SpriteRenderer _previewSprite;
    [SerializeField] GameObject _lineObj;
    Coroutine _changeAllPuzzleAlphaCoroutine;

    public List<PuzzleItem> ListPuzzle => _listPuzzle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        KillPuzzleChangeAlphaCoroutine();
        for (int i = 0; i < _listPuzzle.Count; i++)
        {
            _listPuzzle[i].DOKill();
        }
    }
    private void OnDestroy()
    {
        KillPuzzleChangeAlphaCoroutine();
        for (int i = 0; i < _listPuzzle.Count; i++)
        {
            _listPuzzle[i].DOKill();
        }
    }

    public void LoadLevel()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < _listPuzzle.Count; i++)
        {
            _listPuzzle[i].Id = i;
            //_listPuzzle[i].ShowLineObj(true);
            _listPuzzle[i].IsCorrect = false;
        }
        if (_previewSprite != null)
            _previewSprite.color = new Color(1, 1, 1, 0f);
        ChangeAllPuzzleAlpha(0, 0);
    }
    public void ChangeAllPuzzleAlpha(float alpha, float time, Action completed = null)
    {
        KillPuzzleChangeAlphaCoroutine();
        _changeAllPuzzleAlphaCoroutine = StartCoroutine(ChangeAllPuzzleAlphaCoroutine(alpha, time, completed));
    }

    IEnumerator ChangeAllPuzzleAlphaCoroutine(float alpha, float time, Action completed = null)
    {
        for (int i = 0; i < _listPuzzle.Count; i++)
        {
            _listPuzzle[i].ChangePuzzleAlpha(alpha, time);
        }
        yield return new WaitForSeconds(time);
        completed?.Invoke();
    }

    void KillPuzzleChangeAlphaCoroutine()
    {
        if(_changeAllPuzzleAlphaCoroutine != null)
        {
            StopCoroutine(_changeAllPuzzleAlphaCoroutine);
            _changeAllPuzzleAlphaCoroutine = null;
        }
    }

    public void CheckWinGame()
    {
        bool isWin = true;
        for(int i=0; i< _listPuzzle.Count; i++)
        {
            if (!_listPuzzle[i].IsCorrect)
            {
                isWin = false;
                break;
            }
        }
        if(isWin)
        {
            GameManager.Instance.IsWinGame = true;
            GameManager.Instance.ChangeGameState(GameState.End);
        }
    }

    public void ShowLevelInStart(bool isShow)
    {
        _lineObj.SetActive(!isShow);
        for(int i=0; i< _listPuzzle.Count; i++)
        {
            _listPuzzle[i].ShowLineObj(!isShow);
            _listPuzzle[i].ChangePuzzleAlpha(isShow ? 1 : 0, 0);
        }
    }
}
