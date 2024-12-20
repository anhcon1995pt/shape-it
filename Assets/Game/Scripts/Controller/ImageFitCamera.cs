using AC.GameTool.Attribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFitCamera : MonoBehaviour
{
    [SerializeField]
    Vector2 _defaultScreen;
    [SerializeField, ReadOnlly]
    Vector2 _currentScreen;
    float _defaultRatio, _currentRatio;
    float _defaultSizeY, _currentSizeY;
    private void Awake()
    {
        _currentScreen = new Vector2(Screen.width, Screen.height);
        _defaultRatio = _defaultScreen.y / _defaultScreen.x;
        _currentRatio = _currentScreen.y / _currentScreen.x;
        _defaultSizeY = transform.localScale.y;
        _currentSizeY = _defaultSizeY * _currentRatio / _defaultRatio;
        transform.localScale = new Vector3(transform.localScale.x, _currentSizeY);
    }
    // Start is called before the first frame update
    void Start()
    {
        ScreenEventManager.Instance.OnScreenChange += OnChangeScreenSize;
    }
    private void OnDestroy()
    {
        ScreenEventManager.Instance.OnScreenChange -= OnChangeScreenSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangeScreenSize(Vector2Int screenSize)
    {
        _currentScreen = screenSize;
        _currentRatio = _currentScreen.y / _currentScreen.x;
        _currentSizeY = _defaultSizeY * _currentRatio / _defaultRatio;
        transform.localScale = new Vector3(transform.localScale.x, _currentSizeY);
    }
}
