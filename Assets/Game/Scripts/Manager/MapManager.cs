using AC.GameTool.Attribute;
using AC.GameTool.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [Header("List Level")]
    [SerializeField] Transform _levelParrent;
    [SerializeField, ReadOnlly] LevelController _levelCtrl;
    Dictionary<int, LevelController> _dicLevelCtrl = new Dictionary<int, LevelController>();
    public Vector2 PuzzleOffsetMove;
    public LevelController CurrentLevelCtrl => _levelCtrl;
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(int levelID, Action completed)
    {       
        if (levelID < 0)
        {
            Debug.LogError("Loi Level ID");
            completed?.Invoke();
            return;
        }
        int index = levelID % AssetManager.Instance.CountLevel;
        if(_dicLevelCtrl.ContainsKey(index)) 
        {
            if (_levelCtrl != null)
            {
                _levelCtrl.gameObject.SetActive(false);
            }
            _levelCtrl = _dicLevelCtrl[index];
            _levelCtrl.LoadLevel();
            CanvasManager.Instance.SetupAllPuzzle(_levelCtrl.ListPuzzle);
            completed?.Invoke();

        }
        else
        {
            AssetManager.Instance.GetLevelPrefab(levelID, levelPreb =>
            {
                if (levelPreb == null) {
                    Debug.LogError("khong Load dc Level");
                    completed?.Invoke();
                    return;
                };
                if (_levelCtrl != null)
                {
                    _levelCtrl.gameObject.SetActive(false);
                }
                _levelCtrl = Instantiate(levelPreb, _levelParrent).GetComponent<LevelController>();
                _levelCtrl.LoadLevel();
                _dicLevelCtrl.Add(index, _levelCtrl);
                CanvasManager.Instance.SetupAllPuzzle(_levelCtrl.ListPuzzle);
                completed?.Invoke();
            });
        }
       
    }
}
