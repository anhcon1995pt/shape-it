using AC.GameTool.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : NormalUI
{
    [SerializeField] Button _btnNext;
    [SerializeField] List<GameObject> _listStarObj;
    protected override void Awake()
    {
        base.Awake();
        _btnNext.onClick.AddListener(WinPanel_Click);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        _btnNext.onClick.RemoveListener(WinPanel_Click);
    }
    public override void OnUiShow()
    {
        base.OnUiShow();
        int countStar = GameManager.Instance.GetCountStar();
        for(int i=0; i< _listStarObj.Count; i++)
        {
            _listStarObj[i].SetActive(i< countStar);
        }
    }
    public override void OnCloseUI()
    {
        base.OnCloseUI();
    }

    public void WinPanel_Click()
    {
        GameManager.Instance.ChangeGameState(GameState.Load);
        OnCloseUI();
    }

    
}
