using AC.GameTool.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetManager : Singleton<AssetManager>
{
    [SerializeField] GameLevelDatas _gameLevelDatas;
    Dictionary<AssetReference, AsyncOperationHandle<GameObject>> _dicLevelLoadhanle = new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();
    public GameLevelDatas GameLevelDatas => _gameLevelDatas;

    public int CountLevel => _gameLevelDatas.ListLevelDatas.Count;
    protected override void Awake()
    {
        base.Awake();
    }

    public void GetLevelPrefab(int levelID, Action<GameObject> completed)
    {
        if (levelID < 0 || CountLevel < 0) 
        {
            completed?.Invoke(null);
            return;
        }
        int index = levelID % CountLevel;
        AssetReference levelRef = _gameLevelDatas.ListLevelDatas[index].LevelPrefab;
        if(_dicLevelLoadhanle.ContainsKey(levelRef))
        {
            completed?.Invoke(_dicLevelLoadhanle[levelRef].Result);
        }
        else
        {
            AsyncOperationHandle<GameObject> loadAsset = levelRef.LoadAssetAsync<GameObject>();
            loadAsset.Completed += hanlde =>
            {
                _dicLevelLoadhanle.Add(levelRef, hanlde);
                completed?.Invoke(hanlde.Result);
            };
        }
    }

}
