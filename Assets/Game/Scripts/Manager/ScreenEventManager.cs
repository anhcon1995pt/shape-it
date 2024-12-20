using AC.GameTool.Attribute;
using AC.GameTool.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenEventManager : Singleton<ScreenEventManager>
{
    [SerializeField, ReadOnly] Vector2Int _currentScreen;
    public Vector2Int CurrentScreent => _currentScreen;

    public Action<Vector2Int> OnScreenChange;
    protected override void Awake()
    {
        base.Awake();
        _currentScreen = new Vector2Int(Screen.width, Screen.height);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentScreen.x != Screen.width || _currentScreen.y != Screen.height)
        {
            _currentScreen = new Vector2Int(Screen.width, Screen.height);
            OnScreenChange?.Invoke(_currentScreen);
        }
    }
}
