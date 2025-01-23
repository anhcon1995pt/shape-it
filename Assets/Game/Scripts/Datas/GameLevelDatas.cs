using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Level Datas", menuName = "Level Datas")]
public class GameLevelDatas : ScriptableObject
{
    public List<LevelData> ListLevelDatas;
    
}

[System.Serializable]
public class LevelData
{
    public int Id;
    public float TimeLevel;
    public int Coin;
    public AssetReference LevelPrefab;
    
}
