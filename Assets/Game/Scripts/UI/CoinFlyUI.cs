using AC.GameTool.Core;
using AC.GameTool.SaveData;
using AC.GameTool.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CoinFlyUI : NormalUI
{
    [SerializeField] AssetReference _coinFlyPrefab;
    [SerializeField] Transform _coinFlyParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    private void OnDestroy()
    {
        
    }
    public override void OnUiShow()
    {
        base.OnUiShow();
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
    }

    public void ShowCoinFly(Vector3 posStart, Vector3 PosEnd, int countFly, int coinEarn, Action<int> flyEndSuccessed)
    {
        int deltaCoin = coinEarn / countFly;
        int countEnd = 0;
        for (int i = 0; i < countFly; i++)
        {
            GameObject coinFly = PoolManager.Instance.Spawn(_coinFlyPrefab, _coinFlyParent);
            Vector3 randomPos = Random.insideUnitCircle * 200;
            coinFly.transform.position = posStart;
            coinFly.SetActive(true);
            Sequence sequence = DOTween.Sequence(coinFly.transform);
            sequence.Append(coinFly.transform.DOMove(posStart + randomPos, 0.15f));
            sequence.Append(coinFly.transform.DOMove(PosEnd, 0.85f).SetDelay(i * 0.2f).OnComplete(() =>
            {
                flyEndSuccessed?.Invoke(countEnd);
                PoolManager.Instance.ReturnToPool(coinFly);
                countEnd += 1;
            }));
        }
    }
}
