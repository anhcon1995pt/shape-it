using AC.GameTool.Attribute;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBarUI : MonoBehaviour
{
    [SerializeField, ReadOnly, Range(0f, 1f)] float _percent;
    [SerializeField] Image _imgTimeBar;
    [SerializeField] int _countStar;
    [SerializeField] List<GameObject> _listStar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTimeBarProcess(float percent)
    {
        _percent = percent;
        _imgTimeBar.fillAmount = _percent;
        int countStar = GameManager.Instance.GetCountStar();
        ChangeCountStar(countStar);
    }

    

    void ChangeCountStar(int count)
    {
        if(_countStar != count)
        {
            _countStar = count;
            for(int i=0; i< _listStar.Count; i++)
            {
                _listStar[i].SetActive(i < _countStar);
            }
        }
    }
}
