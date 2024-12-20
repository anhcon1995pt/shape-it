using AC.GameTool.Attribute;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleItemUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, ReadOnly] Image _mainImage;
    [SerializeField, ReadOnly] int _id;
    [SerializeField, ReadOnly] PuzzleItem _puzzle;
    [SerializeField, ReadOnly] Vector2 _defaultSize;
    [SerializeField, ReadOnly] Vector2 _ingameSize;
    [SerializeField, ReadOnly] Transform _defaultParent;
    [SerializeField, ReadOnly] RectTransform _rectTranform;
    [SerializeField, ReadOnly] bool _isDraged;
    [SerializeField] RectTransform _contentRect;
    [SerializeField] Vector2 _offsetDrag;
    [SerializeField, ReadOnly] bool _isInsideContentRect;
    [SerializeField] LayerMask _puzzleLayer;
    [SerializeField] GameObject _lineObj;
    public Action<PuzzleItemUI> OnPuzzleCorrect;

    public Action<PuzzleItemUI> PuzzleOnClick;

    public PuzzleItem Puzzle => _puzzle;
    public bool IsActive;
    public int Id => _id;
    private void Awake()
    {
        _rectTranform = GetComponent<RectTransform>();
        _mainImage = GetComponent<Image>();
        _ingameSize = _mainImage.sprite.rect.size / _mainImage.pixelsPerUnit;
        _defaultSize = _rectTranform.sizeDelta;
        _defaultParent = _rectTranform.parent;
        
    }
    private void OnDisable()
    {
        _rectTranform.DOKill();
    }
    // Start is called before the first frame update
    void Start()
    {
        _offsetDrag = MapManager.Instance.PuzzleOffsetMove;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActivePuzzleUI(bool isActive)
    {
        IsActive = isActive;
        transform.parent.gameObject.SetActive(isActive);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
        if (GameManager.Instance.IsPauseGame) return;
        if (_isDraged) return;
        
        PuzzleOnClick?.Invoke(this);                
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
        if (GameManager.Instance.IsPauseGame) return;
        transform.localPosition = Vector3.zero;
        _rectTranform.sizeDelta = _defaultSize;
        ShowSelected(true);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
        if (GameManager.Instance.IsPauseGame) return;
        if (_isDraged) return;
        ShowSelected(false);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTranform, eventData.position + _offsetDrag, eventData.pressEventCamera, out Vector3 woorldPoint))
        {
            _isDraged = true;
            _isInsideContentRect = RectTransformUtility.RectangleContainsScreenPoint(_contentRect, eventData.position + _offsetDrag);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
        if (GameManager.Instance.IsPauseGame) return;
        if (!_isDraged) return; 
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_rectTranform, eventData.position + _offsetDrag, eventData.pressEventCamera, out Vector3 woorldPoint))
        {
            _rectTranform.position = woorldPoint;
            bool isInsideContentRect = RectTransformUtility.RectangleContainsScreenPoint(_contentRect, eventData.position + _offsetDrag);
            if (isInsideContentRect != _isInsideContentRect)
            {
                _isInsideContentRect = isInsideContentRect;
                _rectTranform.DOKill();
                _rectTranform.DOSizeDelta(_isInsideContentRect ? _defaultSize : _ingameSize, 0.15f);
            }
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.GameState != GameState.Play) return;
        if (GameManager.Instance.IsPauseGame) return;
        if (!_isDraged) return;
        
        _isDraged = false;
        _rectTranform.DOKill();
        _isInsideContentRect = true;
        transform.localPosition = Vector3.zero;
        _rectTranform.sizeDelta = _defaultSize;
        if (GameManager.Instance.GameState != GameState.Play) return;
        RaycastHit2D[] hit2Ds = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(eventData.position + _offsetDrag), Mathf.Infinity, _puzzleLayer);

        for(int i=0; i<hit2Ds.Length; i++)
        {
            RaycastHit2D hit2D = hit2Ds[i];
            if (hit2D.collider != null)
            {
                PuzzleItem puzzle = hit2D.collider.GetComponent<PuzzleItem>();
                if (puzzle != null)
                {
                    if (puzzle.Id == _id)
                    {
                        puzzle.SetPuzzleItemIsCorrect();
                        OnPuzzleCorrect?.Invoke(this);
                        break;
                    }                   
                }
            }
        }
        
    }


    public void SetPuzzleData(PuzzleItem puzzle, Action<PuzzleItemUI> puzzleCorrect)
    {
        _puzzle = puzzle;
        _id = _puzzle.Id;       
        _mainImage.sprite = _puzzle.MainSprite;
        OnPuzzleCorrect = puzzleCorrect;
    }

    public void RunAnimationFadeItem()
    {
        _mainImage.DOKill();
        Color color = _mainImage.color;
        color.a = 1f;
        _mainImage.color = color;
        _mainImage.DOFade(0, 1f).SetEase(Ease.InOutFlash, 10, 1).OnComplete(() =>
        {
            Color color = _mainImage.color;
            color.a = 1f;
            _mainImage.color = color;
        });
    }

    public void ShowSelected(bool isShow)
    {
        if (_lineObj != null)
        {
            _lineObj.SetActive(isShow);
        }
    }
}
