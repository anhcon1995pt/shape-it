using AC.GameTool.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateManager : Singleton<VibrateManager>
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        Vibration.Init();
    }

    public void VibratePuzzleCorrect()
    {
        Vibration.VibratePop();
    }

    public void VibratePuzzleFaile()
    {
        Vibration.VibratePeek();
    }

    public void VibrateGameLose()
    {
        Vibration.Vibrate();
    }

    public void VibrateGameWin()
    {
        Vibration.VibratePeek();
    }
}
